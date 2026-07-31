using DustsSpaceLaunchTracker.Configuration;
using DustsSpaceLaunchTracker.Models;
using DustsSpaceLaunchTracker.Models.Responses;
using DustsSpaceLaunchTracker.Services.Api;
using System.Collections.Concurrent;
using System.Text.Json;

namespace DustsSpaceLaunchTracker.Services.Data
{
    /// <summary>
    /// Memory + file-backed cache under AppData.
    /// Serves stale data within TTL when offline.
    /// </summary>
    public sealed class FileLaunchCache : ILaunchCache
    {
        private readonly ConcurrentDictionary<string, CacheEntry<PagedResult<Launch>>> _pages = new();
        private readonly ConcurrentDictionary<string, CacheEntry<Launch>> _details = new();
        private readonly string _root;
        private readonly JsonSerializerOptions _json;

        /// <param name="cacheRoot">
        /// Optional root directory (used by unit tests). When null, uses app data.
        /// </param>
        public FileLaunchCache(string? cacheRoot = null)
        {
            _root = string.IsNullOrWhiteSpace(cacheRoot)
                ? ResolveDefaultRoot()
                : cacheRoot;
            Directory.CreateDirectory(_root);
            _json = TheSpaceDevsJson.CreateOptions();
        }

        private static string ResolveDefaultRoot()
        {
#if ANDROID || IOS || MACCATALYST || WINDOWS || TIZEN
            return Path.Combine(FileSystem.AppDataDirectory, "launch-cache");
#else
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "DustsSpaceLaunchTracker",
                "launch-cache");
#endif
        }

        public async Task<PagedResult<Launch>?> GetPageAsync(string key, CancellationToken cancellationToken = default)
        {
            if (_pages.TryGetValue(key, out var mem) && !mem.IsExpired)
                return mem.Value;

            var path = PagePath(key);
            if (!File.Exists(path))
                return mem is { IsExpired: false } ? mem.Value : (mem?.Value);

            try
            {
                await using var stream = File.OpenRead(path);
                var disk = await JsonSerializer.DeserializeAsync<CacheEntryDto<PagedResult<Launch>>>(
                    stream, _json, cancellationToken);
                if (disk is null)
                    return null;

                var entry = new CacheEntry<PagedResult<Launch>>(disk.Value!, disk.SavedUtc);
                _pages[key] = entry;
                return entry.IsExpired ? entry.Value : entry.Value; // return even if expired (stale OK offline)
            }
            catch
            {
                return mem?.Value;
            }
        }

        public async Task SetPageAsync(string key, PagedResult<Launch> page, CancellationToken cancellationToken = default)
        {
            var entry = new CacheEntry<PagedResult<Launch>>(page, DateTime.UtcNow);
            _pages[key] = entry;

            var path = PagePath(key);
            var dto = new CacheEntryDto<PagedResult<Launch>>
            {
                SavedUtc = entry.SavedUtc,
                Value = page
            };

            await using var stream = File.Create(path);
            await JsonSerializer.SerializeAsync(stream, dto, _json, cancellationToken);
        }

        public async Task<Launch?> GetDetailAsync(string launchId, CancellationToken cancellationToken = default)
        {
            if (_details.TryGetValue(launchId, out var mem) && !mem.IsExpired)
                return mem.Value;

            var path = DetailPath(launchId);
            if (!File.Exists(path))
                return mem?.Value;

            try
            {
                await using var stream = File.OpenRead(path);
                var disk = await JsonSerializer.DeserializeAsync<CacheEntryDto<Launch>>(
                    stream, _json, cancellationToken);
                if (disk?.Value is null)
                    return null;

                var entry = new CacheEntry<Launch>(disk.Value, disk.SavedUtc);
                _details[launchId] = entry;
                return entry.Value;
            }
            catch
            {
                return mem?.Value;
            }
        }

        public async Task SetDetailAsync(Launch launch, CancellationToken cancellationToken = default)
        {
            var entry = new CacheEntry<Launch>(launch, DateTime.UtcNow);
            _details[launch.Id] = entry;

            var path = DetailPath(launch.Id);
            var dto = new CacheEntryDto<Launch> { SavedUtc = entry.SavedUtc, Value = launch };
            await using var stream = File.Create(path);
            await JsonSerializer.SerializeAsync(stream, dto, _json, cancellationToken);
        }

        public Task ClearAsync(CancellationToken cancellationToken = default)
        {
            _pages.Clear();
            _details.Clear();
            if (Directory.Exists(_root))
            {
                foreach (var file in Directory.EnumerateFiles(_root))
                {
                    try { File.Delete(file); } catch { /* ignore */ }
                }
            }

            return Task.CompletedTask;
        }

        private string PagePath(string key) =>
            Path.Combine(_root, $"page_{Sanitize(key)}.json");

        private string DetailPath(string id) =>
            Path.Combine(_root, $"detail_{Sanitize(id)}.json");

        private static string Sanitize(string key) =>
            string.Concat(key.Select(c => char.IsLetterOrDigit(c) ? c : '_'));

        private sealed class CacheEntry<T>
        {
            public CacheEntry(T value, DateTime savedUtc)
            {
                Value = value;
                SavedUtc = savedUtc;
            }

            public T Value { get; }
            public DateTime SavedUtc { get; }
            public bool IsExpired => DateTime.UtcNow - SavedUtc > AppConfig.CacheTtl;
        }

        private sealed class CacheEntryDto<T>
        {
            public DateTime SavedUtc { get; set; }
            public T? Value { get; set; }
        }
    }
}

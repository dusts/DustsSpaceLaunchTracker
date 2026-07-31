using DustsSpaceLaunchTracker.Configuration;
using DustsSpaceLaunchTracker.Models;
using DustsSpaceLaunchTracker.Models.Responses;
using DustsSpaceLaunchTracker.Services.Api;
using DustsSpaceLaunchTracker.Services.Data;
using Microsoft.Extensions.Logging;

namespace DustsSpaceLaunchTracker.Services
{
    public sealed class LaunchService : ILaunchService
    {
        private readonly ITheSpaceDevsApi _api;
        private readonly ILaunchCache _cache;
        private readonly ILogger<LaunchService> _logger;

        public LaunchService(
            ITheSpaceDevsApi api,
            ILaunchCache cache,
            ILogger<LaunchService> logger)
        {
            _api = api;
            _cache = cache;
            _logger = logger;
        }

        public Task<PagedResult<Launch>> GetUpcomingPageAsync(
            int limit,
            int offset,
            string? search = null,
            int? statusId = null,
            int? launcherId = null,
            bool forceRefresh = false,
            CancellationToken cancellationToken = default)
            => GetPageAsync(
                AppConfig.UpcomingCachePrefix,
                (l, o, s, st, ln, ct) => _api.GetUpcomingLaunchesAsync(
                    l, o, AppConfig.ListMode, s, st, ln, ct),
                limit, offset, search, statusId, launcherId, forceRefresh, cancellationToken);

        public Task<PagedResult<Launch>> GetPreviousPageAsync(
            int limit,
            int offset,
            string? search = null,
            int? statusId = null,
            int? launcherId = null,
            bool forceRefresh = false,
            CancellationToken cancellationToken = default)
            => GetPageAsync(
                AppConfig.PreviousCachePrefix,
                (l, o, s, st, ln, ct) => _api.GetPreviousLaunchesAsync(
                    l, o, AppConfig.ListMode, s, st, ln, ct),
                limit, offset, search, statusId, launcherId, forceRefresh, cancellationToken);

        public async Task<Launch> GetLaunchDetailAsync(
            string id,
            bool forceRefresh = false,
            CancellationToken cancellationToken = default)
        {
            if (!forceRefresh)
            {
                var cached = await _cache.GetDetailAsync(id, cancellationToken);
                if (cached is not null)
                {
                    // Refresh in background when online is ideal; for simplicity fetch fresh next
                    try
                    {
                        var fresh = await _api.GetLaunchDetailAsync(
                            id, AppConfig.DetailMode, cancellationToken);
                        await _cache.SetDetailAsync(fresh, cancellationToken);
                        return fresh;
                    }
                    catch (Exception ex) when (IsNetworkOrCancel(ex, cancellationToken))
                    {
                        _logger.LogWarning(ex, "Detail network failed; using cache for {Id}", id);
                        return cached;
                    }
                }
            }

            try
            {
                var launch = await _api.GetLaunchDetailAsync(
                    id, AppConfig.DetailMode, cancellationToken);
                await _cache.SetDetailAsync(launch, cancellationToken);
                return launch;
            }
            catch (Exception ex) when (!forceRefresh)
            {
                var cached = await _cache.GetDetailAsync(id, cancellationToken);
                if (cached is not null)
                {
                    _logger.LogWarning(ex, "Detail fetch failed; using cache for {Id}", id);
                    return cached;
                }

                throw;
            }
        }

        private async Task<PagedResult<Launch>> GetPageAsync(
            string prefix,
            Func<int, int, string?, int?, int?, CancellationToken, Task<LaunchListResponse>> fetcher,
            int limit,
            int offset,
            string? search,
            int? statusId,
            int? launcherId,
            bool forceRefresh,
            CancellationToken cancellationToken)
        {
            var cacheKey = BuildCacheKey(prefix, limit, offset, search, statusId, launcherId);

            if (!forceRefresh)
            {
                var cached = await _cache.GetPageAsync(cacheKey, cancellationToken);
                // Only short-circuit on first page when we want instant UI; still try network below
                // For offline: if network fails we fall back
            }

            try
            {
                var response = await fetcher(
                    limit, offset, search, statusId, launcherId, cancellationToken);

                var items = response.Results ?? [];
                var page = new PagedResult<Launch>
                {
                    Items = items,
                    TotalCount = response.Count,
                    Offset = offset,
                    Limit = limit,
                    HasNextPage = !string.IsNullOrEmpty(response.Next)
                        || offset + items.Count < response.Count
                };

                await _cache.SetPageAsync(cacheKey, page, cancellationToken);
                return page;
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                var cached = await _cache.GetPageAsync(cacheKey, cancellationToken);
                if (cached is not null)
                {
                    _logger.LogWarning(ex, "Page fetch failed; using cache {Key}", cacheKey);
                    return cached;
                }

                throw;
            }
        }

        private static string BuildCacheKey(
            string prefix, int limit, int offset, string? search, int? statusId, int? launcherId)
            => $"{prefix}|l={limit}|o={offset}|s={search}|st={statusId}|ln={launcherId}";

        private static bool IsNetworkOrCancel(Exception ex, CancellationToken ct)
            => ct.IsCancellationRequested
               || ex is HttpRequestException
               || ex is TaskCanceledException
               || ex is Refit.ApiException;
    }
}

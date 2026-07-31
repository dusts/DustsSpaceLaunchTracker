using DustsSpaceLaunchTracker.Configuration;
using System.Collections.Concurrent;
using System.Text;

namespace DustsSpaceLaunchTracker.Services.Diagnostics
{
    /// <summary>
    /// In-app diagnostics ring buffer for mobile troubleshooting.
    /// </summary>
    public sealed class DiagnosticsService : IDiagnosticsService
    {
        private const int MaxEntries = 200;
        private readonly ConcurrentQueue<DiagnosticEntry> _entries = new();
        private readonly object _gate = new();

        public IReadOnlyList<DiagnosticEntry> Entries
        {
            get
            {
                lock (_gate)
                    return _entries.ToArray();
            }
        }

        public string? LastErrorSummary { get; private set; }
        public string? LastErrorDetail { get; private set; }

        public event EventHandler? Changed;

        public void Info(string source, string message)
            => Add("INFO", source, message, null);

        public void Warning(string source, string message)
            => Add("WARN", source, message, null);

        public void Error(string source, string message, Exception? exception = null)
        {
            var detail = exception?.ToString();
            LastErrorSummary = message;
            LastErrorDetail = detail;
            Add("ERROR", source, message, detail);

            try
            {
                PersistCrashLog(source, message, detail);
            }
            catch
            {
                // never throw from diagnostics
            }
        }

        public void Clear()
        {
            lock (_gate)
            {
                while (_entries.TryDequeue(out _)) { }
            }

            LastErrorSummary = null;
            LastErrorDetail = null;
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public string GetEnvironmentSummary()
        {
            var sb = new StringBuilder();
            try
            {
                sb.AppendLine($"App: {AppInfo.Current.Name} {AppInfo.Current.VersionString} ({AppInfo.Current.BuildString})");
                sb.AppendLine($"Package: {AppInfo.Current.PackageName}");
                sb.AppendLine($"Platform: {DeviceInfo.Current.Platform} {DeviceInfo.Current.VersionString}");
                sb.AppendLine($"Device: {DeviceInfo.Current.Manufacturer} {DeviceInfo.Current.Model} ({DeviceInfo.Current.Idiom})");
                sb.AppendLine($"Network: {Connectivity.Current.NetworkAccess}");
                var profiles = string.Join(", ", Connectivity.Current.ConnectionProfiles);
                sb.AppendLine($"Profiles: {(string.IsNullOrWhiteSpace(profiles) ? "(none)" : profiles)}");
                sb.AppendLine($"API base: {AppConfig.ApiBaseUrl}");
                sb.AppendLine($"API version: {AppConfig.ApiVersion}");
                sb.AppendLine($"List mode: {AppConfig.ListMode}");
                sb.AppendLine($"Token configured: {!string.IsNullOrWhiteSpace(AppConfig.ApiToken)}");
#if DEBUG
                sb.AppendLine("Build: DEBUG");
#else
                sb.AppendLine("Build: RELEASE");
#endif
            }
            catch (Exception ex)
            {
                sb.AppendLine($"Environment summary failed: {ex.Message}");
            }

            return sb.ToString().TrimEnd();
        }

        public string BuildReport()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== DustsSpaceLaunchTracker diagnostics ===");
            sb.AppendLine(GetEnvironmentSummary());
            sb.AppendLine();
            sb.AppendLine("=== Log ===");
            foreach (var entry in Entries)
            {
                sb.AppendLine(entry.ToString());
                sb.AppendLine("---");
            }

            if (!string.IsNullOrEmpty(LastErrorDetail))
            {
                sb.AppendLine("=== Last error detail ===");
                sb.AppendLine(LastErrorDetail);
            }

            return sb.ToString();
        }

        private void Add(string level, string source, string message, string? detail)
        {
            var entry = new DiagnosticEntry
            {
                Timestamp = DateTimeOffset.Now,
                Level = level,
                Source = source,
                Message = message,
                Detail = detail
            };

            lock (_gate)
            {
                _entries.Enqueue(entry);
                while (_entries.Count > MaxEntries && _entries.TryDequeue(out _)) { }
            }

            System.Diagnostics.Debug.WriteLine(entry.ToString());
            Changed?.Invoke(this, EventArgs.Empty);
        }

        private static void PersistCrashLog(string source, string message, string? detail)
        {
            var dir = FileSystem.AppDataDirectory;
            var path = Path.Combine(dir, "diagnostics.log");
            var block = new StringBuilder()
                .AppendLine(DateTimeOffset.Now.ToString("O"))
                .AppendLine($"{source}: {message}")
                .AppendLine(detail ?? string.Empty)
                .AppendLine("---")
                .ToString();
            File.AppendAllText(path, block);
        }
    }
}

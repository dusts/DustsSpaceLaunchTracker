namespace DustsSpaceLaunchTracker.Services.Diagnostics
{
    public interface IDiagnosticsService
    {
        IReadOnlyList<DiagnosticEntry> Entries { get; }
        string? LastErrorSummary { get; }
        string? LastErrorDetail { get; }
        event EventHandler? Changed;

        void Info(string source, string message);
        void Warning(string source, string message);
        void Error(string source, string message, Exception? exception = null);
        void Clear();
        string BuildReport();
        string GetEnvironmentSummary();
    }

    public sealed class DiagnosticEntry
    {
        public required DateTimeOffset Timestamp { get; init; }
        public required string Level { get; init; }
        public required string Source { get; init; }
        public required string Message { get; init; }
        public string? Detail { get; init; }

        public override string ToString()
        {
            var line = $"[{Timestamp:HH:mm:ss}] {Level} {Source}: {Message}";
            return string.IsNullOrEmpty(Detail) ? line : $"{line}\n{Detail}";
        }
    }
}

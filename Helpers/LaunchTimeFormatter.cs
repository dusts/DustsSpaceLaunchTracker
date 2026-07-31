namespace DustsSpaceLaunchTracker.Helpers
{
    /// <summary>Local + UTC NET formatting and countdown text (points 10, 13).</summary>
    public static class LaunchTimeFormatter
    {
        public static string FormatLocal(DateTime? netUtc)
        {
            if (netUtc is null)
                return "NET: TBD";

            var local = ToLocal(netUtc.Value);
            return $"Local: {local:yyyy-MM-dd HH:mm} ({TimeZoneInfo.Local.Id})";
        }

        public static string FormatUtc(DateTime? netUtc)
        {
            if (netUtc is null)
                return "UTC: TBD";

            var utc = DateTime.SpecifyKind(netUtc.Value, DateTimeKind.Utc);
            return $"UTC: {utc:yyyy-MM-dd HH:mm}";
        }

        public static string FormatCountdown(DateTime? netUtc, DateTime utcNow)
        {
            if (netUtc is null)
                return "T‑ TBD";

            var net = DateTime.SpecifyKind(netUtc.Value, DateTimeKind.Utc);
            var delta = net - utcNow;

            if (delta.TotalSeconds >= 0)
            {
                if (delta.TotalDays >= 1)
                    return $"T‑ {delta.Days}d {delta.Hours:D2}h {delta.Minutes:D2}m";
                if (delta.TotalHours >= 1)
                    return $"T‑ {delta.Hours:D2}h {delta.Minutes:D2}m {delta.Seconds:D2}s";
                return $"T‑ {delta.Minutes:D2}m {delta.Seconds:D2}s";
            }

            var ago = utcNow - net;
            if (ago.TotalDays >= 1)
                return $"T+ {ago.Days}d {ago.Hours:D2}h";
            if (ago.TotalHours >= 1)
                return $"T+ {ago.Hours:D2}h {ago.Minutes:D2}m";
            return $"T+ {ago.Minutes:D2}m {ago.Seconds:D2}s";
        }

        public static DateTime ToLocal(DateTime net)
        {
            var utc = net.Kind switch
            {
                DateTimeKind.Utc => net,
                DateTimeKind.Local => net.ToUniversalTime(),
                _ => DateTime.SpecifyKind(net, DateTimeKind.Utc)
            };
            return utc.ToLocalTime();
        }
    }
}

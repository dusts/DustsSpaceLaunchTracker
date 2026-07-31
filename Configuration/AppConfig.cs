namespace DustsSpaceLaunchTracker.Configuration
{
    /// <summary>
    /// Central app/API configuration (point 16). Prefer env vars for secrets.
    /// </summary>
    public static class AppConfig
    {
        public const string ApiVersion = "2.2.0";

        /// <summary>Production Launch Library host.</summary>
        public const string ProductionApiHost = "https://ll.thespacedevs.com/";

        /// <summary>Dev host with higher free-tier rate limits (point 6).</summary>
        public const string DevelopmentApiHost = "https://lldev.thespacedevs.com/";

        /// <summary>
        /// Base URL used by HttpClient (host only; version is in Refit paths).
        /// Debug builds use lldev to reduce rate-limit pain; Release uses production.
        /// Override with env DUSTS_LL_API_BASE.
        /// </summary>
        public static string ApiBaseUrl
        {
            get
            {
                var fromEnv = Environment.GetEnvironmentVariable("DUSTS_LL_API_BASE");
                if (!string.IsNullOrWhiteSpace(fromEnv))
                    return fromEnv.EndsWith('/') ? fromEnv : fromEnv + "/";

#if DEBUG
                return DevelopmentApiHost;
#else
                return ProductionApiHost;
#endif
            }
        }

        /// <summary>
        /// Optional The Space Devs API token (Authorization: Token …).
        /// Set env DUSTS_LL_API_TOKEN or leave null for anonymous.
        /// </summary>
        public static string? ApiToken =>
            Environment.GetEnvironmentVariable("DUSTS_LL_API_TOKEN");

        /// <summary>Lighter payload for list screens (point 2).</summary>
        public const string ListMode = "normal";

        /// <summary>Full payload for detail (point 2 / 4).</summary>
        public const string DetailMode = "detailed";

        public const int PageSize = 15;

        /// <summary>How long cached list pages stay fresh.</summary>
        public static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(15);

        /// <summary>Min delay between load-more requests (point 6).</summary>
        public static readonly TimeSpan LoadMoreMinInterval = TimeSpan.FromMilliseconds(500);

        public const string UpcomingCachePrefix = "upcoming";
        public const string PreviousCachePrefix = "previous";
        public const string DetailCachePrefix = "detail";
    }
}

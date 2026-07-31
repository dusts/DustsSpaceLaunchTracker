namespace DustsSpaceLaunchTracker.Configuration
{
    /// <summary>Compile-time route constants for Refit (version segment included).</summary>
    public static class ApiRoutes
    {
        public const string UpcomingLaunches = "/" + AppConfig.ApiVersion + "/launch/upcoming/";
        public const string PreviousLaunches = "/" + AppConfig.ApiVersion + "/launch/previous/";
        public const string LaunchDetail = "/" + AppConfig.ApiVersion + "/launch/{id}/";
        public const string Launchers = "/" + AppConfig.ApiVersion + "/config/launcher/";
        public const string LauncherDetail = "/" + AppConfig.ApiVersion + "/config/launcher/{id}/";
        public const string Agencies = "/" + AppConfig.ApiVersion + "/agency/";
        public const string AgencyDetail = "/" + AppConfig.ApiVersion + "/agency/{id}/";
        public const string Pads = "/" + AppConfig.ApiVersion + "/pad/";
        public const string PadDetail = "/" + AppConfig.ApiVersion + "/pad/{id}/";
        public const string LaunchStatuses = "/" + AppConfig.ApiVersion + "/launchstatus/";
        public const string UpcomingEvents = "/" + AppConfig.ApiVersion + "/event/upcoming/";
    }
}

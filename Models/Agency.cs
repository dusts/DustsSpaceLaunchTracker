namespace DustsSpaceLaunchTracker.Models
{
    /// <summary>
    /// Represents an agency/organization/launch service provider in TheSpaceDevs API.
    /// Reused in multiple places: launch_service_provider, mission.agencies[], rocket manufacturer, etc.
    /// </summary>
    public class Agency
    {
        public int Id { get; set; }

        /// <summary>
        /// Full name (e.g. "Space Exploration Technologies Corp.")
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Short/abbreviated name (e.g. "SpaceX")
        /// </summary>
        public string? Abbrev { get; set; }

        /// <summary>
        /// Type of entity (e.g. "Commercial", "Government", "Multinational", "Military")
        /// </summary>
        public string? Type { get; set; }

        public string? CountryCode { get; set; }          // ISO 3166-1 alpha-3, e.g. "USA", "CHN", "RUS"

        /// <summary>
        /// Founding year or null
        /// </summary>
        public int? FoundingYear { get; set; }

        /// <summary>
        /// Main logo URL (often white-on-transparent PNG/SVG)
        /// </summary>
        public string? LogoUrl { get; set; }

        /// <summary>
        /// Larger/banner/image URL if available
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// URL to the agency's page on thespacedevs.com
        /// </summary>
        public string? InfoUrl { get; set; }

        /// <summary>
        /// Wikipedia page or official site if provided
        /// </summary>
        public string? WikiUrl { get; set; }

        /// <summary>
        /// Optional short description/bio
        /// </summary>
        public string? Description { get; set; }

        // Less commonly used in card UI but good to have:
        public string? Administrator { get; set; }       // e.g. "Elon Musk" for SpaceX (sometimes present)
        public bool Launchers { get; set; }               // true if they develop/operate launch vehicles
        public bool Spacecraft { get; set; }              // true if they build spacecraft
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DustsSpaceLaunchTracker.Models
{
    public class RocketConfiguration
    {
        /// <summary>
        /// Unique ID of this launcher configuration
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Short/variant name (e.g. "Falcon 9 Block 5", "Electron")
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Rocket family (e.g. "Falcon 9", "Electron", "Angara")
        /// </summary>
        public string? Family { get; set; }

        /// <summary>
        /// Full descriptive name (often same as Name)
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Specific variant identifier (e.g. "Block 5", "230+")
        /// Can be empty
        /// </summary>
        public string? Variant { get; set; }

        /// <summary>
        /// Manufacturer / builder agency
        /// </summary>
        public Agency? Manufacturer { get; set; }

        /// <summary>
        /// Whether this configuration is reusable (e.g. Falcon 9 boosters)
        /// </summary>
        public bool Reusable { get; set; }

        /// <summary>
        /// Main image URL for the rocket variant (good for cards/logos)
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Description / overview text
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// External info link (company page, etc.)
        /// </summary>
        public string? InfoUrl { get; set; }

        /// <summary>
        /// Wikipedia page URL
        /// </summary>
        public string? WikiUrl { get; set; }

        // ───────────────────────────────────────────────────────────────
        // Launch statistics – very useful for badges / filters
        // ───────────────────────────────────────────────────────────────

        public int TotalLaunchCount { get; set; }
        public int SuccessfulLaunches { get; set; }
        public int ConsecutiveSuccessfulLaunches { get; set; }
        public int PendingLaunches { get; set; }

        // Optional capacity / performance fields (sometimes present)
        public double? LeoCapacity { get; set; }      // kg to LEO
        public double? GeoCapacity { get; set; }      // kg to GTO/GEO
        public double? ThrustAtSeaLevel { get; set; }
        public double? ThrustVacuum { get; set; }

        // Optional: associated programs (e.g. Commercial Crew, Starlink)
        public List<Program>? Programs { get; set; } = new();
    }
}

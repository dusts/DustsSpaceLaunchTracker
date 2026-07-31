using System.Text.Json.Serialization;

namespace DustsSpaceLaunchTracker.Models
{
    public class RocketConfiguration
    {
        public int Id { get; set; }

        /// <summary>
        /// Short/variant name (e.g. "Falcon 9", "Electron")
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Rocket family (e.g. "Falcon", "Electron", "Angara")
        /// </summary>
        public string? Family { get; set; }

        /// <summary>
        /// Full descriptive name (e.g. "Falcon 9 Block 5")
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Specific variant identifier (e.g. "Block 5")
        /// </summary>
        public string? Variant { get; set; }

        public Agency? Manufacturer { get; set; }

        public bool Reusable { get; set; }

        public string? ImageUrl { get; set; }

        public string? Description { get; set; }

        public string? InfoUrl { get; set; }

        public string? WikiUrl { get; set; }

        public int TotalLaunchCount { get; set; }
        public int SuccessfulLaunches { get; set; }
        public int ConsecutiveSuccessfulLaunches { get; set; }
        public int PendingLaunches { get; set; }

        public double? LeoCapacity { get; set; }

        // API field is gto_capacity, not geo_capacity
        [JsonPropertyName("gto_capacity")]
        public double? GtoCapacity { get; set; }

        // API field is to_thrust (sea-level thrust)
        [JsonPropertyName("to_thrust")]
        public double? ThrustAtSeaLevel { get; set; }

        // API key is "program" (singular)
        [JsonPropertyName("program")]
        public List<Program>? Programs { get; set; }
    }
}

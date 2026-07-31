using System.Text.Json.Serialization;

namespace DustsSpaceLaunchTracker.Models
{
    /// <summary>
    /// Represents a single launch (detailed mode)
    /// </summary>
    public class Launch
    {
        public string Id { get; set; } = string.Empty;              // UUID
        public string Name { get; set; } = string.Empty;            // e.g. "Falcon 9 Block 5 | Starlink Group 12-3"
        public string? Slug { get; set; }

        public LaunchStatus? Status { get; set; }

        public DateTime? Net { get; set; }                          // No Earlier Than – main launch time
        public DateTime? WindowStart { get; set; }
        public DateTime? WindowEnd { get; set; }

        public int? NetPrecisionId { get; set; }
        public LaunchNetPrecision? NetPrecision { get; set; }

        public int? Probability { get; set; }                       // 0–100 or null

        // API uses no underscore: holdreason / failreason
        [JsonPropertyName("holdreason")]
        public string? HoldReason { get; set; }

        [JsonPropertyName("failreason")]
        public string? FailReason { get; set; }

        public string? WeatherConcerns { get; set; }
        public Agency? LaunchServiceProvider { get; set; }

        public Rocket? Rocket { get; set; }

        public Mission? Mission { get; set; }

        public Pad? Pad { get; set; }

        public string? Image { get; set; }                          // Main launch image URL
        public string? Infographic { get; set; }

        // API keys are infoURLs / vidURLs (mixed case), not snake_case
        [JsonPropertyName("infoURLs")]
        public List<MediaUrl> InfoUrls { get; set; } = new();

        [JsonPropertyName("vidURLs")]
        public List<MediaUrl> VidUrls { get; set; } = new();

        public bool WebcastLive { get; set; }

        public List<Update>? Updates { get; set; }

        // API key is "program" (singular)
        [JsonPropertyName("program")]
        public List<Program>? Programs { get; set; }

        // API key is singular "hashtag"
        [JsonPropertyName("hashtag")]
        public string? Hashtag { get; set; }

        // API returns null for some vehicles (e.g. Starship suborbital / early flights)
        public int? OrbitalLaunchAttemptCount { get; set; }
        public int? LocationLaunchAttemptCount { get; set; }
    }
}


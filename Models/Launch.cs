using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

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

        public int? NetPrecisionId { get; set; }                    // Optional: links to precision table
        public LaunchNetPrecision? NetPrecision { get; set; }       // Detailed in detailed mode

        public int? Probability { get; set; }                       // 0–100 or null
        public string? HoldReason { get; set; }
        public string? FailReason { get; set; }
        public string? WeatherConcerns { get; set; }
        public Agency? LaunchServiceProvider { get; set; }

        public Rocket? Rocket { get; set; }

        public Mission? Mission { get; set; }

        public Pad? Pad { get; set; }

        public string? Image { get; set; }                          // Main launch image URL
        public string? Infographic { get; set; }                    // Optional infographic URL

        public List<string> InfoUrls { get; set; } = new();         // renamed from infoURLs
        public List<string> VidUrls { get; set; } = new();          // renamed from vidURLs
        public bool WebcastLive { get; set; }

        // Optional: for later features
        public List<Update>? Updates { get; set; }
        public List<Program>? Programs { get; set; }                // array of programs
        public List<string>? Hashtags { get; set; }                 // sometimes array

        // Attempt counters (useful for badges / stats)
        public int OrbitalLaunchAttemptCount { get; set; }
        public int LocationLaunchAttemptCount { get; set; }
    }
}

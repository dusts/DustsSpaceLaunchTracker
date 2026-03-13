using System;
using System.Collections.Generic;
using System.Text;

namespace DustsSpaceLaunchTracker.Models
{
    /// <summary>
    /// Represents a non-launch event from the Launch Library API (e.g. static fire, rollout, docking, announcement, etc.)
    /// </summary>
    public class Event
    {
        /// <summary>
        /// Unique numeric ID of the event
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Human-readable title / name of the event
        /// Example: "Falcon 9 Static Fire Test – Starlink Group 12-5"
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Short summary / description (usually 1–3 sentences)
        /// </summary>
        public string? Summary { get; set; }

        /// <summary>
        /// Type/category of the event
        /// Examples: "Static Fire", "Rollout", "Docking", "Announcement", "Spacewalk", "Engine Test", ...
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Main date/time of the event (often approximate)
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Window start (if a time window exists)
        /// </summary>
        public DateTime? WindowStart { get; set; }

        /// <summary>
        /// Window end (if applicable)
        /// </summary>
        public DateTime? WindowEnd { get; set; }

        /// <summary>
        /// Main feature image URL (good for card hero/background)
        /// </summary>
        public string? FeatureImage { get; set; }

        /// <summary>
        /// Additional image URLs (gallery)
        /// </summary>
        public List<string> ImageUrls { get; set; } = new();

        /// <summary>
        /// Related news article or source URL
        /// </summary>
        public string? NewsUrl { get; set; }

        /// <summary>
        /// Video stream / replay URL (YouTube, etc.)
        /// </summary>
        public string? VideoUrl { get; set; }

        /// <summary>
        /// Whether a live webcast is expected / ongoing
        /// </summary>
        public bool WebcastLive { get; set; }

        /// <summary>
        /// Agency / organization that published or is responsible for the event
        /// </summary>
        public Agency? InfoSource { get; set; }

        /// <summary>
        /// Optional link to a related launch (many events are pre/post-launch related)
        /// </summary>
        public Launch? Launch { get; set; }

        /// <summary>
        /// Sometimes contains extra metadata / hashtags / links
        /// </summary>
        public Dictionary<string, string>? Extra { get; set; }
    }
}

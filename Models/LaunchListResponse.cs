using System;
using System.Collections.Generic;
using System.Text;

namespace DustsSpaceLaunchTracker.Models
{
    /// <summary>
    /// Root response for /launch/upcoming/, /launch/previous/, etc.
    /// </summary>
    public class LaunchListResponse
    {
        public int Count { get; set; }
        public string? Next { get; set; }     // URL or null
        public string? Previous { get; set; } // URL or null
        public List<Launch> Results { get; set; } = new();
    }
}

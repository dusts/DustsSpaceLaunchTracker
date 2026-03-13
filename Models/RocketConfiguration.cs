using System;
using System.Collections.Generic;
using System.Text;

namespace DustsSpaceLaunchTracker.Models
{
    public class RocketConfiguration
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;        // "Falcon 9 Block 5", "Electron", ...
        public string? Family { get; set; }
        public string? FullName { get; set; }
        public string? Variant { get; set; }
        public Agency? Manufacturer { get; set; }   // ← still called Manufacturer here to match domain & API naming
        public string? ImageUrl { get; set; }
        public bool Reusable { get; set; }
        public string? Description { get; set; }
    }
}

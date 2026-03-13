using System;
using System.Collections.Generic;
using System.Text;

namespace DustsSpaceLaunchTracker.Models
{
    public class Orbit
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;        // "Low Earth Orbit", "Geostationary Transfer Orbit", ...
        public string Abbrev { get; set; } = string.Empty;      // "LEO", "GTO", ...
    }
}

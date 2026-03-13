using System;
using System.Collections.Generic;
using System.Text;

namespace DustsSpaceLaunchTracker.Models
{
    public class LaunchNetPrecision
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;        // "Minute", "Hour", "Day", "Month"...
        public string Abbrev { get; set; } = string.Empty;
    }
}

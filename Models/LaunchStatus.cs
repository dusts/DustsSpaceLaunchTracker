using System;
using System.Collections.Generic;
using System.Text;

namespace DustsSpaceLaunchTracker.Models
{
    public class LaunchStatus
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;        // "Launch Successful", "Go for Launch", "TBC"...
        public string Abbrev { get; set; } = string.Empty;      // "Success", "Go", "TBC"...
        public string? Description { get; set; }
    }
}

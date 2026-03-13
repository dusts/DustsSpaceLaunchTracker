using System;
using System.Collections.Generic;
using System.Text;

namespace DustsSpaceLaunchTracker.Models
{
    public class Rocket
    {
        public RocketConfiguration? Configuration { get; set; }
        // launcher_stage / spacecraft_stage – usually only needed for very detailed views
    }
}

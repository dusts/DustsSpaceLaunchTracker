namespace DustsSpaceLaunchTracker.Models
{
    public class Mission
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }                       // "Communications", "Dedicated Rideshare", ...
        public Orbit? Orbit { get; set; }

        /// <summary>
        /// Agencies involved (customers, payload owners, etc.)
        /// Usually NASA/ESA for science, DoD for classified, or commercial entities
        /// </summary>
        public List<Agency>? Agencies { get; set; }
    }
}

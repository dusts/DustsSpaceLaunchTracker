namespace DustsSpaceLaunchTracker.Models
{
    public class PadLocation
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;        // "Vandenberg SFB, CA, USA", ...
        public string? CountryCode { get; set; }
        public string? Description { get; set; }
        public string? MapImage { get; set; }
        public string? TimezoneName { get; set; }
    }
}

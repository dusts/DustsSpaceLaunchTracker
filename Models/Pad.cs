namespace DustsSpaceLaunchTracker.Models
{
    public class Pad
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;        // "SLC-40", "Launch Complex 39A", ...
        public PadLocation? Location { get; set; }
        public string? MapImage { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}

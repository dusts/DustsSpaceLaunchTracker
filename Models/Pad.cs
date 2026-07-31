namespace DustsSpaceLaunchTracker.Models
{
    public class Pad
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;        // "SLC-40", "Launch Complex 39A", ...
        public string? Description { get; set; }
        public PadLocation? Location { get; set; }
        public string? MapImage { get; set; }
        public string? MapUrl { get; set; }
        public string? WikiUrl { get; set; }
        public string? InfoUrl { get; set; }
        public string? CountryCode { get; set; }

        // API returns these as strings; NumberHandling.AllowReadingFromString handles conversion
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}

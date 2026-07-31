namespace DustsSpaceLaunchTracker.Models
{
    /// <summary>
    /// infoURLs / vidURLs entry from The Space Devs API.
    /// </summary>
    public class MediaUrl
    {
        public string? Url { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Source { get; set; }
        public string? FeatureImage { get; set; }
        public string? Publisher { get; set; }
    }
}

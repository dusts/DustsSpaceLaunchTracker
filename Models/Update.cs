namespace DustsSpaceLaunchTracker.Models
{
    public class Update   // for recent changes / announcements
    {
        public int Id { get; set; }
        public string? Comment { get; set; }
        public string? ProfileImage { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}

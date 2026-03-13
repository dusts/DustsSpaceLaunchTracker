using DustsSpaceLaunchTracker.Models;
using Refit;

namespace DustsSpaceLaunchTracker.Services.Api
{
    public interface ITheSpaceDevsApi
    {
        [Get("/launch/upcoming/?limit={limit}&offset={offset}&mode=detailed&search={search}")]
        Task<LaunchListResponse> GetUpcomingLaunchesAsync(
            int limit = 20,
            int offset = 0,
            string? search = null);

        [Get("/launch/previous/?limit={limit}&offset={offset}&mode=detailed")]
        Task<LaunchListResponse> GetPreviousLaunchesAsync(
            int limit = 20,
            int offset = 0);
    }

    public class LaunchListResponse
    {
        public int Count { get; set; }
        public string? Next { get; set; }
        public string? Previous { get; set; }
        public List<Launch> Results { get; set; } = new();
    }
}

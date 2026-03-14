using DustsSpaceLaunchTracker.Models;
using DustsSpaceLaunchTracker.Services.Api;

namespace DustsSpaceLaunchTracker.Services
{
    public class LaunchService
    {
        private readonly ITheSpaceDevsApi _api;

        public LaunchService(ITheSpaceDevsApi api)
        {
            _api = api;
        }

        public async Task<List<Launch>> GetUpcomingAsync(int limit = 20, int offset = 0, string? search = null)
        {
            var response = await _api.GetUpcomingLaunchesAsync(limit, offset, search);
            return response.Results;  // Or process further (e.g., sort/filter)
        }

        // Add similar for GetPreviousAsync, GetByIdAsync
        // Later: Integrate NASA/SpaceX, e.g., public async Task<NasaData> GetNasaExtraAsync() { ... }
    }
}

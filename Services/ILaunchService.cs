using DustsSpaceLaunchTracker.Models;
using DustsSpaceLaunchTracker.Models.Responses;

namespace DustsSpaceLaunchTracker.Services
{
    /// <summary>App-facing launch data API (point 7).</summary>
    public interface ILaunchService
    {
        Task<PagedResult<Launch>> GetUpcomingPageAsync(
            int limit,
            int offset,
            string? search = null,
            int? statusId = null,
            int? launcherId = null,
            bool forceRefresh = false,
            CancellationToken cancellationToken = default);

        Task<PagedResult<Launch>> GetPreviousPageAsync(
            int limit,
            int offset,
            string? search = null,
            int? statusId = null,
            int? launcherId = null,
            bool forceRefresh = false,
            CancellationToken cancellationToken = default);

        Task<Launch> GetLaunchDetailAsync(
            string id,
            bool forceRefresh = false,
            CancellationToken cancellationToken = default);
    }
}

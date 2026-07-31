using DustsSpaceLaunchTracker.Models;
using DustsSpaceLaunchTracker.Models.Responses;

namespace DustsSpaceLaunchTracker.Services.Data
{
    /// <summary>Offline / stale-while-revalidate cache (point 5).</summary>
    public interface ILaunchCache
    {
        Task<PagedResult<Launch>?> GetPageAsync(string key, CancellationToken cancellationToken = default);
        Task SetPageAsync(string key, PagedResult<Launch> page, CancellationToken cancellationToken = default);
        Task<Launch?> GetDetailAsync(string launchId, CancellationToken cancellationToken = default);
        Task SetDetailAsync(Launch launch, CancellationToken cancellationToken = default);
        Task ClearAsync(CancellationToken cancellationToken = default);
    }
}

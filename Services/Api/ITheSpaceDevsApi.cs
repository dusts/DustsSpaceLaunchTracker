using DustsSpaceLaunchTracker.Configuration;
using DustsSpaceLaunchTracker.Models;
using DustsSpaceLaunchTracker.Models.Responses;
using Refit;

namespace DustsSpaceLaunchTracker.Services.Api
{
    /// <summary>
    /// Paths start with '/' (Refit requirement). Host is AppConfig.ApiBaseUrl;
    /// version lives in ApiRoutes so BaseAddress + path does not drop /2.2.0/.
    /// CancellationToken is supported as a trailing parameter by Refit (point 3).
    /// </summary>
    public interface ITheSpaceDevsApi
    {
        [Get(ApiRoutes.UpcomingLaunches)]
        Task<LaunchListResponse> GetUpcomingLaunchesAsync(
            int limit = 20,
            int offset = 0,
            string mode = AppConfig.ListMode,
            string? search = null,
            int? status = null,
            int? launcher = null,
            CancellationToken cancellationToken = default);

        [Get(ApiRoutes.PreviousLaunches)]
        Task<LaunchListResponse> GetPreviousLaunchesAsync(
            int limit = 20,
            int offset = 0,
            string mode = AppConfig.ListMode,
            string? search = null,
            int? status = null,
            int? launcher = null,
            CancellationToken cancellationToken = default);

        [Get(ApiRoutes.LaunchDetail)]
        Task<Launch> GetLaunchDetailAsync(
            string id,
            string mode = AppConfig.DetailMode,
            CancellationToken cancellationToken = default);

        [Get(ApiRoutes.Launchers)]
        Task<LauncherConfigListResponse> GetLaunchersAsync(
            int limit = 50,
            int offset = 0,
            string mode = AppConfig.ListMode,
            string? search = null,
            CancellationToken cancellationToken = default);

        [Get(ApiRoutes.LauncherDetail)]
        Task<RocketConfiguration> GetLauncherDetailAsync(
            int id,
            string mode = AppConfig.DetailMode,
            CancellationToken cancellationToken = default);

        [Get(ApiRoutes.Agencies)]
        Task<AgencyListResponse> GetAgenciesAsync(
            int limit = 50,
            int offset = 0,
            string? search = null,
            CancellationToken cancellationToken = default);

        [Get(ApiRoutes.AgencyDetail)]
        Task<Agency> GetAgencyDetailAsync(int id, CancellationToken cancellationToken = default);

        [Get(ApiRoutes.Pads)]
        Task<PadListResponse> GetLaunchPadsAsync(
            int limit = 50,
            int offset = 0,
            string mode = AppConfig.ListMode,
            string? search = null,
            CancellationToken cancellationToken = default);

        [Get(ApiRoutes.PadDetail)]
        Task<Pad> GetLaunchPadDetailAsync(
            int id,
            string mode = AppConfig.DetailMode,
            CancellationToken cancellationToken = default);

        [Get(ApiRoutes.LaunchStatuses)]
        Task<LaunchStatusListResponse> GetLaunchStatusesAsync(
            int limit = 30,
            CancellationToken cancellationToken = default);

        [Get(ApiRoutes.UpcomingEvents)]
        Task<EventListResponse> GetUpcomingEventsAsync(
            int limit = 10,
            int offset = 0,
            string mode = AppConfig.ListMode,
            string? search = null,
            CancellationToken cancellationToken = default);
    }
}

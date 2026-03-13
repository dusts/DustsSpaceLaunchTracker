using DustsSpaceLaunchTracker.Models;
using DustsSpaceLaunchTracker.Models.Responses;
using Refit;

namespace DustsSpaceLaunchTracker.Services.Api
{
    public interface ITheSpaceDevsApi
    {
        // ───────────────────────────────────────────────────────────────
        //  Launches – main endpoints for the tracker
        // ───────────────────────────────────────────────────────────────

        /// <summary>
        /// Upcoming launches (most important for the main screen)
        /// </summary>
        [Get("/launch/upcoming/?limit={limit}&offset={offset}&mode=detailed&search={search}&status={status}&launcher={launcher}")]
        Task<LaunchListResponse> GetUpcomingLaunchesAsync(
            int limit = 20,
            int offset = 0,
            string? search = null,
            int? status = null,
            int? launcher = null);

        /// <summary>
        /// Previous / historical launches
        /// </summary>
        [Get("/launch/previous/?limit={limit}&offset={offset}&mode=detailed&search={search}&status={status}&launcher={launcher}")]
        Task<LaunchListResponse> GetPreviousLaunchesAsync(
            int limit = 20,
            int offset = 0,
            string? search = null,
            int? status = null,
            int? launcher = null);

        /// <summary>
        /// Single launch detail (for detail page)
        /// </summary>
        [Get("/launch/{id}/?mode=detailed")]
        Task<Launch> GetLaunchDetailAsync(string id);

        // ───────────────────────────────────────────────────────────────
        //  Rocket / Launcher configurations
        // ───────────────────────────────────────────────────────────────

        [Get("/config/launcher/?limit={limit}&offset={offset}&mode=detailed&search={search}")]
        Task<LauncherConfigListResponse> GetLaunchersAsync(
            int limit = 50,
            int offset = 0,
            string? search = null);

        [Get("/config/launcher/{id}/?mode=detailed")]
        Task<RocketConfiguration> GetLauncherDetailAsync(int id);

        // ───────────────────────────────────────────────────────────────
        //  Agencies / Launch Service Providers
        // ───────────────────────────────────────────────────────────────

        [Get("/agency/?limit={limit}&offset={offset}&search={search}")]
        Task<AgencyListResponse> GetAgenciesAsync(
            int limit = 50,
            int offset = 0,
            string? search = null);

        [Get("/agency/{id}/")]
        Task<Agency> GetAgencyDetailAsync(int id);

        // ───────────────────────────────────────────────────────────────
        //  Launch Pads / Sites
        // ───────────────────────────────────────────────────────────────

        [Get("/pad/?limit={limit}&offset={offset}&mode=detailed&search={search}")]
        Task<PadListResponse> GetLaunchPadsAsync(
            int limit = 50,
            int offset = 0,
            string? search = null);

        [Get("/pad/{id}/?mode=detailed")]
        Task<Pad> GetLaunchPadDetailAsync(int id);

        // ───────────────────────────────────────────────────────────────
        //  Status codes (useful for filters, colors, badges)
        // ───────────────────────────────────────────────────────────────

        [Get("/launchstatus/?limit={limit}")]
        Task<LaunchStatusListResponse> GetLaunchStatusesAsync(int limit = 30);

        // ───────────────────────────────────────────────────────────────
        //  Upcoming non-launch events (static fires, dockings, announcements, etc.)
        // ───────────────────────────────────────────────────────────────

        [Get("/event/upcoming/?limit={limit}&offset={offset}&mode=detailed&search={search}")]
        Task<EventListResponse> GetUpcomingEventsAsync(
            int limit = 10,
            int offset = 0,
            string? search = null);
    }
}

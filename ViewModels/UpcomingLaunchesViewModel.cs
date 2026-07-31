using DustsSpaceLaunchTracker.Services;
using DustsSpaceLaunchTracker.Services.Diagnostics;

namespace DustsSpaceLaunchTracker.ViewModels
{
    public sealed partial class UpcomingLaunchesViewModel : LaunchListViewModelBase
    {
        public UpcomingLaunchesViewModel(
            ILaunchService launchService,
            IDiagnosticsService diagnostics)
            : base(
                launchService,
                diagnostics,
                title: "Upcoming Launches",
                pageFetcher: (svc, limit, offset, search, status, launcher, force, ct) =>
                    svc.GetUpcomingPageAsync(limit, offset, search, status, launcher, force, ct))
        {
        }
    }
}

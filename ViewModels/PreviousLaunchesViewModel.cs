using DustsSpaceLaunchTracker.Services;
using DustsSpaceLaunchTracker.Services.Diagnostics;

namespace DustsSpaceLaunchTracker.ViewModels
{
    public sealed partial class PreviousLaunchesViewModel : LaunchListViewModelBase
    {
        public PreviousLaunchesViewModel(
            ILaunchService launchService,
            IDiagnosticsService diagnostics)
            : base(
                launchService,
                diagnostics,
                title: "Previous Launches",
                pageFetcher: (svc, limit, offset, search, status, launcher, force, ct) =>
                    svc.GetPreviousPageAsync(limit, offset, search, status, launcher, force, ct))
        {
        }
    }
}

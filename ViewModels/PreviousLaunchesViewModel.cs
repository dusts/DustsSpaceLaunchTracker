using DustsSpaceLaunchTracker.Services;

namespace DustsSpaceLaunchTracker.ViewModels
{
    public sealed partial class PreviousLaunchesViewModel : LaunchListViewModelBase
    {
        public PreviousLaunchesViewModel(ILaunchService launchService)
            : base(
                launchService,
                title: "Previous Launches",
                pageFetcher: (svc, limit, offset, search, status, launcher, force, ct) =>
                    svc.GetPreviousPageAsync(limit, offset, search, status, launcher, force, ct))
        {
        }
    }
}

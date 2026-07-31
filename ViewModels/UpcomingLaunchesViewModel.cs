using DustsSpaceLaunchTracker.Services;

namespace DustsSpaceLaunchTracker.ViewModels
{
    public sealed partial class UpcomingLaunchesViewModel : LaunchListViewModelBase
    {
        public UpcomingLaunchesViewModel(ILaunchService launchService)
            : base(
                launchService,
                title: "Upcoming Launches",
                pageFetcher: (svc, limit, offset, search, status, launcher, force, ct) =>
                    svc.GetUpcomingPageAsync(limit, offset, search, status, launcher, force, ct))
        {
        }
    }
}

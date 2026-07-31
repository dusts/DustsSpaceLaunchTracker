using DustsSpaceLaunchTracker.Views;

namespace DustsSpaceLaunchTracker
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(LaunchDetailPage), typeof(LaunchDetailPage));
        }
    }
}

namespace DustsSpaceLaunchTracker
{
    public partial class App : Application
    {
        private readonly IServiceProvider _services;

        public App(IServiceProvider services)
        {
            InitializeComponent();
            _services = services;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Point 1 / 14: resolve Shell from DI (pages use ctor injection via Shell templates)
            return new Window(_services.GetRequiredService<AppShell>());
        }
    }
}

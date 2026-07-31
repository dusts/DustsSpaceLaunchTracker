using DustsSpaceLaunchTracker.Services.Diagnostics;

namespace DustsSpaceLaunchTracker
{
    public partial class App : Application
    {
        private readonly IServiceProvider _services;

        public App(IServiceProvider services)
        {
            InitializeComponent();
            _services = services;
            HookGlobalExceptionHandlers();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var diagnostics = _services.GetService<IDiagnosticsService>();
            diagnostics?.Info(nameof(App), "Application window created");
            return new Window(_services.GetRequiredService<AppShell>());
        }

        private void HookGlobalExceptionHandlers()
        {
            var diagnostics = _services.GetService<IDiagnosticsService>();
            if (diagnostics is null)
                return;

            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                diagnostics.Error(
                    "AppDomain",
                    ex?.Message ?? "Unhandled AppDomain exception",
                    ex);
            };

            TaskScheduler.UnobservedTaskException += (_, e) =>
            {
                diagnostics.Error("TaskScheduler", e.Exception.Message, e.Exception);
                e.SetObserved();
            };

#if ANDROID || IOS || MACCATALYST || WINDOWS
            // MAUI lifecycle / binding failures often surface here on mobile
#endif
        }
    }
}

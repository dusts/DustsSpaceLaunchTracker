using Microsoft.UI.Xaml;
using System.Text;

namespace DustsSpaceLaunchTracker.WinUI
{
    public partial class App : MauiWinUIApplication
    {
        public App()
        {
            this.InitializeComponent();

#if DEBUG
            // Point 15: crash logging only in DEBUG
            UnhandledException += (_, e) =>
            {
                try
                {
                    var path = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "DustsSpaceLaunchTracker-crash.log");

                    var sb = new StringBuilder();
                    sb.AppendLine(DateTime.Now.ToString("O"));
                    sb.AppendLine(e.Exception?.ToString() ?? "(null exception)");
                    sb.AppendLine("---");
                    File.AppendAllText(path, sb.ToString());
                }
                catch
                {
                    // ignore logging failures
                }

                e.Handled = true;
            };
#endif
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}

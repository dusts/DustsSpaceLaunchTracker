using CommunityToolkit.Mvvm.Input;
using DustsSpaceLaunchTracker.Services.Diagnostics;
using System.Collections.ObjectModel;
using System.Text;

namespace DustsSpaceLaunchTracker.ViewModels
{
    public sealed partial class DiagnosticsViewModel : ViewModelBase
    {
        private readonly IDiagnosticsService _diagnostics;
        private string _environmentSummary = string.Empty;
        private string _logText = string.Empty;
        private string? _statusMessage;
        private bool _isBusy;

        public DiagnosticsViewModel(IDiagnosticsService diagnostics)
        {
            _diagnostics = diagnostics;
            _diagnostics.Changed += (_, _) => MainThread.BeginInvokeOnMainThread(Refresh);
            Refresh();
        }

        public string EnvironmentSummary
        {
            get => _environmentSummary;
            private set => SetProperty(ref _environmentSummary, value);
        }

        public string LogText
        {
            get => _logText;
            private set => SetProperty(ref _logText, value);
        }

        public string? StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            private set => SetProperty(ref _isBusy, value);
        }

        public ObservableCollection<string> Entries { get; } = new();

        public void OnAppearing() => Refresh();

        [RelayCommand]
        private void Refresh()
        {
            EnvironmentSummary = _diagnostics.GetEnvironmentSummary();
            Entries.Clear();
            var sb = new StringBuilder();
            foreach (var entry in _diagnostics.Entries.Reverse())
            {
                var line = entry.ToString();
                Entries.Add(line);
                sb.AppendLine(line);
                sb.AppendLine("---");
            }

            LogText = sb.Length == 0 ? "(no log entries yet)" : sb.ToString();
            StatusMessage = null;
        }

        [RelayCommand]
        private async Task CopyReportAsync()
        {
            try
            {
                await Clipboard.Default.SetTextAsync(_diagnostics.BuildReport());
                StatusMessage = "Diagnostics copied to clipboard.";
                _diagnostics.Info(nameof(DiagnosticsViewModel), "Diagnostics report copied");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Copy failed: {ex.Message}";
                _diagnostics.Error(nameof(DiagnosticsViewModel), "Copy failed", ex);
            }
        }

        [RelayCommand]
        private void ClearLog()
        {
            _diagnostics.Clear();
            Refresh();
            StatusMessage = "Diagnostics cleared.";
        }

        [RelayCommand]
        private async Task ShareReportAsync()
        {
            try
            {
                IsBusy = true;
                var report = _diagnostics.BuildReport();
                var path = Path.Combine(FileSystem.CacheDirectory, $"diagnostics-{DateTime.Now:yyyyMMdd-HHmmss}.txt");
                await File.WriteAllTextAsync(path, report);
                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = "Launch Tracker diagnostics",
                    File = new ShareFile(path)
                });
                StatusMessage = "Share sheet opened.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Share failed: {ex.Message}";
                _diagnostics.Error(nameof(DiagnosticsViewModel), "Share failed", ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void LogConnectivityProbe()
        {
            var access = Connectivity.Current.NetworkAccess;
            var profiles = string.Join(", ", Connectivity.Current.ConnectionProfiles);
            _diagnostics.Info(
                "Connectivity",
                $"NetworkAccess={access}; Profiles={(string.IsNullOrWhiteSpace(profiles) ? "none" : profiles)}");
            Refresh();
            StatusMessage = $"Network: {access}";
        }
    }
}

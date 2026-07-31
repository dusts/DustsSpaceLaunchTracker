using CommunityToolkit.Mvvm.Input;
using DustsSpaceLaunchTracker.Helpers;
using DustsSpaceLaunchTracker.Models;
using DustsSpaceLaunchTracker.Services;
using System.Net;

namespace DustsSpaceLaunchTracker.ViewModels
{
    public sealed partial class LaunchDetailViewModel : ViewModelBase
    {
        private readonly ILaunchService _launchService;
        private CancellationTokenSource? _loadCts;
        private IDispatcherTimer? _timer;

        private string? _launchId;
        private Launch? _launch;
        private bool _isBusy;
        private string? _errorMessage;
        private string _countdownText = string.Empty;
        private string _localNetText = string.Empty;
        private string _utcNetText = string.Empty;

        public LaunchDetailViewModel(ILaunchService launchService)
        {
            _launchService = launchService;
        }

        public string? LaunchId
        {
            get => _launchId;
            set
            {
                if (SetProperty(ref _launchId, value) && !string.IsNullOrWhiteSpace(value))
                    _ = LoadAsync();
            }
        }

        public Launch? Launch
        {
            get => _launch;
            private set
            {
                if (SetProperty(ref _launch, value))
                {
                    OnPropertyChanged(nameof(HasImage));
                    OnPropertyChanged(nameof(RocketName));
                    OnPropertyChanged(nameof(PadSummary));
                    OnPropertyChanged(nameof(MissionSummary));
                    OnPropertyChanged(nameof(AgencyName));
                    OnPropertyChanged(nameof(WebcastUrl));
                    OnPropertyChanged(nameof(HasWebcast));
                    RefreshTimes();
                }
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            private set => SetProperty(ref _isBusy, value);
        }

        public string? ErrorMessage
        {
            get => _errorMessage;
            private set => SetProperty(ref _errorMessage, value);
        }

        public bool HasImage => !string.IsNullOrWhiteSpace(Launch?.Image);
        public string RocketName =>
            Launch?.Rocket?.Configuration?.FullName
            ?? Launch?.Rocket?.Configuration?.Name
            ?? "Unknown rocket";

        public string PadSummary
        {
            get
            {
                if (Launch?.Pad is null) return "Pad: TBD";
                var loc = Launch.Pad.Location?.Name;
                return string.IsNullOrEmpty(loc)
                    ? Launch.Pad.Name
                    : $"{Launch.Pad.Name} — {loc}";
            }
        }

        public string MissionSummary =>
            Launch?.Mission?.Description
            ?? Launch?.Mission?.Name
            ?? "No mission description.";

        public string AgencyName =>
            Launch?.LaunchServiceProvider?.Name ?? "Unknown agency";

        public string? WebcastUrl =>
            Launch?.VidUrls?.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v.Url))?.Url;

        public bool HasWebcast => !string.IsNullOrWhiteSpace(WebcastUrl);

        public string CountdownText
        {
            get => _countdownText;
            private set => SetProperty(ref _countdownText, value);
        }

        public string LocalNetText
        {
            get => _localNetText;
            private set => SetProperty(ref _localNetText, value);
        }

        public string UtcNetText
        {
            get => _utcNetText;
            private set => SetProperty(ref _utcNetText, value);
        }

        [RelayCommand]
        private async Task LoadAsync()
        {
            if (string.IsNullOrWhiteSpace(LaunchId))
                return;

            _loadCts?.Cancel();
            _loadCts = new CancellationTokenSource();
            var ct = _loadCts.Token;

            IsBusy = true;
            ErrorMessage = null;

            try
            {
                Launch = await _launchService.GetLaunchDetailAsync(LaunchId, forceRefresh: false, ct);
                EnsureTimer();
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                ErrorMessage = FriendlyError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            if (string.IsNullOrWhiteSpace(LaunchId))
                return;

            _loadCts?.Cancel();
            _loadCts = new CancellationTokenSource();
            IsBusy = true;
            ErrorMessage = null;
            try
            {
                Launch = await _launchService.GetLaunchDetailAsync(LaunchId, forceRefresh: true, _loadCts.Token);
            }
            catch (Exception ex)
            {
                ErrorMessage = FriendlyError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task OpenWebcastAsync()
        {
            if (WebcastUrl is null) return;
            try
            {
                await Browser.Default.OpenAsync(WebcastUrl, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Could not open webcast: {ex.Message}";
            }
        }

        private void EnsureTimer()
        {
            if (_timer is not null) return;
            var d = Application.Current?.Dispatcher;
            if (d is null) return;
            _timer = d.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (_, _) => RefreshTimes();
            _timer.Start();
        }

        private void RefreshTimes()
        {
            CountdownText = LaunchTimeFormatter.FormatCountdown(Launch?.Net, DateTime.UtcNow);
            LocalNetText = LaunchTimeFormatter.FormatLocal(Launch?.Net);
            UtcNetText = LaunchTimeFormatter.FormatUtc(Launch?.Net);
        }

        private static string FriendlyError(Exception ex) => ex switch
        {
            Refit.ApiException api when api.StatusCode == HttpStatusCode.TooManyRequests =>
                "Rate limited. Try again shortly.",
            HttpRequestException => "Network error. Cached detail shown if available.",
            TaskCanceledException => "Request timed out.",
            _ => string.IsNullOrWhiteSpace(ex.Message) ? "Failed to load launch." : ex.Message
        };

        public void OnDisappearing()
        {
            _loadCts?.Cancel();
            if (_timer is not null)
            {
                _timer.Stop();
                _timer = null;
            }
        }
    }
}

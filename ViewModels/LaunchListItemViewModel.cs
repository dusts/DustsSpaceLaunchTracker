using DustsSpaceLaunchTracker.Helpers;
using DustsSpaceLaunchTracker.Models;

namespace DustsSpaceLaunchTracker.ViewModels
{
    /// <summary>List row model with live countdown + local/UTC times.</summary>
    public sealed class LaunchListItemViewModel : ViewModelBase
    {
        private string _countdownText = string.Empty;
        private string _localNetText = string.Empty;
        private string _utcNetText = string.Empty;

        public LaunchListItemViewModel(Launch launch)
        {
            Launch = launch;
            RefreshTimes(DateTime.UtcNow);
        }

        public Launch Launch { get; }

        public string Id => Launch.Id;
        public string Name => Launch.Name;
        public string? ImageUrl => Launch.Image;
        public bool HasImage => !string.IsNullOrWhiteSpace(Launch.Image);

        public string RocketName =>
            Launch.Rocket?.Configuration?.FullName
            ?? Launch.Rocket?.Configuration?.Name
            ?? Launch.LaunchServiceProvider?.Name
            ?? "Unknown rocket";

        public string StatusName => Launch.Status?.Name ?? "Unknown status";
        public LaunchStatus? Status => Launch.Status;
        public string? PadName => Launch.Pad?.Name ?? Launch.Pad?.Location?.Name;

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

        public void RefreshTimes(DateTime utcNow)
        {
            CountdownText = LaunchTimeFormatter.FormatCountdown(Launch.Net, utcNow);
            LocalNetText = LaunchTimeFormatter.FormatLocal(Launch.Net);
            UtcNetText = LaunchTimeFormatter.FormatUtc(Launch.Net);
        }
    }
}

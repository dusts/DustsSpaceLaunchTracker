using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DustsSpaceLaunchTracker.Models;
using DustsSpaceLaunchTracker.Services;
using System.Collections.ObjectModel;

namespace DustsSpaceLaunchTracker.ViewModels
{
    public partial class UpcomingLaunchesViewModel : ObservableObject
    {
        private readonly LaunchService _launchService;

        [ObservableProperty]
        private ObservableCollection<Launch> launches = new();  // ← now partial property (no private field!)

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string? errorMessage;  // renamed slightly for clarity – optional

        public UpcomingLaunchesViewModel(LaunchService launchService)
        {
            _launchService = launchService;
        }

        [RelayCommand]
        private async Task LoadLaunchesAsync()
        {
            if (IsLoading) return;

            IsLoading = true;
            ErrorMessage = null;

            try
            {
                var items = await _launchService.GetUpcomingAsync(limit: 10);
                Launches.Clear();
                foreach (var launch in items)
                {
                    Launches.Add(launch);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Optional: auto-load on navigation / page appear
        public async Task OnAppearingAsync()
        {
            await LoadLaunchesAsync();
        }
    }
}

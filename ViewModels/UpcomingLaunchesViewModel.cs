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
        private ObservableCollection<Launch> launches = new();

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string? errorMessage;

        public UpcomingLaunchesViewModel(LaunchService launchService)
        {
            _launchService = launchService;
        }

        // This generates: LoadLaunchesCommand (ICommand) + LoadLaunchesAsync() method
        [RelayCommand]
        private async Task LoadLaunchesAsync()
        {
            if (IsLoading) return;

            IsLoading = true;
            ErrorMessage = null;

            try
            {
                var items = await _launchService.GetUpcomingAsync(limit: 15); // adjust limit as needed
                Launches.Clear();
                foreach (var item in items)
                {
                    Launches.Add(item);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Call this from page OnAppearing
        public async Task InitializeAsync()
        {
            await LoadLaunchesAsync();
        }
    }
}

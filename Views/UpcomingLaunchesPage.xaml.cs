using DustsSpaceLaunchTracker.ViewModels;

namespace DustsSpaceLaunchTracker.Views;

public partial class UpcomingLaunchesPage : ContentPage
{
    private readonly UpcomingLaunchesViewModel _vm;

    public UpcomingLaunchesPage(UpcomingLaunchesViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.InitializeAsync();
    }
}
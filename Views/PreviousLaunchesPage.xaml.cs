using DustsSpaceLaunchTracker.ViewModels;

namespace DustsSpaceLaunchTracker.Views;

public partial class PreviousLaunchesPage : ContentPage
{
    private readonly PreviousLaunchesViewModel _vm;

    public PreviousLaunchesPage(PreviousLaunchesViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.InitializeAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _vm.OnDisappearing();
    }
}

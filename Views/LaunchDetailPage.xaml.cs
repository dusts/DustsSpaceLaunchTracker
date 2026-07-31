using DustsSpaceLaunchTracker.ViewModels;

namespace DustsSpaceLaunchTracker.Views;

[QueryProperty(nameof(LaunchId), "LaunchId")]
public partial class LaunchDetailPage : ContentPage
{
    private readonly LaunchDetailViewModel _vm;

    public LaunchDetailPage(LaunchDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    public string LaunchId
    {
        set => _vm.LaunchId = value;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _vm.OnDisappearing();
    }
}

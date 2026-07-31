using DustsSpaceLaunchTracker.ViewModels;

namespace DustsSpaceLaunchTracker.Views;

public partial class DiagnosticsPage : ContentPage
{
    private readonly DiagnosticsViewModel _vm;

    public DiagnosticsPage(DiagnosticsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.OnAppearing();
    }
}

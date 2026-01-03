namespace PrismaAI.UI.Views;

public partial class SettingsPage : ContentPage
{
    private readonly ViewModels.SettingsViewModel _viewModel;

    public SettingsPage(ViewModels.SettingsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}

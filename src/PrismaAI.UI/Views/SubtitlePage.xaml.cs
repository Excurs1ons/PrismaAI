using PrismaAI.UI.ViewModels;

namespace PrismaAI.UI.Views;

public partial class SubtitlePage : ContentPage
{
    private readonly SubtitleViewModel _viewModel;

    public SubtitlePage(SubtitleViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // 页面消失时可以暂停
    }
}

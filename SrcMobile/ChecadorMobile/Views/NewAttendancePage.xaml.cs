using MauiAppChecador.ViewModels;

namespace MauiAppChecador.Views;

public partial class NewAttendancePage : ContentPage
{
    private readonly NewAttendanceViewModel _viewModel;

    public NewAttendancePage(NewAttendanceViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}

using MauiAppChecador.ViewModels;

namespace MauiAppChecador.Views;

public partial class NewAttendancePage : ContentPage
{
    public NewAttendancePage(NewAttendanceViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}

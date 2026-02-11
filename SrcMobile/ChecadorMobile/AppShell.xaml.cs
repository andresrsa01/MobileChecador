using MauiAppChecador.Services;
using MauiAppChecador.Views;
using MauiAppChecador.ViewModels;

namespace MauiAppChecador;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        var result = await DisplayAlert("Cerrar Sesion", "¿Esta seguro que desea cerrar sesion?", "Si", "No");
        
        if (result)
        {
            var authService = Handler?.MauiContext?.Services.GetService<IAuthService>();
            authService?.Logout();
            
            // Navegar al login
            await GoToAsync("///login");
        }
    }

    private void OnSettingsClicked(object sender, EventArgs e)
    {
        AppInfo.ShowSettingsUI();
    }
}

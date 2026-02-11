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
        var result = await DisplayAlert(
            "Cerrar Sesión", 
            "¿Está seguro que desea cerrar sesión?\n\nSe eliminarán todos los datos locales de la aplicación.", 
            "Sí", 
            "No");
        
        if (result)
        {
            try
            {
                var authService = Handler?.MauiContext?.Services.GetService<IAuthService>();
                if (authService != null)
                {
                    await authService.LogoutAsync();
                }
                
                // Navegar al login
                await GoToAsync("///login");
            }
            catch (Exception ex)
            {
                await DisplayAlert(
                    "Error",
                    $"Error al cerrar sesión: {ex.Message}",
                    "OK");
            }
        }
    }

    private void OnSettingsClicked(object sender, EventArgs e)
    {
        AppInfo.ShowSettingsUI();
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiAppChecador.Services;

namespace MauiAppChecador.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string username = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Por favor ingrese usuario y contraseña";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var result = await _authService.LoginAsync(Username, Password);

            if (result.Success)
            {
                // Navegar a la página principal
                Application.Current!.MainPage = new AppShell();
                await Shell.Current.GoToAsync("///main");
                
                // Verificar permisos de ubicación después del login
                await CheckLocationPermissionsAfterLoginAsync();
            }
            else
            {
                ErrorMessage = result.Message;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CheckLocationPermissionsAfterLoginAsync()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                // Preguntar al usuario si desea otorgar permisos ahora
                bool shouldRequest = await Application.Current?.MainPage?.DisplayAlert(
                    "Permisos de Ubicación",
                    "La aplicación necesita acceso a tu ubicación para registrar asistencias. ¿Deseas habilitar los permisos ahora?",
                    "Sí", "Más tarde")!;

                if (shouldRequest)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                    if (status == PermissionStatus.Granted)
                    {
                        await Application.Current?.MainPage?.DisplayAlert(
                            "Permisos Otorgados",
                            "Los permisos de ubicación han sido habilitados correctamente.",
                            "OK")!;
                    }
                    else if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
                    {
                        // En iOS, si se deniega, debe ir a configuración
                        await Application.Current?.MainPage?.DisplayAlert(
                            "Permisos Denegados",
                            "Para habilitar los permisos de ubicación, ve a:\nConfiguracion > MobileChecador > Ubicación",
                            "Entendido")!;
                    }
                    else
                    {
                        await Application.Current?.MainPage?.DisplayAlert(
                            "Permisos Denegados",
                            "Los permisos de ubicación fueron denegados. Puedes habilitarlos más tarde en la configuración de tu dispositivo.",
                            "OK")!;
                    }
                }
                else
                {
                    // Usuario eligió "Más tarde"
                    await Application.Current?.MainPage?.DisplayAlert(
                        "Recordatorio",
                        "Recuerda que necesitarás habilitar los permisos de ubicación para poder registrar tu asistencia.",
                        "OK")!;
                }
            }
        }
        catch (Exception ex)
        {
            // Silenciosamente fallar, no interrumpir la experiencia del usuario
            System.Diagnostics.Debug.WriteLine($"Error checking location permissions: {ex.Message}");
        }
    }

    [RelayCommand]
    private void ClearForm()
    {
        Username = string.Empty;
        Password = string.Empty;
        ErrorMessage = string.Empty;
    }
}

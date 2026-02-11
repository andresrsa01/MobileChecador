using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiAppChecador.Models;
using MauiAppChecador.Services;

namespace MauiAppChecador.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private User? currentUser;

    [ObservableProperty]
    private string userProfileName = string.Empty;

    [ObservableProperty]
    private string fullName = string.Empty;

    [ObservableProperty]
    private string userEmail = string.Empty;

    [ObservableProperty]
    private string createdAt = string.Empty;

    [ObservableProperty]
    private string lastLogin = string.Empty;

    [ObservableProperty]
    private bool isActive;

    public ProfileViewModel(IAuthService authService)
    {
        _authService = authService;
        _ = LoadUserDataAsync();
    }

    private async Task LoadUserDataAsync()
    {
        CurrentUser = await _authService.GetCurrentUserAsync();
        
        if (CurrentUser != null)
        {
            UserProfileName = CurrentUser.Username;
            FullName = CurrentUser.FullName;
            UserEmail = CurrentUser.Email;
            CreatedAt = CurrentUser.CreatedAt.ToString("dd/MM/yyyy HH:mm");
            LastLogin = CurrentUser.LastLogin?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";
            IsActive = CurrentUser.IsActive;
        }
    }

    [RelayCommand]
    private async Task RefreshProfile()
    {
        await LoadUserDataAsync();
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        var result = await Application.Current!.MainPage!.DisplayAlert(
            "Cerrar Sesión", 
            "¿Está seguro que desea cerrar sesión?\n\nSe eliminarán todos los datos locales de la aplicación.", 
            "Sí", 
            "No");
        
        if (result)
        {
            try
            {
                await _authService.LogoutAsync();
                await Shell.Current.GoToAsync("///login");
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Error",
                    $"Error al cerrar sesión: {ex.Message}",
                    "OK");
            }
        }
    }
}

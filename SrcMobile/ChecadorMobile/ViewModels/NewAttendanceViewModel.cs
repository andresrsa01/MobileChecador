using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiAppChecador.Models;
using MauiAppChecador.Services;

namespace MauiAppChecador.ViewModels;

public partial class NewAttendanceViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly IApiService _apiService;

    [ObservableProperty]
    private string statusMessage = "Presiona el botón para registrar tu asistencia";

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private double latitude;

    [ObservableProperty]
    private double longitude;

    public NewAttendanceViewModel(IAuthService authService, IApiService apiService)
    {
        _authService = authService;
        _apiService = apiService;
    }

    [RelayCommand]
    private async Task RegisterAttendanceAsync()
    {
        if (IsLoading)
            return;

        IsLoading = true;
        StatusMessage = "Obteniendo ubicación...";

        try
        {
            // Obtener ubicación actual
            var location = await GetCurrentLocationAsync();

            if (location == null)
            {
                StatusMessage = "No se pudo obtener la ubicación";
                return;
            }

            Latitude = location.Latitude;
            Longitude = location.Longitude;

            StatusMessage = "Enviando asistencia...";

            // Obtener usuario actual
            var currentUser = _authService.GetCurrentUser();

            if (currentUser == null)
            {
                StatusMessage = "No hay usuario autenticado";
                return;
            }

            // Crear request
            var request = new AttendanceRequest
            {
                UserId = currentUser.Id,
                Latitude = Latitude,
                Longitude = Longitude,
                Timestamp = DateTime.UtcNow
            };

            // Enviar a la API
            var response = await _apiService.RegisterAttendanceAsync(request);

            if (response.Success)
            {
                StatusMessage = $"? {response.Message}";
                await Application.Current?.MainPage?.DisplayAlert("Éxito", response.Message, "OK")!;
            }
            else
            {
                StatusMessage = $"? {response.Message}";
                await Application.Current?.MainPage?.DisplayAlert("Error", response.Message, "OK")!;
            }
        }
        catch (FeatureNotSupportedException)
        {
            StatusMessage = "La geolocalización no está soportada en este dispositivo";
            await Application.Current?.MainPage?.DisplayAlert("Error", "La geolocalización no está disponible", "OK")!;
        }
        catch (PermissionException)
        {
            StatusMessage = "No se tienen permisos de ubicación";
            await Application.Current?.MainPage?.DisplayAlert("Error", "Se requieren permisos de ubicación", "OK")!;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
            await Application.Current?.MainPage?.DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK")!;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task<Location?> GetCurrentLocationAsync()
    {
        try
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            var location = await Geolocation.Default.GetLocationAsync(request);
            return location;
        }
        catch
        {
            return null;
        }
    }
}

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

    [ObservableProperty]
    private bool hasLocationPermission;

    [ObservableProperty]
    private bool hasInternetConnection;

    public NewAttendanceViewModel(IAuthService authService, IApiService apiService)
    {
        _authService = authService;
        _apiService = apiService;
        
        // Suscribirse a cambios de conectividad
        Connectivity.Current.ConnectivityChanged += OnConnectivityChanged;
        HasInternetConnection = Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
    }

    public async Task InitializeAsync()
    {
        await CheckLocationPermissionAsync();
        CheckInternetConnection();
    }

    private async Task CheckLocationPermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        HasLocationPermission = status == PermissionStatus.Granted;

        if (!HasLocationPermission)
        {
            StatusMessage = "?? Se requieren permisos de ubicación para registrar asistencia";
        }
        else if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            StatusMessage = "?? Sin conexión a Internet";
        }
        else
        {
            StatusMessage = "Presiona el botón para registrar tu asistencia";
        }
    }

    private void CheckInternetConnection()
    {
        HasInternetConnection = Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
        
        if (!HasInternetConnection && HasLocationPermission)
        {
            StatusMessage = "?? Sin conexión a Internet";
        }
    }

    private void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        HasInternetConnection = e.NetworkAccess == NetworkAccess.Internet;
        
        if (!HasInternetConnection)
        {
            StatusMessage = "?? Sin conexión a Internet";
        }
        else if (HasLocationPermission)
        {
            StatusMessage = "Presiona el botón para registrar tu asistencia";
        }
    }

    [RelayCommand]
    private async Task RegisterAttendanceAsync()
    {
        if (IsLoading)
            return;

        IsLoading = true;

        try
        {
            // Verificar conexión a Internet
            if (!CheckInternetConnectivity())
            {
                StatusMessage = "?? Sin conexión a Internet";
                await Application.Current?.MainPage?.DisplayAlert(
                    "Sin Conexión",
                    "No hay conexión a Internet. Por favor, verifica tu conexión e intenta nuevamente.",
                    "OK")!;
                return;
            }

            // Verificar permisos de ubicación
            var hasPermission = await RequestLocationPermissionAsync();
            
            if (!hasPermission)
            {
                StatusMessage = "?? Se requieren permisos de ubicación para continuar";
                await Application.Current?.MainPage?.DisplayAlert(
                    "Permisos Requeridos", 
                    "Para registrar tu asistencia, necesitas habilitar los permisos de ubicación en la configuración de tu dispositivo.",
                    "OK")!;
                return;
            }

            StatusMessage = "Obteniendo ubicación...";

            // Obtener ubicación actual
            var location = await GetCurrentLocationAsync();

            if (location == null)
            {
                StatusMessage = "No se pudo obtener la ubicación";
                await Application.Current?.MainPage?.DisplayAlert(
                    "Error de Ubicación",
                    "No se pudo obtener tu ubicación actual. Verifica que el GPS esté activado.",
                    "OK")!;
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
                await Application.Current?.MainPage?.DisplayAlert("Error", "No hay usuario autenticado", "OK")!;
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

            // Verificar conexión a Internet nuevamente antes de enviar
            if (!CheckInternetConnectivity())
            {
                StatusMessage = "?? Se perdió la conexión a Internet";
                await Application.Current?.MainPage?.DisplayAlert(
                    "Sin Conexión",
                    "Se perdió la conexión a Internet. Por favor, verifica tu conexión e intenta nuevamente.",
                    "OK")!;
                return;
            }

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
            await Application.Current?.MainPage?.DisplayAlert(
                "Error", 
                "La geolocalización no está disponible en este dispositivo", 
                "OK")!;
        }
        catch (PermissionException)
        {
            StatusMessage = "?? No se tienen permisos de ubicación";
            await Application.Current?.MainPage?.DisplayAlert(
                "Permisos Denegados",
                "Los permisos de ubicación fueron denegados. Por favor, habilítalos en la configuración de tu dispositivo.",
                "OK")!;
        }
        catch (HttpRequestException)
        {
            StatusMessage = "?? Error de conexión con el servidor";
            await Application.Current?.MainPage?.DisplayAlert(
                "Error de Conexión",
                "No se pudo conectar con el servidor. Verifica tu conexión a Internet e intenta nuevamente.",
                "OK")!;
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

    private bool CheckInternetConnectivity()
    {
        return Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
    }

    private async Task<bool> RequestLocationPermissionAsync()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status == PermissionStatus.Granted)
            {
                HasLocationPermission = true;
                return true;
            }

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // En iOS, si el permiso fue denegado, el usuario debe ir a configuración
                HasLocationPermission = false;
                await Application.Current?.MainPage?.DisplayAlert(
                    "Permisos de Ubicación",
                    "Los permisos de ubicación están deshabilitados. Por favor, ve a Configuración > MobileChecador > Ubicación y habilítalos.",
                    "OK")!;
                return false;
            }

            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            HasLocationPermission = status == PermissionStatus.Granted;

            return HasLocationPermission;
        }
        catch (Exception)
        {
            HasLocationPermission = false;
            return false;
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

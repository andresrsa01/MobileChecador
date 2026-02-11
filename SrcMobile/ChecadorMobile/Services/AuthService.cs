using MauiAppChecador.Models;
using System.Text.Json;

namespace MauiAppChecador.Services;

public class AuthService : IAuthService
{
    private readonly IDatabaseService _databaseService;
    private readonly IApiService _apiService;
    private User? _currentUser;
    private string? _token;

    // Claves para Preferences
    private const string AUTH_TOKEN_KEY = "auth_token";
    private const string USER_ID_KEY = "user_id";
    private const string USER_DATA_KEY = "user_data";

    public AuthService(IDatabaseService databaseService, IApiService apiService)
    {
        _databaseService = databaseService;
        _apiService = apiService;
    }

    public async Task<LoginResponse> LoginAsync(string username, string password)
    {
        try
        {
            // Intentar login con API
            var request = new LoginRequest
            {
                Username = username,
                Password = password
            };

            var response = await _apiService.LoginAsync(request);

            if (response.Success)
            {
                _currentUser = response.User;
                _token = response.Token;

                // Guardar usuario en base de datos local
                if (_currentUser != null)
                {
                    // Actualizar LastLogin antes de guardar
                    _currentUser.LastLogin = DateTime.Now;
                    
                    var saveResult = await _databaseService.SaveUserAsync(_currentUser);
                    System.Diagnostics.Debug.WriteLine($"[AuthService] Usuario guardado con resultado: {saveResult}, ID={_currentUser.Id}, Username={_currentUser.Username}");
                    
                    // Guardar geofence si viene en la respuesta
                    if (response.GeofenceConfig != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"[AuthService] Guardando GeofenceConfig: Lat={response.GeofenceConfig.CenterLatitude}, Lng={response.GeofenceConfig.CenterLongitude}, Radius={response.GeofenceConfig.RadiusInMeters}");
                        var result = await _databaseService.SaveGeofenceConfigAsync(response.GeofenceConfig);
                        System.Diagnostics.Debug.WriteLine($"[AuthService] GeofenceConfig guardado con resultado: {result}");
                    }
                }

                // Guardar datos en preferences
                SaveUserDataToPreferences(_currentUser, _token);
            }

            return response;
        }
        catch(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AuthService] Error en LoginAsync: {ex.Message}");
            
            // Si falla la API, intentar login local
            var user = await _databaseService.GetUserByUsernameAsync(username);
            
            if (user != null && user.Password == password && user.IsActive)
            {
                // Actualizar ultimo login
                user.LastLogin = DateTime.Now;
                await _databaseService.SaveUserAsync(user);
                
                _currentUser = user;
                
                // Guardar datos en preferences
                SaveUserDataToPreferences(_currentUser, null);
                
                return new LoginResponse
                {
                    Success = true,
                    Message = "Login local exitoso",
                    User = _currentUser
                };
            }

            return new LoginResponse
            {
                Success = false,
                Message = "Usuario o contraseña incorrectos"
            };
        }
    }

    public async Task LogoutAsync()
    {
        System.Diagnostics.Debug.WriteLine("[AuthService] Iniciando logout y limpieza de datos");
        
        // Limpiar variables en memoria
        _currentUser = null;
        _token = null;
        
        // Limpiar Preferences
        Preferences.Remove(AUTH_TOKEN_KEY);
        Preferences.Remove(USER_ID_KEY);
        Preferences.Remove(USER_DATA_KEY);
        
        // Limpiar completamente la base de datos
        try
        {
            await _databaseService.ClearAllDataAsync();
            System.Diagnostics.Debug.WriteLine("[AuthService] Logout completado exitosamente");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AuthService] Error durante logout: {ex.Message}");
        }
    }

    public bool IsAuthenticated()
    {
        return _currentUser != null || Preferences.ContainsKey(USER_ID_KEY);
    }

    public User? GetCurrentUser()
    {
        return _currentUser;
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        // Si ya está en memoria, devolverlo
        if (_currentUser != null)
            return _currentUser;

        // Intentar cargar desde Preferences
        if (Preferences.ContainsKey(USER_DATA_KEY))
        {
            try
            {
                var userData = Preferences.Get(USER_DATA_KEY, string.Empty);
                _currentUser = JsonSerializer.Deserialize<User>(userData);
                
                if (_currentUser != null)
                    return _currentUser;
            }
            catch
            {
                // Si hay error deserializando, continuar con BD
            }
        }

        // Si no está en Preferences, cargar desde BD
        if (Preferences.ContainsKey(USER_ID_KEY))
        {
            var userId = Preferences.Get(USER_ID_KEY, 0);
            if (userId > 0)
            {
                _currentUser = await _databaseService.GetUserByIdAsync(userId);
                
                // Guardar en Preferences para proxima vez
                if (_currentUser != null)
                {
                    SaveUserDataToPreferences(_currentUser, _token);
                }
            }
        }

        return _currentUser;
    }

    public async Task InitializeSessionAsync()
    {
        // Cargar token si existe
        if (Preferences.ContainsKey(AUTH_TOKEN_KEY))
        {
            _token = Preferences.Get(AUTH_TOKEN_KEY, string.Empty);
        }

        // Cargar usuario
        await GetCurrentUserAsync();
    }

    private void SaveUserDataToPreferences(User? user, string? token)
    {
        if (user != null)
        {
            // Guardar ID
            Preferences.Set(USER_ID_KEY, user.Id);

            // Guardar datos completos serializados
            var userData = JsonSerializer.Serialize(user);
            Preferences.Set(USER_DATA_KEY, userData);
        }

        // Guardar token si existe
        if (!string.IsNullOrEmpty(token))
        {
            Preferences.Set(AUTH_TOKEN_KEY, token);
        }
    }

    public async Task<GeofenceConfig?> GetGeofenceConfigAsync()
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
        {
            System.Diagnostics.Debug.WriteLine($"[AuthService] GetGeofenceConfigAsync - Usuario no autenticado");
            return null;
        }
        
        var config = await _databaseService.GetGeofenceConfigAsync();
        
        if (config != null)
        {
            System.Diagnostics.Debug.WriteLine($"[AuthService] GetGeofenceConfigAsync - Config encontrado: Id={config.Id}, Lat={config.CenterLatitude}, Lng={config.CenterLongitude}, Radius={config.RadiusInMeters}");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"[AuthService] GetGeofenceConfigAsync - Config NO encontrado en BD");
        }
        
        return config;
    }
}

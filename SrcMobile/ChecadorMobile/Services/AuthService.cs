using MauiAppChecador.Models;

namespace MauiAppChecador.Services;

public class AuthService : IAuthService
{
    private readonly IDatabaseService _databaseService;
    private readonly IApiService _apiService;
    private User? _currentUser;
    private string? _token;

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
                    await _databaseService.SaveUserAsync(_currentUser);
                    await _databaseService.UpdateLastLoginAsync(_currentUser.Id);
                }

                // Guardar token en preferences
                Preferences.Set("auth_token", _token);
                Preferences.Set("user_id", _currentUser?.Id ?? 0);
            }

            return response;
        }
        catch
        {
            // Si falla la API, intentar login local
            var user = await _databaseService.GetUserByUsernameAsync(username);
            
            if (user != null && user.Password == password && user.IsActive)
            {
                _currentUser = user;
                
                // Actualizar último login
                await _databaseService.UpdateLastLoginAsync(user.Id);
                
                // Recargar usuario con LastLogin actualizado
                _currentUser = await _databaseService.GetUserByUsernameAsync(username);
                
                Preferences.Set("user_id", user.Id);
                
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

    public void Logout()
    {
        _currentUser = null;
        _token = null;
        Preferences.Remove("auth_token");
        Preferences.Remove("user_id");
    }

    public bool IsAuthenticated()
    {
        return _currentUser != null || Preferences.ContainsKey("user_id");
    }

    public User? GetCurrentUser()
    {
        return _currentUser;
    }
}

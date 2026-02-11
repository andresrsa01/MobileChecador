using MauiAppChecador.Models;

namespace MauiAppChecador.Services;

public interface IDatabaseService
{
    Task InitializeDatabaseAsync();
    Task<User?> GetUserByIdAsync(int userId);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<bool> ValidateCredentialsAsync(string username, string password);
    Task UpdateLastLoginAsync(int userId);
    Task<int> SaveUserAsync(User user);
    Task<List<User>> GetAllUsersAsync();
    Task<int> DeleteUserAsync(User user);
    
    // Geofence methods
    Task<int> SaveGeofenceConfigAsync(GeofenceConfig geofenceConfig);
    Task<GeofenceConfig?> GetGeofenceConfigAsync();
    Task<int> DeleteGeofenceConfigAsync();
}

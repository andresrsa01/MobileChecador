using MauiAppChecador.Models;
using Refit;

namespace MauiAppChecador.Services;

public interface IApiService
{
    [Post("/api/auth/login")]
    Task<LoginResponse> LoginAsync([Body] LoginRequest request);

    [Get("/api/users/{id}")]
    Task<User> GetUserAsync(int id);

    [Get("/api/users")]
    Task<List<User>> GetUsersAsync();

    [Post("/api/attendance/register")]
    Task<AttendanceResponse> RegisterAttendanceAsync([Body] AttendanceRequest request);

    [Get("/api/attendance/user/{userId}")]
    Task<List<Attendance>> GetUserAttendancesAsync(int userId);

    [Get("/api/attendance/today")]
    Task<List<Attendance>> GetTodayAttendancesAsync();

    [Get("/api/geofence")]
    Task<List<GeofenceConfig>> GetGeofenceConfigsAsync();

    [Get("/api/geofence/workplace/{workplaceId}")]
    Task<GeofenceConfig> GetGeofenceByWorkplaceAsync(int workplaceId);

    [Get("/api/geofence/{id}")]
    Task<GeofenceConfig> GetGeofenceByIdAsync(int id);

    [Get("/api/workplace")]
    Task<List<Workplace>> GetWorkplacesAsync([Query] bool includeInactive = false);

    [Get("/api/workplace/{id}")]
    Task<Workplace> GetWorkplaceAsync(int id);

    [Get("/api/workplace/{id}/users")]
    Task<List<User>> GetWorkplaceUsersAsync(int id);
}

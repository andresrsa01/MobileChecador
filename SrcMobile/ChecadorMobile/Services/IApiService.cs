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
}

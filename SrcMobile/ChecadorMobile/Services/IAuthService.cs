using MauiAppChecador.Models;

namespace MauiAppChecador.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(string username, string password);
    void Logout();
    bool IsAuthenticated();
    User? GetCurrentUser();
}

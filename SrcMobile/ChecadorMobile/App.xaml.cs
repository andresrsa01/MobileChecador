using MauiAppChecador.Services;
using MauiAppChecador.Views;
using MauiAppChecador.ViewModels;

namespace MauiAppChecador;

public partial class App : Application
{
    private readonly IAuthService _authService;
    private readonly IDatabaseService _databaseService;
    private readonly LoginViewModel _loginViewModel;

    public App(IAuthService authService, IDatabaseService databaseService, LoginViewModel loginViewModel)
    {
        InitializeComponent();
        _authService = authService;
        _databaseService = databaseService;
        _loginViewModel = loginViewModel;

        // Set MainPage immediately to avoid NotImplementedException
        MainPage = new AppShell();

        // Inicializar la base de datos
        InitializeDatabaseAsync();
    }

    private async void InitializeDatabaseAsync()
    {
        await _databaseService.InitializeDatabaseAsync();
        
        // Inicializar sesión desde datos persistidos
        await _authService.InitializeSessionAsync();

        // Verificar si el usuario ya está autenticado
        if (_authService.IsAuthenticated())
        {
            await Shell.Current.GoToAsync("///main");
        }
        else
        {
            await Shell.Current.GoToAsync("///login");
        }
    }
}

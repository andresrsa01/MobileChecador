using MauiAppChecador.Services;
using MauiAppChecador.ViewModels;
using MauiAppChecador.Views;
using Microsoft.Extensions.Logging;
using Refit;

namespace MauiAppChecador;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = Microsoft.Maui.Hosting.MauiApp.CreateBuilder();
        builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Servicios (Singleton)
        builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();

        // Refit - API Service
        builder.Services.AddRefitClient<IApiService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.ejemplo.com"));

        // ViewModels (Transient)
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();

        // Views (Transient)
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<ProfilePage>();

        return builder.Build();
    }
}

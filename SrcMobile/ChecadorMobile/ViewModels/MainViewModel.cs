using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiAppChecador.Models;
using MauiAppChecador.Services;

namespace MauiAppChecador.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private User? currentUser;

    [ObservableProperty]
    private string welcomeMessage = string.Empty;

    public MainViewModel(IAuthService authService, INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
        LoadUserInfo();
    }

    private void LoadUserInfo()
    {
        CurrentUser = _authService.GetCurrentUser();
        WelcomeMessage = $"{CurrentUser?.FullName ?? CurrentUser?.Username ?? "Usuario"}";
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await Task.Delay(1000);
        LoadUserInfo();
    }

    [RelayCommand]
    private async Task NavigateToNewAttendanceAsync()
    {
        await _navigationService.NavigateToAsync("///newAttendance");
    }

    [RelayCommand]
    private async Task NavigateToListAttendancesAsync()
    {
        await _navigationService.NavigateToAsync("///listAttendances");
    }

    [RelayCommand]
    private async Task NavigateToProfileAsync()
    {
        await _navigationService.NavigateToAsync("///profile");
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        AppInfo.ShowSettingsUI();
    }
}

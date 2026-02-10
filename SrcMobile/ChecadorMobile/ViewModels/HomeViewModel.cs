using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiAppChecador.Services;
using System.Collections.ObjectModel;

namespace MauiAppChecador.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly IDatabaseService _databaseService;

    [ObservableProperty]
    private string title = "Inicio";

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private ObservableCollection<string> items = new();

    public HomeViewModel(IAuthService authService, IDatabaseService databaseService)
    {
        _authService = authService;
        _databaseService = databaseService;
        LoadData();
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;

        try
        {
            Items.Clear();
            
            // Cargar datos de ejemplo
            var users = await _databaseService.GetAllUsersAsync();
            
            foreach (var user in users)
            {
                Items.Add($"{user.Username} - {user.Email}");
            }

            if (Items.Count == 0)
            {
                Items.Add("No hay datos disponibles");
            }
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void LoadData()
    {
        Items.Add("Item 1");
        Items.Add("Item 2");
        Items.Add("Item 3");
    }
}

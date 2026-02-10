using SQLite;
using MauiAppChecador.Models;

namespace MauiAppChecador.Services;

public class DatabaseService : IDatabaseService
{
    private SQLiteAsyncConnection? _database;

    private async Task InitAsync()
    {
        if (_database != null)
            return;

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "mauiapp.db3");
        _database = new SQLiteAsyncConnection(dbPath);
        await _database.CreateTableAsync<User>();
        
        // Crear usuario demo si no existe
        await CreateDemoUserAsync();
    }

    private async Task CreateDemoUserAsync()
    {
        var existingUser = await GetUserByUsernameAsync("admin");
        
        if (existingUser == null)
        {
            var demoUser = new User
            {
                Username = "admin",
                Password = "admin123", // En producción usar hash
                FullName = "Administrador",
                Email = "admin@ejemplo.com",
                CreatedAt = DateTime.Now,
                IsActive = true
            };
            
            await SaveUserAsync(demoUser);
        }
    }

    public async Task InitializeDatabaseAsync()
    {
        await InitAsync();
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        await InitAsync();
        return await _database!.Table<User>()
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync();
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        await InitAsync();
        return await _database!.Table<User>()
            .Where(u => u.Username == username)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> ValidateCredentialsAsync(string username, string password)
    {
        await InitAsync();
        var user = await GetUserByUsernameAsync(username);
        return user != null && user.Password == password && user.IsActive;
    }

    public async Task UpdateLastLoginAsync(int userId)
    {
        await InitAsync();
        var user = await _database!.Table<User>()
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync();
        
        if (user != null)
        {
            user.LastLogin = DateTime.Now;
            await _database!.UpdateAsync(user);
        }
    }

    public async Task<int> SaveUserAsync(User user)
    {
        await InitAsync();
        
        if (user.Id != 0)
        {
            return await _database!.UpdateAsync(user);
        }
        else
        {
            return await _database!.InsertAsync(user);
        }
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        await InitAsync();
        return await _database!.Table<User>().ToListAsync();
    }

    public async Task<int> DeleteUserAsync(User user)
    {
        await InitAsync();
        return await _database!.DeleteAsync(user);
    }
}

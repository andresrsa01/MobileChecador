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
        await _database.CreateTableAsync<GeofenceConfig>();
        
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
                Password = "admin123", // En produccion usar hash
                FullName = "Administrador",
                Email = "admin@ejemplo.com",
                Role = "Administrador",
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
        
        // Verificar si el usuario ya existe en la BD local
        var existingUser = await _database!.Table<User>()
            .Where(u => u.Id == user.Id)
            .FirstOrDefaultAsync();
        
        if (existingUser != null)
        {
            // Si existe, actualizar
            return await _database!.UpdateAsync(user);
        }
        else
        {
            // Si no existe, insertar
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

    // Geofence methods
    public async Task<int> SaveGeofenceConfigAsync(GeofenceConfig geofenceConfig)
    {
        await InitAsync();
        
        System.Diagnostics.Debug.WriteLine($"[DatabaseService] SaveGeofenceConfigAsync - Iniciando guardado");
        System.Diagnostics.Debug.WriteLine($"[DatabaseService] GeofenceConfig: Lat={geofenceConfig.CenterLatitude}, Lng={geofenceConfig.CenterLongitude}, Radius={geofenceConfig.RadiusInMeters}");
        
        // Verificar si ya existe un geofence
        var existing = await GetGeofenceConfigAsync();
        
        geofenceConfig.UpdatedAt = DateTime.UtcNow;
        
        int result;
        if (existing != null)
        {
            System.Diagnostics.Debug.WriteLine($"[DatabaseService] Actualizando GeofenceConfig existente con Id={existing.Id}");
            geofenceConfig.Id = existing.Id;
            result = await _database!.UpdateAsync(geofenceConfig);
            System.Diagnostics.Debug.WriteLine($"[DatabaseService] Update resultado: {result}");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"[DatabaseService] Insertando nuevo GeofenceConfig");
            result = await _database!.InsertAsync(geofenceConfig);
            System.Diagnostics.Debug.WriteLine($"[DatabaseService] Insert resultado: {result}, Id asignado: {geofenceConfig.Id}");
        }
        
        return result;
    }

    public async Task<GeofenceConfig?> GetGeofenceConfigAsync()
    {
        await InitAsync();
        System.Diagnostics.Debug.WriteLine($"[DatabaseService] GetGeofenceConfigAsync - Buscando config en BD");
        
        var config = await _database!.Table<GeofenceConfig>()
            .FirstOrDefaultAsync();
        
        if (config != null)
        {
            System.Diagnostics.Debug.WriteLine($"[DatabaseService] GetGeofenceConfigAsync - Config encontrado: Id={config.Id}");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"[DatabaseService] GetGeofenceConfigAsync - Config NO encontrado");
        }
        
        return config;
    }

    public async Task<int> DeleteGeofenceConfigAsync()
    {
        await InitAsync();
        var config = await GetGeofenceConfigAsync();
        if (config != null)
        {
            return await _database!.DeleteAsync(config);
        }
        return 0;
    }
    
    public async Task ClearAllDataAsync()
    {
        await InitAsync();
        
        System.Diagnostics.Debug.WriteLine("[DatabaseService] Iniciando limpieza completa de la base de datos");
        
        try
        {
            // Eliminar todos los registros de cada tabla
            await _database!.DeleteAllAsync<User>();
            await _database!.DeleteAllAsync<GeofenceConfig>();
            
            System.Diagnostics.Debug.WriteLine("[DatabaseService] Base de datos limpiada exitosamente");
            
            // Recrear usuario demo después de limpiar
            await CreateDemoUserAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DatabaseService] Error al limpiar base de datos: {ex.Message}");
            throw;
        }
    }
}

using CheckadorAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CheckadorAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Attendance> Attendances { get; set; } = null!;
    public DbSet<GeofenceConfig> GeofenceConfigs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar índices
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Configurar relaciones
        modelBuilder.Entity<Attendance>()
            .HasOne(a => a.User)
            .WithMany(u => u.Attendances)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GeofenceConfig>()
            .HasOne(g => g.User)
            .WithOne(u => u.GeofenceConfig)
            .HasForeignKey<GeofenceConfig>(g => g.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Datos iniciales (seed)
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Hash de la contraseña "Admin123!" usando BCrypt (valores pre-calculados para evitar cambios en cada migración)
        // Admin123! -> $2a$11$2oHZX7cKZmJ5qF5kW3kfD.8YqhJ8wHZF6N8kGv9fMp0YxZQdB8d6K
        // User123! -> $2a$11$3pHZX8dLZnK6rG6lX4lgE.9ZriI9xIaG7O9lHw0gNq1ZyaRdC9e7L
        var adminPasswordHash = "$2a$11$2oHZX7cKZmJ5qF5kW3kfD.8YqhJ8wHZF6N8kGv9fMp0YxZQdB8d6K";
        var userPasswordHash = "$2a$11$3pHZX8dLZnK6rG6lX4lgE.9ZriI9xIaG7O9lHw0gNq1ZyaRdC9e7L";
        
        // Fecha fija para los datos iniciales
        var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Usuario administrador
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = adminPasswordHash,
                FullName = "Administrador del Sistema",
                Email = "admin@checador.com",
                Role = "Administrador",
                CreatedAt = seedDate,
                IsActive = true
            },
            new User
            {
                Id = 2,
                Username = "usuario1",
                PasswordHash = userPasswordHash,
                FullName = "Usuario de Prueba",
                Email = "usuario1@checador.com",
                Role = "Usuario",
                CreatedAt = seedDate,
                IsActive = true
            }
        );

        // Geofence para el usuario de prueba
        // Coordenadas de ejemplo (CDMX - Zócalo)
        modelBuilder.Entity<GeofenceConfig>().HasData(
            new GeofenceConfig
            {
                Id = 1,
                UserId = 2,
                CenterLatitude = 19.432608,
                CenterLongitude = -99.133209,
                RadiusInMeters = 200,
                LocationName = "Oficina Central",
                UpdatedAt = seedDate
            }
        );
    }
}

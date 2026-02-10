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
        // Hash de la contraseña "Admin123!" usando BCrypt
        var adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!");
        var userPasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!");

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
                CreatedAt = DateTime.UtcNow,
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
                CreatedAt = DateTime.UtcNow,
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
                UpdatedAt = DateTime.UtcNow
            }
        );
    }
}

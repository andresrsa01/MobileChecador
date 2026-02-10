using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheckadorAPI.Models;

[Table("Users")]
public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Role { get; set; } = "Usuario"; // "Administrador" o "Usuario"

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastLogin { get; set; }

    public bool IsActive { get; set; } = true;

    // Navegación
    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    public virtual GeofenceConfig? GeofenceConfig { get; set; }
}

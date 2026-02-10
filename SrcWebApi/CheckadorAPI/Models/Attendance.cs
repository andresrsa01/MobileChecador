using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheckadorAPI.Models;

[Table("Attendances")]
public class Attendance
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public double Latitude { get; set; }

    [Required]
    public double Longitude { get; set; }

    [Required]
    public DateTime Timestamp { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsWithinGeofence { get; set; } = true;

    public double? DistanceFromCenter { get; set; }

    // Navegación
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
}

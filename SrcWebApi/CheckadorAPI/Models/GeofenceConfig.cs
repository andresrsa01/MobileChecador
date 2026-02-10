using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheckadorAPI.Models;

[Table("GeofenceConfigs")]
public class GeofenceConfig
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public double CenterLatitude { get; set; }

    [Required]
    public double CenterLongitude { get; set; }

    [Required]
    public double RadiusInMeters { get; set; } = 100;

    [Required]
    [MaxLength(200)]
    public string LocationName { get; set; } = string.Empty;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navegación
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
}

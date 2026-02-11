using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheckadorAPI.Models;

[Table("GeofenceConfigs")]
public class GeofenceConfig
{
    [Key]
    public int Id { get; set; }

    // Ahora pertenece a un Workplace en lugar de un User
    [Required]
    public int WorkplaceId { get; set; }

    [Required]
    public double CenterLatitude { get; set; }

    [Required]
    public double CenterLongitude { get; set; }

    [Required]
    public double RadiusInMeters { get; set; } = 100;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navegación
    [ForeignKey(nameof(WorkplaceId))]
    public virtual Workplace Workplace { get; set; } = null!;
}


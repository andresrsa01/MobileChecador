using System.ComponentModel.DataAnnotations;

namespace CheckadorAPI.DTOs;

public class CreateGeofenceRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public double CenterLatitude { get; set; }

    [Required]
    public double CenterLongitude { get; set; }

    [Required]
    [Range(10, 10000)]
    public double RadiusInMeters { get; set; }

    [Required]
    [MaxLength(200)]
    public string LocationName { get; set; } = string.Empty;
}

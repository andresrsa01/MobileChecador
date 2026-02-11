using System.ComponentModel.DataAnnotations;

namespace CheckadorAPI.DTOs;

public class CreateGeofenceRequest
{
    [Required(ErrorMessage = "El ID del workplace es requerido")]
    public int WorkplaceId { get; set; }

    [Required(ErrorMessage = "La latitud del centro es requerida")]
    public double CenterLatitude { get; set; }

    [Required(ErrorMessage = "La longitud del centro es requerida")]
    public double CenterLongitude { get; set; }

    [Required(ErrorMessage = "El radio es requerido")]
    [Range(10, 10000, ErrorMessage = "El radio debe estar entre 10 y 10000 metros")]
    public double RadiusInMeters { get; set; }
}



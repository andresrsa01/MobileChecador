using System.ComponentModel.DataAnnotations;

namespace CheckadorAPI.DTOs;

public class UpdateWorkplaceRequest
{
    [MaxLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
    public string? Name { get; set; }

    [MaxLength(300, ErrorMessage = "La dirección no puede exceder 300 caracteres")]
    public string? Address { get; set; }

    [MaxLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
    [Phone(ErrorMessage = "El formato del teléfono no es válido")]
    public string? Phone { get; set; }

    [MaxLength(10, ErrorMessage = "El código postal no puede exceder 10 caracteres")]
    public string? Zip { get; set; }

    public bool? IsActive { get; set; }

    // Información del Geofence (opcional para actualización)
    [Range(-90, 90, ErrorMessage = "La latitud debe estar entre -90 y 90")]
    public double? CenterLatitude { get; set; }

    [Range(-180, 180, ErrorMessage = "La longitud debe estar entre -180 y 180")]
    public double? CenterLongitude { get; set; }

    [Range(10, 10000, ErrorMessage = "El radio debe estar entre 10 y 10000 metros")]
    public double? RadiusInMeters { get; set; }
}


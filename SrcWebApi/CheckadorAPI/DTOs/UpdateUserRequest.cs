using System.ComponentModel.DataAnnotations;

namespace CheckadorAPI.DTOs;

public class UpdateUserRequest
{
    [MaxLength(200, ErrorMessage = "El nombre completo no puede exceder 200 caracteres")]
    public string? FullName { get; set; }

    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido")]
    [MaxLength(200, ErrorMessage = "El correo electrónico no puede exceder 200 caracteres")]
    public string? Email { get; set; }

    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    [MaxLength(100, ErrorMessage = "La contraseña no puede exceder 100 caracteres")]
    public string? Password { get; set; }

    public bool? IsActive { get; set; }

    public string? Role { get; set; }

    public int? WorkplaceId { get; set; }
}


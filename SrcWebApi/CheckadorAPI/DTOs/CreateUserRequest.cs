using System.ComponentModel.DataAnnotations;

namespace CheckadorAPI.DTOs;

public class CreateUserRequest
{
    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    [MaxLength(100, ErrorMessage = "El nombre de usuario no puede exceder 100 caracteres")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    [MaxLength(100, ErrorMessage = "La contraseña no puede exceder 100 caracteres")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre completo es requerido")]
    [MaxLength(200, ErrorMessage = "El nombre completo no puede exceder 200 caracteres")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es requerido")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido")]
    [MaxLength(200, ErrorMessage = "El correo electrónico no puede exceder 200 caracteres")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El rol es requerido")]
    public string Role { get; set; } = "Usuario";

    // Solo requerido para usuarios de tipo "Usuario", no para "Administrador"
    public int? WorkplaceId { get; set; }
}


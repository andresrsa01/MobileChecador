using System.ComponentModel.DataAnnotations;

namespace CheckadorAPI.DTOs;

/// <summary>
/// Modelo de solicitud para autenticación de usuarios
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Nombre de usuario único en el sistema
    /// </summary>
    /// <example>admin</example>
    [Required]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña del usuario
    /// </summary>
    /// <example>Admin123!</example>
    [Required]
    public string Password { get; set; } = string.Empty;
}

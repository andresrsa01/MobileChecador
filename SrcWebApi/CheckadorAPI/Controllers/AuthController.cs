using CheckadorAPI.Data;
using CheckadorAPI.DTOs;
using CheckadorAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CheckadorAPI.Controllers;

/// <summary>
/// Controlador de autenticación y autorización
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtHelper _jwtHelper;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        ApplicationDbContext context,
        IJwtHelper jwtHelper,
        ILogger<AuthController> logger)
    {
        _context = context;
        _jwtHelper = jwtHelper;
        _logger = logger;
    }

    /// <summary>
    /// Autentica un usuario y devuelve un token JWT
    /// </summary>
    /// <param name="request">Credenciales de inicio de sesión (usuario y contraseña)</param>
    /// <returns>Token JWT y datos del usuario si la autenticación es exitosa</returns>
    /// <response code="200">Login exitoso o credenciales inválidas</response>
    /// <response code="400">Datos de entrada inválidos</response>
    /// <response code="500">Error interno del servidor</response>
    /// <remarks>
    /// Ejemplo de request:
    /// 
    ///     POST /api/Auth/login
    ///     {
    ///        "username": "admin",
    ///        "password": "Admin123!"
    ///     }
    /// 
    /// Este endpoint valida las credenciales del usuario y devuelve:
    /// - Token JWT para autenticación en endpoints protegidos
    /// - Información del usuario (sin contraseña)
    /// - Configuración de geofence si existe
    /// </remarks>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return Ok(new LoginResponse
                {
                    Success = false,
                    Message = "Usuario y contraseña son requeridos"
                });
            }

            var user = await _context.Users
                .Include(u => u.Workplace)
                    .ThenInclude(w => w.GeofenceConfig)
                .FirstOrDefaultAsync(u => u.Username == request.Username);


            if (user == null)
            {
                _logger.LogWarning("Intento de login con usuario inexistente: {Username}", request.Username);
                return Ok(new LoginResponse
                {
                    Success = false,
                    Message = "Usuario o contraseña incorrectos"
                });
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Intento de login con usuario inactivo: {Username}", request.Username);
                return Ok(new LoginResponse
                {
                    Success = false,
                    Message = "Usuario desactivado. Contacta al administrador"
                });
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Intento de login con contraseña incorrecta para usuario: {Username}", request.Username);
                return Ok(new LoginResponse
                {
                    Success = false,
                    Message = "Usuario o contraseña incorrectos"
                });
            }

            // Actualizar último login
            user.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Generar token JWT
            var token = _jwtHelper.GenerateToken(user.Id, user.Username, user.Role);

            _logger.LogInformation("Login exitoso para usuario: {Username}", request.Username);

            return Ok(new LoginResponse
            {
                Success = true,
                Token = token,
                Message = "Login exitoso",
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role,
                    WorkplaceId = user.WorkplaceId,
                    WorkplaceName = user.Workplace?.Name,
                    CreatedAt = user.CreatedAt,
                    LastLogin = user.LastLogin,
                    IsActive = user.IsActive
                },
                GeofenceConfig = user.Workplace?.GeofenceConfig != null ? new GeofenceConfigDto
                {
                    Id = user.Workplace.GeofenceConfig.Id,
                    CenterLatitude = user.Workplace.GeofenceConfig.CenterLatitude,
                    CenterLongitude = user.Workplace.GeofenceConfig.CenterLongitude,
                    RadiusInMeters = user.Workplace.GeofenceConfig.RadiusInMeters,
                    UpdatedAt = user.Workplace.GeofenceConfig.UpdatedAt
                } : null
            });

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el login");
            return StatusCode(500, new LoginResponse
            {
                Success = false,
                Message = "Error interno del servidor"
            });
        }
    }
}

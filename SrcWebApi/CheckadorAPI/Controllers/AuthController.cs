using CheckadorAPI.Data;
using CheckadorAPI.DTOs;
using CheckadorAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CheckadorAPI.Controllers;

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
                .Include(u => u.GeofenceConfig)
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
                    CreatedAt = user.CreatedAt,
                    LastLogin = user.LastLogin,
                    IsActive = user.IsActive
                },
                GeofenceConfig = user.GeofenceConfig != null ? new GeofenceConfigDto
                {
                    Id = user.GeofenceConfig.Id,
                    UserId = user.GeofenceConfig.UserId,
                    CenterLatitude = user.GeofenceConfig.CenterLatitude,
                    CenterLongitude = user.GeofenceConfig.CenterLongitude,
                    RadiusInMeters = user.GeofenceConfig.RadiusInMeters,
                    LocationName = user.GeofenceConfig.LocationName,
                    UpdatedAt = user.GeofenceConfig.UpdatedAt
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

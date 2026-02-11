using CheckadorAPI.Data;
using CheckadorAPI.DTOs;
using CheckadorAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CheckadorAPI.Controllers;

/// <summary>
/// Controlador para la gestión de usuarios del sistema
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        ApplicationDbContext context,
        ILogger<UsersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene la lista de todos los usuarios del sistema
    /// </summary>
    /// <returns>Lista de usuarios sin información sensible</returns>
    /// <response code="200">Lista de usuarios obtenida exitosamente</response>
    /// <response code="401">Token JWT no válido o ausente</response>
    /// <response code="403">Usuario sin permisos de Administrador</response>
    /// <response code="500">Error interno del servidor</response>
    /// <remarks>
    /// **Requiere rol:** Administrador
    /// 
    /// Este endpoint devuelve todos los usuarios registrados en el sistema,
    /// excluyendo información sensible como contraseñas.
    /// </remarks>
    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        try
        {
            var users = await _context.Users
                .Include(u => u.Workplace)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role,
                    WorkplaceId = u.WorkplaceId,
                    WorkplaceName = u.Workplace != null ? u.Workplace.Name : null,
                    CreatedAt = u.CreatedAt,
                    LastLogin = u.LastLogin,
                    IsActive = u.IsActive
                })
                .ToListAsync();

            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuarios");
            return StatusCode(500, new { message = "Error al obtener usuarios" });
        }
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Workplace)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            var userDto = new UserDto
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
            };

            return Ok(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuario {UserId}", id);
            return StatusCode(500, new { message = "Error al obtener usuario" });
        }
    }


    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            // Validar que el rol sea válido
            if (request.Role != "Administrador" && request.Role != "Usuario")
            {
                return BadRequest(new { message = "El rol debe ser 'Administrador' o 'Usuario'" });
            }

            // Validar que los usuarios tengan un workplace asignado (excepto administradores)
            if (request.Role == "Usuario" && !request.WorkplaceId.HasValue)
            {
                return BadRequest(new { message = "Los usuarios deben tener un workplace asignado" });
            }

            // Validar que los administradores no tengan workplace
            if (request.Role == "Administrador" && request.WorkplaceId.HasValue)
            {
                return BadRequest(new { message = "Los administradores no pueden tener un workplace asignado" });
            }

            // Verificar si el workplace existe (si se proporcionó)
            if (request.WorkplaceId.HasValue)
            {
                var workplaceExists = await _context.Workplaces.AnyAsync(w => w.Id == request.WorkplaceId.Value && w.IsActive);
                if (!workplaceExists)
                {
                    return BadRequest(new { message = "El workplace especificado no existe o no está activo" });
                }
            }

            // Verificar si el usuario ya existe
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return BadRequest(new { message = "El nombre de usuario ya existe" });
            }

            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest(new { message = "El correo electrónico ya está registrado" });
            }

            // Hash de la contraseña
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                FullName = request.FullName,
                Email = request.Email,
                Role = request.Role,
                WorkplaceId = request.WorkplaceId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Cargar el workplace para el DTO
            await _context.Entry(user).Reference(u => u.Workplace).LoadAsync();

            _logger.LogInformation("Usuario creado exitosamente: {Username}", user.Username);

            var userDto = new UserDto
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
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear usuario");
            return StatusCode(500, new { message = "Error al crear usuario" });
        }
    }


    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Workplace)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            if (!string.IsNullOrWhiteSpace(request.FullName))
                user.FullName = request.FullName;

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                if (await _context.Users.AnyAsync(u => u.Email == request.Email && u.Id != id))
                {
                    return BadRequest(new { message = "El correo electrónico ya está registrado" });
                }
                user.Email = request.Email;
            }

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            if (request.IsActive.HasValue)
                user.IsActive = request.IsActive.Value;

            if (!string.IsNullOrWhiteSpace(request.Role) && 
                (request.Role == "Administrador" || request.Role == "Usuario"))
            {
                user.Role = request.Role;
                
                // Si cambia a Administrador, quitar el workplace
                if (request.Role == "Administrador")
                {
                    user.WorkplaceId = null;
                }
            }

            // Actualizar workplace si se proporciona
            if (request.WorkplaceId.HasValue)
            {
                // Verificar que no sea administrador
                if (user.Role == "Administrador")
                {
                    return BadRequest(new { message = "Los administradores no pueden tener un workplace asignado" });
                }

                // Verificar que el workplace existe
                var workplaceExists = await _context.Workplaces.AnyAsync(w => w.Id == request.WorkplaceId.Value && w.IsActive);
                if (!workplaceExists)
                {
                    return BadRequest(new { message = "El workplace especificado no existe o no está activo" });
                }

                user.WorkplaceId = request.WorkplaceId;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Usuario actualizado: {UserId}", id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar usuario {UserId}", id);
            return StatusCode(500, new { message = "Error al actualizar usuario" });
        }
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            // No permitir eliminar el usuario actual
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == id.ToString())
            {
                return BadRequest(new { message = "No puedes eliminar tu propia cuenta" });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Usuario eliminado: {UserId}", id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar usuario {UserId}", id);
            return StatusCode(500, new { message = "Error al eliminar usuario" });
        }
    }

}

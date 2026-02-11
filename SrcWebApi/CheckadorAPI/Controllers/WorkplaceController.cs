using CheckadorAPI.Data;
using CheckadorAPI.DTOs;
using CheckadorAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CheckadorAPI.Controllers;

/// <summary>
/// Controlador para la gestión de lugares de trabajo (Workplaces)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
[Tags("Workplaces")]
public class WorkplaceController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<WorkplaceController> _logger;

    public WorkplaceController(ApplicationDbContext context, ILogger<WorkplaceController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtener todos los lugares de trabajo
    /// </summary>
    /// <returns>Lista de workplaces</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WorkplaceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WorkplaceDto>>> GetWorkplaces([FromQuery] bool includeInactive = false)
    {
        try
        {
            var query = _context.Workplaces
                .Include(w => w.GeofenceConfig)
                .AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(w => w.IsActive);
            }

            var workplaces = await query
                .OrderBy(w => w.Name)
                .Select(w => new WorkplaceDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    Address = w.Address,
                    Phone = w.Phone,
                    Zip = w.Zip,
                    CreatedAt = w.CreatedAt,
                    IsActive = w.IsActive,
                    GeofenceConfig = w.GeofenceConfig != null ? new GeofenceConfigDto
                    {
                        Id = w.GeofenceConfig.Id,
                        CenterLatitude = w.GeofenceConfig.CenterLatitude,
                        CenterLongitude = w.GeofenceConfig.CenterLongitude,
                        RadiusInMeters = w.GeofenceConfig.RadiusInMeters,
                        UpdatedAt = w.GeofenceConfig.UpdatedAt
                    } : null
                })
                .ToListAsync();

            return Ok(workplaces);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los workplaces");
            return StatusCode(500, new { message = "Error al obtener los workplaces" });
        }
    }

    /// <summary>
    /// Obtener un lugar de trabajo por ID
    /// </summary>
    /// <param name="id">ID del workplace</param>
    /// <returns>Datos del workplace</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(WorkplaceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkplaceDto>> GetWorkplace(int id)
    {
        try
        {
            var workplace = await _context.Workplaces
                .Include(w => w.GeofenceConfig)
                .Where(w => w.Id == id)
                .Select(w => new WorkplaceDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    Address = w.Address,
                    Phone = w.Phone,
                    Zip = w.Zip,
                    CreatedAt = w.CreatedAt,
                    IsActive = w.IsActive,
                    GeofenceConfig = w.GeofenceConfig != null ? new GeofenceConfigDto
                    {
                        Id = w.GeofenceConfig.Id,
                        CenterLatitude = w.GeofenceConfig.CenterLatitude,
                        CenterLongitude = w.GeofenceConfig.CenterLongitude,
                        RadiusInMeters = w.GeofenceConfig.RadiusInMeters,
                        UpdatedAt = w.GeofenceConfig.UpdatedAt
                    } : null
                })
                .FirstOrDefaultAsync();

            if (workplace == null)
            {
                return NotFound(new { message = "Workplace no encontrado" });
            }

            return Ok(workplace);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el workplace con ID {Id}", id);
            return StatusCode(500, new { message = "Error al obtener el workplace" });
        }
    }

    /// <summary>
    /// Crear un nuevo lugar de trabajo con su configuración de geofence
    /// </summary>
    /// <param name="request">Datos del workplace y su geofence a crear</param>
    /// <returns>Workplace creado con su geofence</returns>
    [HttpPost]
    [ProducesResponseType(typeof(WorkplaceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WorkplaceDto>> CreateWorkplace([FromBody] CreateWorkplaceRequest request)
    {
        try
        {
            // Crear el workplace
            var workplace = new Workplace
            {
                Name = request.Name,
                Address = request.Address,
                Phone = request.Phone,
                Zip = request.Zip,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            // Crear el geofence asociado
            var geofence = new GeofenceConfig
            {
                CenterLatitude = request.CenterLatitude,
                CenterLongitude = request.CenterLongitude,
                RadiusInMeters = request.RadiusInMeters,
                UpdatedAt = DateTime.UtcNow,
                Workplace = workplace // Establecer la relación
            };

            _context.Workplaces.Add(workplace);
            _context.GeofenceConfigs.Add(geofence);
            await _context.SaveChangesAsync();

            var workplaceDto = new WorkplaceDto
            {
                Id = workplace.Id,
                Name = workplace.Name,
                Address = workplace.Address,
                Phone = workplace.Phone,
                Zip = workplace.Zip,
                CreatedAt = workplace.CreatedAt,
                IsActive = workplace.IsActive,
                GeofenceConfig = new GeofenceConfigDto
                {
                    Id = geofence.Id,
                    CenterLatitude = geofence.CenterLatitude,
                    CenterLongitude = geofence.CenterLongitude,
                    RadiusInMeters = geofence.RadiusInMeters,
                    UpdatedAt = geofence.UpdatedAt
                }
            };

            _logger.LogInformation("Workplace creado con geofence: {Name} (ID: {Id})", workplace.Name, workplace.Id);

            return CreatedAtAction(nameof(GetWorkplace), new { id = workplace.Id }, workplaceDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el workplace");
            return StatusCode(500, new { message = "Error al crear el workplace" });
        }
    }


    /// <summary>
    /// Actualizar un lugar de trabajo existente y su geofence
    /// </summary>
    /// <param name="id">ID del workplace</param>
    /// <param name="request">Datos a actualizar</param>
    /// <returns>Workplace actualizado</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(WorkplaceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WorkplaceDto>> UpdateWorkplace(int id, [FromBody] UpdateWorkplaceRequest request)
    {
        try
        {
            var workplace = await _context.Workplaces
                .Include(w => w.GeofenceConfig)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (workplace == null)
            {
                return NotFound(new { message = "Workplace no encontrado" });
            }

            // Actualizar campos del workplace solo si se proporcionaron
            if (request.Name != null)
                workplace.Name = request.Name;

            if (request.Address != null)
                workplace.Address = request.Address;

            if (request.Phone != null)
                workplace.Phone = request.Phone;

            if (request.Zip != null)
                workplace.Zip = request.Zip;

            if (request.IsActive.HasValue)
                workplace.IsActive = request.IsActive.Value;

            // Actualizar campos del geofence si se proporcionaron
            if (workplace.GeofenceConfig != null)
            {
                var geofenceUpdated = false;

                if (request.CenterLatitude.HasValue)
                {
                    workplace.GeofenceConfig.CenterLatitude = request.CenterLatitude.Value;
                    geofenceUpdated = true;
                }

                if (request.CenterLongitude.HasValue)
                {
                    workplace.GeofenceConfig.CenterLongitude = request.CenterLongitude.Value;
                    geofenceUpdated = true;
                }

                if (request.RadiusInMeters.HasValue)
                {
                    workplace.GeofenceConfig.RadiusInMeters = request.RadiusInMeters.Value;
                    geofenceUpdated = true;
                }

                if (geofenceUpdated)
                {
                    workplace.GeofenceConfig.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();

            var workplaceDto = new WorkplaceDto
            {
                Id = workplace.Id,
                Name = workplace.Name,
                Address = workplace.Address,
                Phone = workplace.Phone,
                Zip = workplace.Zip,
                CreatedAt = workplace.CreatedAt,
                IsActive = workplace.IsActive,
                GeofenceConfig = workplace.GeofenceConfig != null ? new GeofenceConfigDto
                {
                    Id = workplace.GeofenceConfig.Id,
                    CenterLatitude = workplace.GeofenceConfig.CenterLatitude,
                    CenterLongitude = workplace.GeofenceConfig.CenterLongitude,
                    RadiusInMeters = workplace.GeofenceConfig.RadiusInMeters,
                    UpdatedAt = workplace.GeofenceConfig.UpdatedAt
                } : null
            };

            _logger.LogInformation("Workplace actualizado: {Name} (ID: {Id})", workplace.Name, workplace.Id);

            return Ok(workplaceDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el workplace con ID {Id}", id);
            return StatusCode(500, new { message = "Error al actualizar el workplace" });
        }
    }


    /// <summary>
    /// Eliminar un lugar de trabajo (soft delete) y su geofence
    /// </summary>
    /// <param name="id">ID del workplace</param>
    /// <returns>Resultado de la operación</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteWorkplace(int id)
    {
        try
        {
            var workplace = await _context.Workplaces
                .Include(w => w.Users)
                .Include(w => w.GeofenceConfig)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (workplace == null)
            {
                return NotFound(new { message = "Workplace no encontrado" });
            }

            // Verificar si tiene usuarios asignados
            if (workplace.Users.Any())
            {
                return BadRequest(new { message = "No se puede eliminar el workplace porque tiene usuarios asignados" });
            }

            // Soft delete del workplace
            workplace.IsActive = false;
            
            _logger.LogInformation("Workplace desactivado: {Name} (ID: {Id})", workplace.Name, workplace.Id);

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el workplace con ID {Id}", id);
            return StatusCode(500, new { message = "Error al eliminar el workplace" });
        }
    }


    /// <summary>
    /// Obtener usuarios de un lugar de trabajo
    /// </summary>
    /// <param name="id">ID del workplace</param>
    /// <returns>Lista de usuarios</returns>
    [HttpGet("{id}/users")]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetWorkplaceUsers(int id)
    {
        try
        {
            var workplace = await _context.Workplaces
                .Include(w => w.Users)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (workplace == null)
            {
                return NotFound(new { message = "Workplace no encontrado" });
            }

            var users = workplace.Users
                .Where(u => u.IsActive)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    LastLogin = u.LastLogin
                })
                .ToList();

            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los usuarios del workplace con ID {Id}", id);
            return StatusCode(500, new { message = "Error al obtener los usuarios" });
        }
    }
}

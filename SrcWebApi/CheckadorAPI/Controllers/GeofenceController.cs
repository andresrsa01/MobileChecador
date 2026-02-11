using CheckadorAPI.Data;
using CheckadorAPI.DTOs;
using CheckadorAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CheckadorAPI.Controllers;

/// <summary>
/// Controlador para consulta de configuraciones de geofence.
/// NOTA: Para crear/actualizar geofences, usar el WorkplaceController
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
[Tags("Geofences")]
public class GeofenceController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GeofenceController> _logger;

    public GeofenceController(
        ApplicationDbContext context,
        ILogger<GeofenceController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtener todas las configuraciones de geofence
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GeofenceConfigDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GeofenceConfigDto>>> GetGeofences()
    {
        try
        {
            var geofences = await _context.GeofenceConfigs
                .Include(g => g.Workplace)
                .Select(g => new GeofenceConfigDto
                {
                    Id = g.Id,
                    CenterLatitude = g.CenterLatitude,
                    CenterLongitude = g.CenterLongitude,
                    RadiusInMeters = g.RadiusInMeters,
                    UpdatedAt = g.UpdatedAt
                })
                .ToListAsync();

            return Ok(geofences);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener geofences");
            return StatusCode(500, new { message = "Error al obtener geofences" });
        }
    }

    /// <summary>
    /// Obtener geofence por ID de workplace
    /// </summary>
    /// <param name="workplaceId">ID del workplace</param>
    [HttpGet("workplace/{workplaceId}")]
    [ProducesResponseType(typeof(GeofenceConfigDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GeofenceConfigDto>> GetGeofenceByWorkplaceId(int workplaceId)
    {
        try
        {
            var geofence = await _context.GeofenceConfigs
                .FirstOrDefaultAsync(g => g.WorkplaceId == workplaceId);

            if (geofence == null)
            {
                return NotFound(new { message = "No hay configuración de geofence para este workplace" });
            }

            var geofenceDto = new GeofenceConfigDto
            {
                Id = geofence.Id,
                CenterLatitude = geofence.CenterLatitude,
                CenterLongitude = geofence.CenterLongitude,
                RadiusInMeters = geofence.RadiusInMeters,
                UpdatedAt = geofence.UpdatedAt
            };

            return Ok(geofenceDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener geofence del workplace {WorkplaceId}", workplaceId);
            return StatusCode(500, new { message = "Error al obtener geofence" });
        }
    }

    /// <summary>
    /// Obtener geofence por ID
    /// </summary>
    /// <param name="id">ID del geofence</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GeofenceConfigDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GeofenceConfigDto>> GetGeofence(int id)
    {
        try
        {
            var geofence = await _context.GeofenceConfigs.FindAsync(id);

            if (geofence == null)
            {
                return NotFound(new { message = "Geofence no encontrado" });
            }

            var geofenceDto = new GeofenceConfigDto
            {
                Id = geofence.Id,
                CenterLatitude = geofence.CenterLatitude,
                CenterLongitude = geofence.CenterLongitude,
                RadiusInMeters = geofence.RadiusInMeters,
                UpdatedAt = geofence.UpdatedAt
            };

            return Ok(geofenceDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener geofence con ID {Id}", id);
            return StatusCode(500, new { message = "Error al obtener geofence" });
        }
    }
}



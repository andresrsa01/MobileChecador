using CheckadorAPI.Data;
using CheckadorAPI.DTOs;
using CheckadorAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CheckadorAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GeofenceConfigDto>>> GetGeofences()
    {
        try
        {
            var geofences = await _context.GeofenceConfigs
                .Include(g => g.User)
                .Select(g => new GeofenceConfigDto
                {
                    Id = g.Id,
                    UserId = g.UserId,
                    CenterLatitude = g.CenterLatitude,
                    CenterLongitude = g.CenterLongitude,
                    RadiusInMeters = g.RadiusInMeters,
                    LocationName = g.LocationName,
                    UpdatedAt = g.UpdatedAt
                })
                .ToListAsync();

            return Ok(geofences);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener geofences");
            return StatusCode(500, "Error al obtener geofences");
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<GeofenceConfigDto>> GetGeofenceByUserId(int userId)
    {
        try
        {
            var geofence = await _context.GeofenceConfigs
                .FirstOrDefaultAsync(g => g.UserId == userId);

            if (geofence == null)
            {
                return NotFound("No hay configuración de geofence para este usuario");
            }

            var geofenceDto = new GeofenceConfigDto
            {
                Id = geofence.Id,
                UserId = geofence.UserId,
                CenterLatitude = geofence.CenterLatitude,
                CenterLongitude = geofence.CenterLongitude,
                RadiusInMeters = geofence.RadiusInMeters,
                LocationName = geofence.LocationName,
                UpdatedAt = geofence.UpdatedAt
            };

            return Ok(geofenceDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener geofence del usuario {UserId}", userId);
            return StatusCode(500, "Error al obtener geofence");
        }
    }

    [HttpPost]
    public async Task<ActionResult<GeofenceConfigDto>> CreateGeofence([FromBody] CreateGeofenceRequest request)
    {
        try
        {
            // Verificar que el usuario existe
            var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);
            if (!userExists)
            {
                return BadRequest("El usuario no existe");
            }

            // Verificar si ya tiene un geofence
            var existingGeofence = await _context.GeofenceConfigs
                .FirstOrDefaultAsync(g => g.UserId == request.UserId);

            if (existingGeofence != null)
            {
                return BadRequest("El usuario ya tiene una configuración de geofence. Usa PUT para actualizarla.");
            }

            var geofence = new GeofenceConfig
            {
                UserId = request.UserId,
                CenterLatitude = request.CenterLatitude,
                CenterLongitude = request.CenterLongitude,
                RadiusInMeters = request.RadiusInMeters,
                LocationName = request.LocationName,
                UpdatedAt = DateTime.UtcNow
            };

            _context.GeofenceConfigs.Add(geofence);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Geofence creado para usuario {UserId}", request.UserId);

            var geofenceDto = new GeofenceConfigDto
            {
                Id = geofence.Id,
                UserId = geofence.UserId,
                CenterLatitude = geofence.CenterLatitude,
                CenterLongitude = geofence.CenterLongitude,
                RadiusInMeters = geofence.RadiusInMeters,
                LocationName = geofence.LocationName,
                UpdatedAt = geofence.UpdatedAt
            };

            return CreatedAtAction(nameof(GetGeofenceByUserId), new { userId = geofence.UserId }, geofenceDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear geofence");
            return StatusCode(500, "Error al crear geofence");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGeofence(int id, [FromBody] CreateGeofenceRequest request)
    {
        try
        {
            var geofence = await _context.GeofenceConfigs.FindAsync(id);

            if (geofence == null)
            {
                return NotFound("Geofence no encontrado");
            }

            geofence.CenterLatitude = request.CenterLatitude;
            geofence.CenterLongitude = request.CenterLongitude;
            geofence.RadiusInMeters = request.RadiusInMeters;
            geofence.LocationName = request.LocationName;
            geofence.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Geofence actualizado: {GeofenceId}", id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar geofence {GeofenceId}", id);
            return StatusCode(500, "Error al actualizar geofence");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGeofence(int id)
    {
        try
        {
            var geofence = await _context.GeofenceConfigs.FindAsync(id);

            if (geofence == null)
            {
                return NotFound("Geofence no encontrado");
            }

            _context.GeofenceConfigs.Remove(geofence);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Geofence eliminado: {GeofenceId}", id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar geofence {GeofenceId}", id);
            return StatusCode(500, "Error al eliminar geofence");
        }
    }
}

using CheckadorAPI.Data;
using CheckadorAPI.DTOs;
using CheckadorAPI.Helpers;
using CheckadorAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CheckadorAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AttendanceController> _logger;

    public AttendanceController(
        ApplicationDbContext context,
        ILogger<AttendanceController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AttendanceResponse>> RegisterAttendance([FromBody] AttendanceRequest request)
    {
        try
        {
            // Verificar que el usuario existe
            var user = await _context.Users
                .Include(u => u.Workplace)
                    .ThenInclude(w => w.GeofenceConfig)
                .FirstOrDefaultAsync(u => u.Id == request.UserId);

            if (user == null)
            {
                return Ok(new AttendanceResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                });
            }

            // Verificar que el usuario tenga un workplace asignado
            if (user.Workplace == null)
            {
                _logger.LogWarning("Usuario {UserId} no tiene workplace asignado", request.UserId);
                return Ok(new AttendanceResponse
                {
                    Success = false,
                    Message = "No tienes un workplace asignado. Contacta al administrador"
                });
            }

            // Verificar geofence
            if (user.Workplace.GeofenceConfig == null)
            {
                _logger.LogWarning("Workplace {WorkplaceId} no tiene configuración de geofence", user.WorkplaceId);
                return Ok(new AttendanceResponse
                {
                    Success = false,
                    Message = "No hay configuración de geofence para tu workplace"
                });
            }

            var distance = GeofenceHelper.CalculateDistance(
                request.Latitude,
                request.Longitude,
                user.Workplace.GeofenceConfig.CenterLatitude,
                user.Workplace.GeofenceConfig.CenterLongitude
            );

            var isWithinGeofence = distance <= user.Workplace.GeofenceConfig.RadiusInMeters;

            if (!isWithinGeofence)
            {
                _logger.LogWarning(
                    "Usuario {UserId} intentó registrar asistencia fuera del geofence. Distancia: {Distance}m",
                    request.UserId,
                    distance
                );

                return Ok(new AttendanceResponse
                {
                    Success = false,
                    Message = $"Ubicación fuera del área permitida. Distancia: {distance:F0}m"
                });
            }


            // Verificar si ya registró asistencia hoy
            var today = DateTime.UtcNow.Date;
            var hasAttendanceToday = await _context.Attendances
                .AnyAsync(a => a.UserId == request.UserId && 
                              a.Timestamp.Date == today);

            if (hasAttendanceToday)
            {
                return Ok(new AttendanceResponse
                {
                    Success = false,
                    Message = "Ya registraste tu asistencia el día de hoy"
                });
            }

            // Crear registro de asistencia
            var attendance = new Attendance
            {
                UserId = request.UserId,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Timestamp = request.Timestamp,
                IsWithinGeofence = true,
                DistanceFromCenter = distance,
                CreatedAt = DateTime.UtcNow
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Asistencia registrada exitosamente para usuario {UserId}. ID: {AttendanceId}",
                request.UserId,
                attendance.Id
            );

            return Ok(new AttendanceResponse
            {
                Success = true,
                Message = "Asistencia registrada exitosamente",
                AttendanceId = attendance.Id
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar asistencia para usuario {UserId}", request.UserId);
            return StatusCode(500, new AttendanceResponse
            {
                Success = false,
                Message = "Error interno del servidor al registrar asistencia"
            });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<Attendance>>> GetUserAttendances(int userId)
    {
        try
        {
            var attendances = await _context.Attendances
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();

            return Ok(attendances);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener asistencias del usuario {UserId}", userId);
            return StatusCode(500, "Error al obtener asistencias");
        }
    }

    [HttpGet("today")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<IEnumerable<Attendance>>> GetTodayAttendances()
    {
        try
        {
            var today = DateTime.UtcNow.Date;
            var attendances = await _context.Attendances
                .Include(a => a.User)
                .Where(a => a.Timestamp.Date == today)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();

            return Ok(attendances);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener asistencias del día");
            return StatusCode(500, "Error al obtener asistencias");
        }
    }
}

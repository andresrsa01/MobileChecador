using System.ComponentModel.DataAnnotations;

namespace CheckadorAPI.DTOs;

public class AttendanceRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public double Latitude { get; set; }

    [Required]
    public double Longitude { get; set; }

    [Required]
    public DateTime Timestamp { get; set; }
}

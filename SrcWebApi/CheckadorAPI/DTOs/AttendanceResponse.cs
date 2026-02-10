namespace CheckadorAPI.DTOs;

public class AttendanceResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int AttendanceId { get; set; }
}

namespace MauiAppChecador.Models;

public class AttendanceRequest
{
    public int UserId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime Timestamp { get; set; }
}

namespace MauiAppChecador.Models;

public class Attendance
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime Timestamp { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsWithinGeofence { get; set; }
    public double? DistanceFromCenter { get; set; }
    public User? User { get; set; }
}

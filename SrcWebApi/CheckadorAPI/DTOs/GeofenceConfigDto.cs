namespace CheckadorAPI.DTOs;

public class GeofenceConfigDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public double CenterLatitude { get; set; }
    public double CenterLongitude { get; set; }
    public double RadiusInMeters { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}

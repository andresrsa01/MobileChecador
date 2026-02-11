using SQLite;

namespace MauiAppChecador.Models;

public class GeofenceConfig
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public double CenterLatitude { get; set; }
    public double CenterLongitude { get; set; }
    public double RadiusInMeters { get; set; }
    public DateTime UpdatedAt { get; set; }
}

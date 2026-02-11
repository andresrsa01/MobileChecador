namespace CheckadorAPI.DTOs;

public class WorkplaceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public GeofenceConfigDto? GeofenceConfig { get; set; }
}

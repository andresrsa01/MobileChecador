using System.ComponentModel.DataAnnotations;

namespace CheckadorAPI.DTOs;

public class UpdateUserRequest
{
    [MaxLength(200)]
    public string? FullName { get; set; }

    [EmailAddress]
    [MaxLength(200)]
    public string? Email { get; set; }

    [MinLength(6)]
    [MaxLength(100)]
    public string? Password { get; set; }

    public bool? IsActive { get; set; }

    public string? Role { get; set; }
}

using SQLite;

namespace MauiAppChecador.Models;

[Table("users")]
public class User
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Unique, MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;

    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? LastLogin { get; set; }

    public bool IsActive { get; set; }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CheckadorAPI.Helpers;

public interface IJwtHelper
{
    string GenerateToken(int userId, string username, string role);
    ClaimsPrincipal? ValidateToken(string token);
}

public class JwtHelper : IJwtHelper
{
    private readonly IConfiguration _configuration;

    public JwtHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(int userId, string username, string role)
    {
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "ThisIsAVerySecureKeyForJWTTokenGeneration123!");
        var issuer = _configuration["Jwt:Issuer"] ?? "CheckadorAPI";
        var audience = _configuration["Jwt:Audience"] ?? "CheckadorMobileApp";

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "ThisIsAVerySecureKeyForJWTTokenGeneration123!");
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"] ?? "CheckadorAPI",
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"] ?? "CheckadorMobileApp",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            return tokenHandler.ValidateToken(token, validationParameters, out _);
        }
        catch
        {
            return null;
        }
    }
}

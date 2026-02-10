# Ejemplo de Implementación del Backend

## ?? Endpoint de Login Actualizado

### Request
```http
POST /api/auth/login HTTP/1.1
Content-Type: application/json

{
  "username": "jperez",
  "password": "miPassword123"
}
```

### Response (Exitoso)
```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwibmFtZSI6Ikp1YW4gUMOpcmV6IiwiaWF0IjoxNjk4ODg4ODg4fQ.abc123...",
  "message": "Login exitoso",
  "user": {
    "id": 1,
    "username": "jperez",
    "fullName": "Juan Pérez",
    "email": "jperez@ejemplo.com",
    "createdAt": "2024-01-15T10:30:00Z",
    "lastLogin": "2024-01-20T14:25:00Z",
    "isActive": true
  },
  "geofenceConfig": {
    "centerLatitude": 19.432608,
    "centerLongitude": -99.133209,
    "radiusInMeters": 100,
    "locationName": "Oficina Principal - CDMX"
  }
}
```

### Response (Usuario sin Geofence configurado)
```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "message": "Login exitoso",
  "user": {
    "id": 2,
    "username": "mlopez",
    "fullName": "María López",
    "email": "mlopez@ejemplo.com",
    "createdAt": "2024-01-10T08:00:00Z",
    "lastLogin": "2024-01-20T09:15:00Z",
    "isActive": true
  },
  "geofenceConfig": null
}
```

### Response (Error de Credenciales)
```json
{
  "success": false,
  "token": "",
  "message": "Usuario o contraseña incorrectos",
  "user": null,
  "geofenceConfig": null
}
```

## ?? Ejemplo de Implementación en C# (Backend ASP.NET Core)

```csharp
// Controllers/AuthController.cs
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IGeofenceService _geofenceService;
    private readonly ITokenService _tokenService;

    public AuthController(
        IUserService userService, 
        IGeofenceService geofenceService,
        ITokenService tokenService)
    {
        _userService = userService;
        _geofenceService = geofenceService;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        // Validar credenciales
        var user = await _userService.ValidateCredentialsAsync(
            request.Username, 
            request.Password
        );

        if (user == null || !user.IsActive)
        {
            return Ok(new LoginResponse
            {
                Success = false,
                Message = "Usuario o contraseña incorrectos"
            });
        }

        // Actualizar último login
        await _userService.UpdateLastLoginAsync(user.Id);

        // Generar token JWT
        var token = _tokenService.GenerateToken(user);

        // Obtener configuración de geofence para este usuario
        var geofenceConfig = await _geofenceService.GetGeofenceByUserIdAsync(user.Id);

        return Ok(new LoginResponse
        {
            Success = true,
            Token = token,
            Message = "Login exitoso",
            User = user,
            GeofenceConfig = geofenceConfig // Puede ser null
        });
    }
}

// Models/LoginRequest.cs
public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

// Models/LoginResponse.cs
public class LoginResponse
{
    public bool Success { get; set; }
    public string Token { get; set; }
    public string Message { get; set; }
    public User User { get; set; }
    public GeofenceConfig GeofenceConfig { get; set; }
}

// Models/GeofenceConfig.cs
public class GeofenceConfig
{
    public double CenterLatitude { get; set; }
    public double CenterLongitude { get; set; }
    public double RadiusInMeters { get; set; }
    public string LocationName { get; set; }
}

// Services/IGeofenceService.cs
public interface IGeofenceService
{
    Task<GeofenceConfig> GetGeofenceByUserIdAsync(int userId);
}

// Services/GeofenceService.cs
public class GeofenceService : IGeofenceService
{
    private readonly AppDbContext _context;

    public GeofenceService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<GeofenceConfig> GetGeofenceByUserIdAsync(int userId)
    {
        // Buscar geofence por UserId en la base de datos
        var geofence = await _context.Geofences
            .Where(g => g.UserId == userId && g.IsActive)
            .FirstOrDefaultAsync();

        if (geofence == null)
            return null;

        return new GeofenceConfig
        {
            CenterLatitude = geofence.CenterLatitude,
            CenterLongitude = geofence.CenterLongitude,
            RadiusInMeters = geofence.RadiusInMeters,
            LocationName = geofence.LocationName
        };
    }
}
```

## ??? Esquema de Base de Datos (Backend)

```sql
-- Tabla de usuarios
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL, -- Debe estar hasheado
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    LastLogin DATETIME2,
    IsActive BIT DEFAULT 1
);

-- Tabla de configuración de geofence
CREATE TABLE Geofences (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    CenterLatitude DECIMAL(10, 8) NOT NULL,
    CenterLongitude DECIMAL(11, 8) NOT NULL,
    RadiusInMeters DECIMAL(10, 2) NOT NULL,
    LocationName NVARCHAR(200) NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Geofences_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Índice para búsqueda rápida
CREATE INDEX IX_Geofences_UserId ON Geofences(UserId) WHERE IsActive = 1;
```

## ?? Ejemplos de Datos de Prueba

```sql
-- Insertar usuarios de prueba
INSERT INTO Users (Username, Password, FullName, Email, IsActive)
VALUES 
    ('jperez', 'hash_de_password_1', 'Juan Pérez', 'jperez@ejemplo.com', 1),
    ('mlopez', 'hash_de_password_2', 'María López', 'mlopez@ejemplo.com', 1),
    ('agarcia', 'hash_de_password_3', 'Ana García', 'agarcia@ejemplo.com', 1);

-- Insertar geofences de prueba
INSERT INTO Geofences (UserId, CenterLatitude, CenterLongitude, RadiusInMeters, LocationName, IsActive)
VALUES 
    -- Oficina Principal (CDMX)
    (1, 19.432608, -99.133209, 100, 'Oficina Principal - CDMX', 1),
    
    -- Oficina Guadalajara
    (2, 20.659699, -103.349609, 150, 'Oficina Guadalajara', 1),
    
    -- Obra en Monterrey
    (3, 25.686613, -100.316116, 300, 'Obra Industrial - Monterrey', 1);
```

## ?? Pruebas con Postman / cURL

```bash
# Prueba de login exitoso
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "jperez",
    "password": "miPassword123"
  }'

# Respuesta esperada: LoginResponse con geofenceConfig incluido
```

## ?? Configuraciones Recomendadas por Tipo de Ubicación

| Tipo de Ubicación | Radio Recomendado | Ejemplo |
|-------------------|-------------------|---------|
| Oficina pequeña | 50m | Edificio de oficinas |
| Oficina grande/corporativo | 100-150m | Campus corporativo |
| Obra/Proyecto | 200-500m | Construcción, instalación |
| Almacén/Bodega | 100-200m | Centro de distribución |
| Tienda/Sucursal | 50-75m | Retail, sucursal bancaria |

## ?? Consideraciones de Seguridad

1. **Nunca** almacenar passwords en texto plano (usar BCrypt, Argon2, etc.)
2. **Usar HTTPS** para todas las comunicaciones
3. **Validar tokens JWT** en cada request
4. **Limitar intentos de login** para prevenir fuerza bruta
5. **Logs de acceso** para auditoría

## ?? Monitoreo y Logs

```csharp
// Ejemplo de logging
_logger.LogInformation(
    "Login exitoso: Usuario={Username}, GeofenceConfigured={HasGeofence}", 
    user.Username, 
    geofenceConfig != null
);
```

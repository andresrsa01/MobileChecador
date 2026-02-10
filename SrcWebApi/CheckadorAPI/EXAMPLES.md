# Ejemplos de Uso de la API

## Configuración Inicial

### Cadenas de Conexión Comunes

#### SQL Server Local (LocalDB)
```json
"Server=(localdb)\\mssqllocaldb;Database=CheckadorDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

#### SQL Server Express
```json
"Server=localhost\\SQLEXPRESS;Database=CheckadorDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

#### SQL Server con autenticación SQL
```json
"Server=localhost;Database=CheckadorDB;User Id=sa;Password=TuPassword123!;TrustServerCertificate=True"
```

#### Azure SQL Database
```json
"Server=tcp:tuservidor.database.windows.net,1433;Database=CheckadorDB;User Id=tuusuario@tuservidor;Password=TuPassword;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
```

## Ejemplos de Llamadas a la API

### 1. Login (Autenticación)

**Endpoint:** `POST /api/auth/login`

```bash
# Con curl
curl -X POST "https://localhost:5001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "Admin123!"
  }' \
  --insecure
```

```powershell
# Con PowerShell
$response = Invoke-RestMethod `
  -Uri "https://localhost:5001/api/auth/login" `
  -Method Post `
  -ContentType "application/json" `
  -Body (@{
    username = "admin"
    password = "Admin123!"
  } | ConvertTo-Json) `
  -SkipCertificateCheck

$token = $response.token
```

```csharp
// Desde C# / .NET MAUI
var client = new HttpClient();
var request = new LoginRequest 
{ 
    Username = "admin", 
    Password = "Admin123!" 
};

var response = await client.PostAsJsonAsync(
    "https://localhost:5001/api/auth/login", 
    request
);

var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
string token = loginResponse.Token;
```

### 2. Crear un Nuevo Usuario

**Endpoint:** `POST /api/users` (requiere rol Administrador)

```bash
curl -X POST "https://localhost:5001/api/users" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TU_TOKEN_AQUI" \
  -d '{
    "username": "juan.perez",
    "password": "Password123!",
    "fullName": "Juan Pérez García",
    "email": "juan.perez@empresa.com",
    "role": "Usuario"
  }' \
  --insecure
```

```powershell
Invoke-RestMethod `
  -Uri "https://localhost:5001/api/users" `
  -Method Post `
  -Headers @{ "Authorization" = "Bearer $token" } `
  -ContentType "application/json" `
  -Body (@{
    username = "juan.perez"
    password = "Password123!"
    fullName = "Juan Pérez García"
    email = "juan.perez@empresa.com"
    role = "Usuario"
  } | ConvertTo-Json) `
  -SkipCertificateCheck
```

### 3. Configurar Geofence para un Usuario

**Endpoint:** `POST /api/geofence` (requiere rol Administrador)

```bash
curl -X POST "https://localhost:5001/api/geofence" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TU_TOKEN_AQUI" \
  -d '{
    "userId": 3,
    "centerLatitude": 19.432608,
    "centerLongitude": -99.133209,
    "radiusInMeters": 150,
    "locationName": "Oficina Central - CDMX"
  }' \
  --insecure
```

**Coordenadas de ejemplo en México:**
- **Zócalo CDMX**: Lat: 19.432608, Lon: -99.133209
- **Torre Latinoamericana**: Lat: 19.433890, Lon: -99.140700
- **Ángel de la Independencia**: Lat: 19.426944, Lon: -99.167778
- **Bosque de Chapultepec**: Lat: 19.419444, Lon: -99.206389

### 4. Registrar Asistencia

**Endpoint:** `POST /api/attendance/register` (requiere autenticación)

```bash
curl -X POST "https://localhost:5001/api/attendance/register" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TU_TOKEN_AQUI" \
  -d '{
    "userId": 2,
    "latitude": 19.432608,
    "longitude": -99.133209,
    "timestamp": "2024-02-10T14:30:00Z"
  }' \
  --insecure
```

```csharp
// Desde .NET MAUI
var client = new HttpClient();
client.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", token);

var request = new AttendanceRequest
{
    UserId = userId,
    Latitude = currentLocation.Latitude,
    Longitude = currentLocation.Longitude,
    Timestamp = DateTime.UtcNow
};

var response = await client.PostAsJsonAsync(
    "https://localhost:5001/api/attendance/register",
    request
);

var result = await response.Content.ReadFromJsonAsync<AttendanceResponse>();
```

### 5. Obtener Asistencias de un Usuario

**Endpoint:** `GET /api/attendance/user/{userId}` (requiere autenticación)

```bash
curl -X GET "https://localhost:5001/api/attendance/user/2" \
  -H "Authorization: Bearer TU_TOKEN_AQUI" \
  --insecure
```

### 6. Obtener Todas las Asistencias del Día

**Endpoint:** `GET /api/attendance/today` (requiere rol Administrador)

```bash
curl -X GET "https://localhost:5001/api/attendance/today" \
  -H "Authorization: Bearer TU_TOKEN_AQUI" \
  --insecure
```

### 7. Listar Todos los Usuarios

**Endpoint:** `GET /api/users` (requiere rol Administrador)

```bash
curl -X GET "https://localhost:5001/api/users" \
  -H "Authorization: Bearer TU_TOKEN_AQUI" \
  --insecure
```

### 8. Actualizar un Usuario

**Endpoint:** `PUT /api/users/{id}` (requiere rol Administrador)

```bash
curl -X PUT "https://localhost:5001/api/users/3" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TU_TOKEN_AQUI" \
  -d '{
    "fullName": "Juan Pérez Actualizado",
    "email": "juan.nuevo@empresa.com",
    "isActive": true
  }' \
  --insecure
```

### 9. Actualizar Geofence

**Endpoint:** `PUT /api/geofence/{id}` (requiere rol Administrador)

```bash
curl -X PUT "https://localhost:5001/api/geofence/1" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TU_TOKEN_AQUI" \
  -d '{
    "userId": 2,
    "centerLatitude": 19.426944,
    "centerLongitude": -99.167778,
    "radiusInMeters": 250,
    "locationName": "Oficina Reforma"
  }' \
  --insecure
```

### 10. Eliminar un Usuario

**Endpoint:** `DELETE /api/users/{id}` (requiere rol Administrador)

```bash
curl -X DELETE "https://localhost:5001/api/users/3" \
  -H "Authorization: Bearer TU_TOKEN_AQUI" \
  --insecure
```

## Integración con .NET MAUI

### Configurar el Cliente HTTP

```csharp
// En MauiProgram.cs
builder.Services.AddSingleton<HttpClient>(sp =>
{
    var handler = new HttpClientHandler
    {
        // Solo para desarrollo - NO usar en producción
        ServerCertificateCustomValidationCallback = 
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };

    var client = new HttpClient(handler);
    
    // Para Android Emulator
    #if ANDROID
    client.BaseAddress = new Uri("http://10.0.2.2:5000");
    #elif IOS
    client.BaseAddress = new Uri("http://localhost:5000");
    #else
    client.BaseAddress = new Uri("http://192.168.1.XXX:5000"); // Tu IP local
    #endif

    return client;
});
```

### Servicio de API en MAUI

```csharp
public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LoginResponse> LoginAsync(string username, string password)
    {
        var request = new LoginRequest 
        { 
            Username = username, 
            Password = password 
        };

        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<LoginResponse>();
    }

    public async Task<AttendanceResponse> RegisterAttendanceAsync(
        int userId, 
        double latitude, 
        double longitude,
        string token)
    {
        var request = new AttendanceRequest
        {
            UserId = userId,
            Latitude = latitude,
            Longitude = longitude,
            Timestamp = DateTime.UtcNow
        };

        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.PostAsJsonAsync(
            "/api/attendance/register", 
            request
        );
        
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AttendanceResponse>();
    }
}
```

## Códigos de Estado HTTP

- **200 OK**: Solicitud exitosa
- **201 Created**: Recurso creado exitosamente
- **204 No Content**: Actualización/eliminación exitosa sin contenido de respuesta
- **400 Bad Request**: Datos inválidos en la solicitud
- **401 Unauthorized**: No autenticado o token inválido
- **403 Forbidden**: No tiene permisos para acceder al recurso
- **404 Not Found**: Recurso no encontrado
- **500 Internal Server Error**: Error en el servidor

## Mensajes de Error Comunes

### "Usuario o contraseña incorrectos"
- Verifica que el username y password sean correctos
- Verifica que el usuario esté activo

### "No hay configuración de geofence para este usuario"
- El usuario necesita tener un geofence asignado por un administrador
- Usa `POST /api/geofence` para crear uno

### "Ubicación fuera del área permitida"
- El usuario está fuera del radio del geofence
- Verifica las coordenadas y el radio configurado

### "Ya registraste tu asistencia el día de hoy"
- Solo se permite una asistencia por día
- Espera al siguiente día para registrar otra asistencia

### "401 Unauthorized"
- El token JWT expiró (validez de 7 días)
- Realiza login nuevamente para obtener un nuevo token

### "403 Forbidden"
- Intentas acceder a un endpoint administrativo sin ser administrador
- Verifica el rol del usuario

## Tips de Desarrollo

### Obtener tu IP Local

```powershell
# Windows PowerShell
Get-NetIPAddress -AddressFamily IPv4 | Where-Object {$_.InterfaceAlias -notmatch "Loopback"}

# Windows CMD
ipconfig | findstr IPv4
```

### Permitir Conexiones Externas (Windows Firewall)

```powershell
# Agregar regla de firewall
New-NetFirewallRule -DisplayName "Checador API" -Direction Inbound -LocalPort 5000,5001 -Protocol TCP -Action Allow
```

### Ejecutar en una IP Específica

Modifica `Properties/launchSettings.json`:

```json
"applicationUrl": "https://0.0.0.0:5001;http://0.0.0.0:5000"
```

O usa la línea de comandos:

```bash
dotnet run --urls "http://0.0.0.0:5000;https://0.0.0.0:5001"
```

## Seguridad en Producción

### 1. Cambiar la Clave JWT
```json
"Jwt": {
  "Key": "TuClaveSecretaMuyLargaYSegura_ConAlMenos64CaracteresParaMayorSeguridad!",
  "Issuer": "CheckadorAPI",
  "Audience": "CheckadorMobileApp"
}
```

### 2. Usar HTTPS en Producción
- Obtener certificado SSL válido
- Configurar redirección HTTPS obligatoria

### 3. Configurar CORS Apropiadamente
```csharp
// En Program.cs - Solo permitir tu dominio
builder.Services.AddCors(options =>
{
    options.AddPolicy("Production", builder =>
    {
        builder.WithOrigins("https://tu-dominio.com")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
```

### 4. Deshabilitar Swagger en Producción
```csharp
// En Program.cs
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```

### 5. Usar Variables de Entorno
```bash
# En producción, no almacenar secretos en appsettings.json
export ConnectionStrings__DefaultConnection="tu_cadena_de_conexion"
export Jwt__Key="tu_clave_secreta"
```

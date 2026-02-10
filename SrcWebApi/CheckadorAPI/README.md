# Checador API

API REST para el sistema de checador de asistencias con geolocalización.

## Características

- **Autenticación JWT**: Sistema seguro de autenticación basado en tokens
- **Roles de Usuario**: Administrador y Usuario con permisos diferenciados
- **Geofencing**: Validación de ubicación para registro de asistencia
- **Entity Framework Core**: ORM para gestión de base de datos
- **SQL Server**: Base de datos relacional
- **Swagger/OpenAPI**: Documentación interactiva de la API

## Tecnologías

- .NET 9.0
- Entity Framework Core 9.0
- SQL Server
- JWT Bearer Authentication
- BCrypt para hash de contraseñas
- Swagger/OpenAPI

## Estructura del Proyecto

```
CheckadorAPI/
??? Controllers/
?   ??? AuthController.cs          # Autenticación y login
?   ??? AttendanceController.cs    # Registro de asistencias
?   ??? UsersController.cs         # Gestión de usuarios (Admin)
?   ??? GeofenceController.cs      # Gestión de geofences (Admin)
??? Data/
?   ??? ApplicationDbContext.cs    # Contexto de Entity Framework
??? DTOs/
?   ??? LoginRequest.cs
?   ??? LoginResponse.cs
?   ??? AttendanceRequest.cs
?   ??? AttendanceResponse.cs
?   ??? CreateUserRequest.cs
?   ??? UpdateUserRequest.cs
?   ??? CreateGeofenceRequest.cs
?   ??? UserDto.cs
?   ??? GeofenceConfigDto.cs
??? Models/
?   ??? User.cs                    # Modelo de usuario
?   ??? Attendance.cs              # Modelo de asistencia
?   ??? GeofenceConfig.cs          # Modelo de configuración de geofence
??? Helpers/
?   ??? GeofenceHelper.cs          # Cálculo de distancias (Haversine)
?   ??? JwtHelper.cs               # Generación y validación de JWT
??? Program.cs                      # Configuración de la aplicación
??? appsettings.json               # Configuración

```

## Configuración Inicial

### 1. Configurar Cadena de Conexión

Edita `appsettings.json` y ajusta la cadena de conexión según tu configuración:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CheckadorDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

Para SQL Server en red:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=TU_SERVIDOR;Database=CheckadorDB;User Id=tu_usuario;Password=tu_password;TrustServerCertificate=True"
}
```

### 2. Aplicar Migraciones

```bash
cd CheckadorAPI
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 3. Ejecutar la Aplicación

```bash
dotnet run
```

La API estará disponible en:
- HTTPS: https://localhost:5001
- HTTP: http://localhost:5000
- Swagger: https://localhost:5001/swagger

## Usuarios por Defecto

La base de datos incluye usuarios iniciales:

### Administrador
- **Usuario**: `admin`
- **Contraseña**: `Admin123!`
- **Rol**: Administrador

### Usuario Normal
- **Usuario**: `usuario1`
- **Contraseña**: `User123!`
- **Rol**: Usuario
- **Geofence**: Oficina Central (Zócalo CDMX)
  - Latitud: 19.432608
  - Longitud: -99.133209
  - Radio: 200m

## Endpoints

### Autenticación

#### POST /api/auth/login
Inicia sesión y obtiene un token JWT.

**Request:**
```json
{
  "username": "admin",
  "password": "Admin123!"
}
```

**Response:**
```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "message": "Login exitoso",
  "user": {
    "id": 1,
    "username": "admin",
    "fullName": "Administrador del Sistema",
    "email": "admin@checador.com",
    "role": "Administrador",
    "isActive": true
  },
  "geofenceConfig": null
}
```

### Asistencias

#### POST /api/attendance/register
Registra una asistencia (requiere autenticación).

**Headers:**
```
Authorization: Bearer {token}
```

**Request:**
```json
{
  "userId": 2,
  "latitude": 19.432608,
  "longitude": -99.133209,
  "timestamp": "2024-02-10T14:30:00Z"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Asistencia registrada exitosamente",
  "attendanceId": 1
}
```

#### GET /api/attendance/user/{userId}
Obtiene las asistencias de un usuario (requiere autenticación).

#### GET /api/attendance/today
Obtiene las asistencias del día (solo Administrador).

### Usuarios

#### GET /api/users
Lista todos los usuarios (solo Administrador).

#### GET /api/users/{id}
Obtiene un usuario específico (requiere autenticación).

#### POST /api/users
Crea un nuevo usuario (solo Administrador).

**Request:**
```json
{
  "username": "nuevo.usuario",
  "password": "Password123!",
  "fullName": "Nuevo Usuario",
  "email": "nuevo@example.com",
  "role": "Usuario"
}
```

#### PUT /api/users/{id}
Actualiza un usuario (solo Administrador).

**Request:**
```json
{
  "fullName": "Nombre Actualizado",
  "email": "nuevo.email@example.com",
  "password": "NuevaPassword123!",
  "isActive": true,
  "role": "Usuario"
}
```

#### DELETE /api/users/{id}
Elimina un usuario (solo Administrador).

### Geofences

#### GET /api/geofence
Lista todos los geofences (solo Administrador).

#### GET /api/geofence/user/{userId}
Obtiene el geofence de un usuario (solo Administrador).

#### POST /api/geofence
Crea un nuevo geofence (solo Administrador).

**Request:**
```json
{
  "userId": 2,
  "centerLatitude": 19.432608,
  "centerLongitude": -99.133209,
  "radiusInMeters": 200,
  "locationName": "Oficina Central"
}
```

#### PUT /api/geofence/{id}
Actualiza un geofence (solo Administrador).

#### DELETE /api/geofence/{id}
Elimina un geofence (solo Administrador).

## Seguridad

- Las contraseñas se almacenan usando BCrypt hash
- Autenticación basada en JWT con expiración de 7 días
- Los endpoints protegidos requieren autenticación
- Los endpoints administrativos requieren rol de Administrador
- CORS configurado para permitir solicitudes desde la app móvil

## Validaciones de Asistencia

El sistema valida:
1. **Autenticación**: Usuario debe estar autenticado
2. **Usuario activo**: Usuario debe estar activo
3. **Geofence configurado**: Usuario debe tener un geofence asignado
4. **Ubicación válida**: La ubicación debe estar dentro del radio permitido
5. **Una asistencia por día**: No se puede registrar más de una vez al día

## Algoritmo de Geofencing

El sistema utiliza la **fórmula de Haversine** para calcular la distancia entre dos puntos geográficos:

```csharp
public static double CalculateDistance(
    double lat1, double lon1,
    double lat2, double lon2)
{
    var dLat = ToRadians(lat2 - lat1);
    var dLon = ToRadians(lon2 - lon1);

    var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

    var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

    return EarthRadiusKm * c * 1000; // Retorna en metros
}
```

## Configuración de la App MAUI

Para conectar la aplicación MAUI con esta API, actualiza la URL base en el servicio de API:

```csharp
// Para Android Emulator:
var baseUrl = "http://10.0.2.2:5000";

// Para dispositivo físico (asegúrate de usar tu IP local):
var baseUrl = "http://192.168.1.XXX:5000";

// Para iOS Simulator:
var baseUrl = "http://localhost:5000";
```

## Desarrollo

### Agregar una Nueva Migración

```bash
dotnet ef migrations add NombreDeLaMigracion
dotnet ef database update
```

### Revertir una Migración

```bash
dotnet ef database update NombreMigracionAnterior
dotnet ef migrations remove
```

### Ver Migraciones Aplicadas

```bash
dotnet ef migrations list
```

## Producción

Para desplegar en producción:

1. **Actualizar appsettings.json**:
   - Cambiar la cadena de conexión
   - Usar una clave JWT fuerte y secreta
   - Deshabilitar Swagger en producción

2. **Configurar HTTPS**:
   - Obtener un certificado SSL válido
   - Configurar redirección HTTPS

3. **Configurar CORS**:
   - Restringir orígenes permitidos
   - No usar "AllowAll" en producción

4. **Logging**:
   - Configurar un sistema de logging apropiado
   - Monitorear errores y excepciones

## Solución de Problemas

### Error: "Cannot open database"
- Verifica que SQL Server esté corriendo
- Verifica la cadena de conexión
- Ejecuta las migraciones: `dotnet ef database update`

### Error: "Unable to connect to database"
- Verifica el servidor de base de datos
- Verifica las credenciales
- Verifica el firewall

### Error 401 Unauthorized
- Verifica que el token JWT esté incluido en el header
- Verifica que el token no haya expirado
- Formato correcto: `Authorization: Bearer {token}`

### Error 403 Forbidden
- Verifica que el usuario tenga el rol correcto
- Los endpoints administrativos requieren rol "Administrador"

## Soporte

Para reportar problemas o solicitar características, contacta al equipo de desarrollo.

## Licencia

Este proyecto es privado y confidencial.

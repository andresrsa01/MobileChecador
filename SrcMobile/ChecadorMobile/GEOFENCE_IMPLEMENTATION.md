# Implementación de Validación de Geofence

## Descripción General

La validación de geofence ahora se obtiene desde el backend durante el proceso de login y se almacena localmente en SQLite. Esto permite:
- ? Reducir llamadas a la API
- ? Validación offline del geofence
- ? Mejor rendimiento en el registro de asistencia
- ? Sincronización automática al iniciar sesión

## Archivos Creados

### 1. **Models\GeofenceConfig.cs**
Define la configuración del geofence almacenada en SQLite:
- `Id`: Identificador único (PrimaryKey, AutoIncrement)
- `UserId`: ID del usuario al que pertenece el geofence
- `CenterLatitude`: Latitud del centro del geofence
- `CenterLongitude`: Longitud del centro del geofence
- `RadiusInMeters`: Radio del área permitida en metros
- `LocationName`: Nombre descriptivo de la ubicación
- `UpdatedAt`: Fecha de última actualización

### 2. **Helpers\GeofenceHelper.cs**
Utilidades para cálculos de geofence:
- `CalculateDistance()`: Calcula la distancia entre dos puntos GPS usando la fórmula de Haversine
- `IsWithinGeofence()`: Verifica si una ubicación está dentro del radio permitido

## Archivos Modificados

### 1. **Models\LoginResponse.cs**
Agregado campo `GeofenceConfig?` para recibir el geofence desde el backend al hacer login.

### 2. **Services\IDatabaseService.cs**
Agregados métodos para gestionar geofence en SQLite:
```csharp
Task<int> SaveGeofenceConfigAsync(GeofenceConfig geofenceConfig);
Task<GeofenceConfig?> GetGeofenceConfigByUserIdAsync(int userId);
Task<int> DeleteGeofenceConfigByUserIdAsync(int userId);
```

### 3. **Services\DatabaseService.cs**
- Creación de tabla `GeofenceConfig` en el método `InitAsync()`
- Implementación de métodos para CRUD de geofence

### 4. **Services\IAuthService.cs**
Agregado método:
```csharp
Task<GeofenceConfig?> GetGeofenceConfigAsync();
```

### 5. **Services\AuthService.cs**
- Modificado `LoginAsync()` para guardar el geofence en SQLite cuando viene en la respuesta
- Implementado `GetGeofenceConfigAsync()` para obtener el geofence del usuario actual desde SQLite

### 6. **ViewModels\NewAttendanceViewModel.cs**
Modificado para:
- ? Ya NO consulta el geofence desde la API
- ? Consulta el geofence desde SQLite (almacenado en el login)
- ? Validar que la ubicación actual esté dentro del geofence
- ? Mostrar la distancia si el usuario está fuera del área permitida
- ? Funciona offline una vez que se tiene el geofence almacenado

## Flujo de Validación

### Al Hacer Login:
1. Usuario ingresa credenciales
2. Se envía request al backend con usuario y contraseña
3. El backend responde con `LoginResponse` que incluye:
   - Token de autenticación
   - Datos del usuario
   - **Configuración del geofence (nuevo)**
4. La app guarda el geofence en SQLite
5. Usuario queda autenticado con geofence configurado

### Al Registrar Asistencia:
1. **Obtener ubicación actual** del usuario
2. **Consultar geofence desde SQLite** (no hace llamada al backend)
3. **Calcular distancia** entre la ubicación actual y el centro del geofence
4. **Validar si está dentro del radio** permitido
5. Si está fuera:
   - Mostrar mensaje con la distancia actual
   - Bloquear el registro de asistencia
6. Si está dentro:
   - Continuar con el registro normal

## Requisitos del Backend

### El backend debe modificar el endpoint de login:
```
POST /api/auth/login
```

Para que la respuesta incluya el geofence en el JSON:
```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "message": "Login exitoso",
  "user": {
    "id": 1,
    "username": "jperez",
    "fullName": "Juan Pérez",
    "email": "jperez@ejemplo.com",
    "isActive": true
  },
  "geofenceConfig": {
    "centerLatitude": 19.4326,
    "centerLongitude": -99.1332,
    "radiusInMeters": 100,
    "locationName": "Oficina Principal"
  }
}
```

**Nota:** El campo `geofenceConfig` puede ser `null` si el usuario no tiene un geofence configurado.

### ? Ya NO se necesita el endpoint separado:
```
GET /api/geofence/{userId}  // Este endpoint ya no es necesario
```

## Base de Datos SQLite

### Tabla: GeofenceConfig
```sql
CREATE TABLE GeofenceConfig (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    CenterLatitude REAL NOT NULL,
    CenterLongitude REAL NOT NULL,
    RadiusInMeters REAL NOT NULL,
    LocationName TEXT NOT NULL,
    UpdatedAt DATETIME NOT NULL
);
```

## Configuración Recomendada

- **Radio típico para oficinas**: 50-100 metros
- **Radio para obras/proyectos**: 100-500 metros
- **Precisión GPS**: El GPS puede tener variación de ±10-30 metros

## Mensajes de Error

- **"Ubicación No Válida"**: El usuario está fuera del área permitida
- **"Error de Configuración"**: No hay geofence almacenado (sugiere cerrar sesión y volver a iniciar)

## Ventajas de Esta Implementación

1. ? **Rendimiento**: No hay llamada a la API cada vez que se registra asistencia
2. ? **Offline**: La validación funciona sin conexión a Internet
3. ? **Sincronización**: El geofence se actualiza automáticamente al hacer login
4. ? **Simplicidad**: Un solo endpoint (login) en lugar de dos
5. ? **Consistencia**: El geofence siempre está disponible mientras el usuario esté autenticado

## Próximos Pasos

1. **Reinicia la aplicación** para aplicar todos los cambios
2. **Actualiza el backend** para incluir el geofenceConfig en el LoginResponse
3. Prueba el flujo completo:
   - Login ? debe guardar el geofence en SQLite
   - Registro de asistencia ? debe validar contra el geofence almacenado
   - Logout ? el geofence se mantiene (opcional: implementar limpieza)

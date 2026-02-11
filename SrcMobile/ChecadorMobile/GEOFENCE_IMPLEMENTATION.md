# Implementacion de Validacion de Geofence

## Descripcion General

La validacion de geofence ahora se obtiene desde el backend durante el proceso de login y se almacena localmente en SQLite. Esto permite:
- ? Reducir llamadas a la API
- ? Validacion offline del geofence
- ? Mejor rendimiento en el registro de asistencia
- ? Sincronizacion automatica al iniciar sesion

## Archivos Creados

### 1. **Models\GeofenceConfig.cs**
Define la configuracion del geofence almacenada en SQLite:
- `Id`: Identificador unico (PrimaryKey, AutoIncrement)
- `UserId`: ID del usuario al que pertenece el geofence
- `CenterLatitude`: Latitud del centro del geofence
- `CenterLongitude`: Longitud del centro del geofence
- `RadiusInMeters`: Radio del area permitida en metros
- `LocationName`: Nombre descriptivo de la ubicacion
- `UpdatedAt`: Fecha de ultima actualizacion

### 2. **Helpers\GeofenceHelper.cs**
Utilidades para calculos de geofence:
- `CalculateDistance()`: Calcula la distancia entre dos puntos GPS usando la formula de Haversine
- `IsWithinGeofence()`: Verifica si una ubicacion esta dentro del radio permitido

## Archivos Modificados

### 1. **Models\LoginResponse.cs**
Agregado campo `GeofenceConfig?` para recibir el geofence desde el backend al hacer login.

### 2. **Services\IDatabaseService.cs**
Agregados metodos para gestionar geofence en SQLite:
```csharp
Task<int> SaveGeofenceConfigAsync(GeofenceConfig geofenceConfig);
Task<GeofenceConfig?> GetGeofenceConfigByUserIdAsync(int userId);
Task<int> DeleteGeofenceConfigByUserIdAsync(int userId);
```

### 3. **Services\DatabaseService.cs**
- Creacion de tabla `GeofenceConfig` en el metodo `InitAsync()`
- Implementacion de metodos para CRUD de geofence

### 4. **Services\IAuthService.cs**
Agregado metodo:
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
- ? Validar que la ubicacion actual este dentro del geofence
- ? Mostrar la distancia si el usuario esta fuera del area permitida
- ? Funciona offline una vez que se tiene el geofence almacenado

## Flujo de Validacion

### Al Hacer Login:
1. Usuario ingresa credenciales
2. Se envia request al backend con usuario y contraseña
3. El backend responde con `LoginResponse` que incluye:
   - Token de autenticacion
   - Datos del usuario
   - **Configuracion del geofence (nuevo)**
4. La app guarda el geofence en SQLite
5. Usuario queda autenticado con geofence configurado

### Al Registrar Asistencia:
1. **Obtener ubicacion actual** del usuario
2. **Consultar geofence desde SQLite** (no hace llamada al backend)
3. **Calcular distancia** entre la ubicacion actual y el centro del geofence
4. **Validar si esta dentro del radio** permitido
5. Si esta fuera:
   - Mostrar mensaje con la distancia actual
   - Bloquear el registro de asistencia
6. Si esta dentro:
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
    "fullName": "Juan Perez",
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

## Configuracion Recomendada

- **Radio tipico para oficinas**: 50-100 metros
- **Radio para obras/proyectos**: 100-500 metros
- **Precision GPS**: El GPS puede tener variacion de ±10-30 metros

## Mensajes de Error

- **"Ubicacion No Valida"**: El usuario esta fuera del area permitida
- **"Error de Configuracion"**: No hay geofence almacenado (sugiere cerrar sesion y volver a iniciar)

## Ventajas de Esta Implementacion

1. ? **Rendimiento**: No hay llamada a la API cada vez que se registra asistencia
2. ? **Offline**: La validacion funciona sin conexion a Internet
3. ? **Sincronizacion**: El geofence se actualiza automaticamente al hacer login
4. ? **Simplicidad**: Un solo endpoint (login) en lugar de dos
5. ? **Consistencia**: El geofence siempre esta disponible mientras el usuario este autenticado

## Proximos Pasos

1. **Reinicia la aplicacion** para aplicar todos los cambios
2. **Actualiza el backend** para incluir el geofenceConfig en el LoginResponse
3. Prueba el flujo completo:
   - Login ? debe guardar el geofence en SQLite
   - Registro de asistencia ? debe validar contra el geofence almacenado
   - Logout ? el geofence se mantiene (opcional: implementar limpieza)

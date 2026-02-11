# Cambio: Eliminación del Campo LocationName

## Resumen
Se eliminó el campo `LocationName` de la entidad `GeofenceConfig` y todos sus DTOs relacionados, simplificando la estructura de datos.

---

## Archivos Modificados

### 1. Modelos
- **`Models/GeofenceConfig.cs`**
  - ? Eliminado: `LocationName` (string)

### 2. DTOs
- **`DTOs/GeofenceConfigDto.cs`**
  - ? Eliminado: `LocationName` (string)

- **`DTOs/CreateGeofenceRequest.cs`**
  - ? Eliminado: `LocationName` (string, Required)

- **`DTOs/CreateWorkplaceRequest.cs`**
  - ? Eliminado: `LocationName` (string, Required)

- **`DTOs/UpdateWorkplaceRequest.cs`**
  - ? Eliminado: `LocationName` (string?, Optional)

### 3. Controladores
- **`Controllers/WorkplaceController.cs`**
  - ? Eliminadas todas las referencias a `LocationName` en:
    - `GetWorkplaces()` - Response DTO
    - `GetWorkplace(id)` - Response DTO
    - `CreateWorkplace()` - Creación del geofence y Response DTO
    - `UpdateWorkplace(id)` - Lógica de actualización y Response DTO

- **`Controllers/GeofenceController.cs`**
  - ? Eliminadas todas las referencias a `LocationName` en:
    - `GetGeofences()` - Response DTO
    - `GetGeofenceByWorkplaceId(workplaceId)` - Response DTO
    - `GetGeofence(id)` - Response DTO

- **`Controllers/AuthController.cs`**
  - ? Eliminada referencia a `LocationName` en:
    - `Login()` - Response DTO del geofence

### 4. Base de Datos
- **`Data/ApplicationDbContext.cs`**
  - ? Eliminado: `LocationName` del seed data

- **Migración**: `20260211060726_RemoveLocationNameFromGeofence`
  - ? Columna `LocationName` eliminada de la tabla `GeofenceConfigs`

---

## Estructura Actual de GeofenceConfig

### Modelo
```csharp
public class GeofenceConfig
{
    public int Id { get; set; }
    public int WorkplaceId { get; set; }
    public double CenterLatitude { get; set; }
    public double CenterLongitude { get; set; }
    public double RadiusInMeters { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public virtual Workplace Workplace { get; set; }
}
```

### DTO
```csharp
public class GeofenceConfigDto
{
    public int Id { get; set; }
    public double CenterLatitude { get; set; }
    public double CenterLongitude { get; set; }
    public double RadiusInMeters { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

---

## Ejemplo Actualizado: Crear Workplace

### Antes (con LocationName):
```json
POST /api/Workplace
{
  "name": "Oficina Sur",
  "address": "Av. Insurgentes Sur 1234",
  "phone": "5555551234",
  "zip": "03100",
  "centerLatitude": 19.368425,
  "centerLongitude": -99.165000,
  "radiusInMeters": 150,
  "locationName": "Oficina Sur - Área de Registro"
}
```

### Ahora (sin LocationName):
```json
POST /api/Workplace
{
  "name": "Oficina Sur",
  "address": "Av. Insurgentes Sur 1234",
  "phone": "5555551234",
  "zip": "03100",
  "centerLatitude": 19.368425,
  "centerLongitude": -99.165000,
  "radiusInMeters": 150
}
```

---

## Respuesta de Login Actualizada

### Antes:
```json
{
  "success": true,
  "token": "...",
  "user": { ... },
  "geofenceConfig": {
    "id": 1,
    "centerLatitude": 19.432608,
    "centerLongitude": -99.133209,
    "radiusInMeters": 200,
    "locationName": "Oficina Central",
    "updatedAt": "2024-01-01T00:00:00Z"
  }
}
```

### Ahora:
```json
{
  "success": true,
  "token": "...",
  "user": { ... },
  "geofenceConfig": {
    "id": 1,
    "centerLatitude": 19.432608,
    "centerLongitude": -99.133209,
    "radiusInMeters": 200,
    "updatedAt": "2024-01-01T00:00:00Z"
  }
}
```

---

## Validaciones Actualizadas

### CreateWorkplaceRequest
```csharp
// Campos obligatorios:
- Name (string, max 200)
- Address (string, max 300)
- Phone (string, max 20)
- Zip (string, max 10)
- CenterLatitude (double, -90 a 90)
- CenterLongitude (double, -180 a 180)
- RadiusInMeters (double, 10 a 10000)
// ? LocationName - ELIMINADO
```

### UpdateWorkplaceRequest
```csharp
// Todos los campos opcionales:
- Name (string?)
- Address (string?)
- Phone (string?)
- Zip (string?)
- IsActive (bool?)
- CenterLatitude (double?)
- CenterLongitude (double?)
- RadiusInMeters (double?)
// ? LocationName - ELIMINADO
```

---

## Estado de la Base de Datos

? **Migración aplicada exitosamente**: `20260211060726_RemoveLocationNameFromGeofence`
- Columna `LocationName` eliminada de la tabla `GeofenceConfigs`
- Datos existentes no se vieron afectados (excepto la eliminación de la columna)

---

## Impacto

### Ventajas de la Eliminación:
1. **Simplicidad**: Menos campos para gestionar y validar
2. **Claridad**: El nombre del workplace ya identifica la ubicación
3. **Menos redundancia**: Evita duplicidad de información (el Workplace ya tiene Name y Address)
4. **API más limpia**: Menos campos en los requests y responses

### Cambios Requeridos en Clientes:
- ? Eliminar el campo `locationName` de los formularios de creación de workplace
- ? Eliminar el campo `locationName` de los formularios de actualización
- ? Eliminar la visualización de `locationName` en las respuestas de geofence
- ? Usar `workplace.name` en lugar de `geofence.locationName` para identificar la ubicación

---

## Verificación

### Compilación
? Build exitoso sin errores

### Migración
? Migración aplicada exitosamente en la base de datos

### Endpoints Actualizados
- ? `POST /api/Workplace` - Ya no requiere locationName
- ? `PUT /api/Workplace/{id}` - Ya no acepta locationName
- ? `GET /api/Workplace` - Ya no devuelve locationName en geofenceConfig
- ? `GET /api/Workplace/{id}` - Ya no devuelve locationName en geofenceConfig
- ? `GET /api/Geofence/workplace/{id}` - Ya no devuelve locationName
- ? `POST /api/Auth/login` - Ya no devuelve locationName en geofenceConfig

---

## Notas

- Los datos existentes en la base de datos siguen intactos (excepto la columna LocationName que se eliminó)
- Los workplaces existentes continúan funcionando normalmente
- Los geofences existentes mantienen su funcionalidad completa
- La aplicación móvil necesitará actualizarse para dejar de usar `locationName`

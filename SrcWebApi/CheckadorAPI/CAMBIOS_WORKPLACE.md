# Cambios Realizados: Implementación de Workplace

## Resumen
Se ha reestructurado la arquitectura de la aplicación para introducir el concepto de **Workplace** (Lugar de trabajo), modificando las relaciones entre entidades.

## Cambios en el Modelo de Datos

### Nueva Entidad: Workplace
**Archivo**: `Models/Workplace.cs`

Se creó la entidad `Workplace` con los siguientes atributos:
- `Id` (int): Identificador único
- `Name` (string): Nombre del lugar de trabajo
- `Address` (string): Dirección
- `Phone` (string): Teléfono de contacto
- `Zip` (string): Código postal
- `CreatedAt` (DateTime): Fecha de creación
- `IsActive` (bool): Estado activo/inactivo

**Relaciones**:
- Un Workplace puede tener múltiples usuarios (`Users`)
- Un Workplace tiene una configuración de geofence (`GeofenceConfig`) - Relación 1:1

### Modificaciones en User
**Archivo**: `Models/User.cs`

**Cambios**:
- Se agregó el campo `WorkplaceId` (int?) - Nullable para permitir administradores sin workplace
- Se agregó la navegación `Workplace` 
- Se eliminó la relación directa con `GeofenceConfig`

**Reglas de negocio**:
- Los usuarios con rol "Usuario" DEBEN tener un `WorkplaceId` asignado
- Los usuarios con rol "Administrador" NO deben tener `WorkplaceId` (debe ser null)
- Un usuario solo puede estar vinculado a un workplace

### Modificaciones en GeofenceConfig
**Archivo**: `Models/GeofenceConfig.cs`

**Cambios**:
- Se reemplazó `UserId` por `WorkplaceId`
- La relación ahora es con `Workplace` en lugar de `User`
- Ahora cada workplace tiene su propia configuración de geofence

## DTOs Creados

### WorkplaceDto
**Archivo**: `DTOs/WorkplaceDto.cs`
- DTO para representar un Workplace con sus datos y GeofenceConfig opcional

### CreateWorkplaceRequest
**Archivo**: `DTOs/CreateWorkplaceRequest.cs`
- DTO para crear nuevos workplaces con validaciones
- **INCLUYE** campos obligatorios para crear el GeofenceConfig:
  - `CenterLatitude` (double): Latitud del centro del geofence
  - `CenterLongitude` (double): Longitud del centro del geofence
  - `RadiusInMeters` (double): Radio en metros del geofence
  - `LocationName` (string): Nombre de la ubicación del geofence

### UpdateWorkplaceRequest
**Archivo**: `DTOs/UpdateWorkplaceRequest.cs`
- DTO para actualizar workplaces existentes (todos los campos opcionales)
- **INCLUYE** campos opcionales para actualizar el GeofenceConfig:
  - `CenterLatitude` (double?): Nueva latitud del centro
  - `CenterLongitude` (double?): Nueva longitud del centro
  - `RadiusInMeters` (double?): Nuevo radio en metros
  - `LocationName` (string?): Nuevo nombre de la ubicación


### Modificaciones en DTOs Existentes

#### UserDto
- Se agregó `WorkplaceId` (int?)
- Se agregó `WorkplaceName` (string?)

#### GeofenceConfigDto
- Se eliminó `UserId` (ya que ahora pertenece a Workplace)

#### CreateGeofenceRequest
- Se reemplazó `UserId` por `WorkplaceId`

#### CreateUserRequest
- Se agregó `WorkplaceId` (int?) con validación según el rol

#### UpdateUserRequest
- Se agregó `WorkplaceId` (int?) para poder actualizar el workplace de un usuario

## Controladores

### Nuevo: WorkplaceController
**Archivo**: `Controllers/WorkplaceController.cs`

**Endpoints disponibles**:

1. **GET /api/Workplace**
   - Obtiene todos los workplaces con sus geofences
   - Query param: `includeInactive` (bool) - Para incluir workplaces inactivos
   - Requiere rol: Administrador

2. **GET /api/Workplace/{id}**
   - Obtiene un workplace por ID con su geofence
   - Requiere rol: Administrador

3. **POST /api/Workplace**
   - Crea un nuevo workplace **junto con su geofence** (ambos son obligatorios)
   - Body: `CreateWorkplaceRequest` (incluye datos de workplace y geofence)
   - Requiere rol: Administrador

4. **PUT /api/Workplace/{id}**
   - Actualiza un workplace existente **y opcionalmente su geofence**
   - Body: `UpdateWorkplaceRequest` (todos los campos opcionales)
   - Requiere rol: Administrador

5. **DELETE /api/Workplace/{id}**
   - Desactiva un workplace (soft delete) y su geofence
   - No permite eliminar si tiene usuarios asignados
   - Requiere rol: Administrador

6. **GET /api/Workplace/{id}/users**
   - Obtiene los usuarios de un workplace
   - Requiere rol: Administrador

**Cambios importantes**:
- ? **Al crear un workplace, se crea automáticamente su geofence** (no pueden existir por separado)
- ? Al actualizar un workplace, se puede actualizar también su geofence
- ? Al desactivar un workplace, el geofence se mantiene vinculado


### Modificaciones en GeofenceController
**Archivo**: `Controllers/GeofenceController.cs`

**Cambios**:
- ? **Se eliminaron los endpoints de creación, actualización y eliminación** (ahora se hace a través de WorkplaceController)
- ? Se mantienen solo los endpoints de consulta:
  - `GET /api/Geofence` - Listar todos los geofences
  - `GET /api/Geofence/{id}` - Obtener un geofence por ID
  - `GET /api/Geofence/workplace/{workplaceId}` - Obtener geofence por workplace ID
- ? Se actualizó la documentación indicando que la gestión se hace desde WorkplaceController


### Modificaciones en UsersController
**Archivo**: `Controllers/UsersController.cs`

**Cambios**:
- Se agregó inclusión de `Workplace` en las consultas
- Se agregaron campos `WorkplaceId` y `WorkplaceName` en los DTOs de respuesta
- Se agregaron validaciones en `CreateUser`:
  - Los usuarios con rol "Usuario" deben tener un workplace asignado
  - Los administradores no pueden tener workplace asignado
  - Verificación de existencia del workplace
- Se agregaron validaciones en `UpdateUser`:
  - Si se cambia el rol a "Administrador", se quita el workplace
  - Verificación de workplace válido al actualizar
- Se mejoraron los mensajes de error

### Modificaciones en AuthController
**Archivo**: `Controllers/AuthController.cs`

**Cambios**:
- Se actualizó el `Include` para cargar `Workplace` y su `GeofenceConfig`
- Se modificó el `LoginResponse` para incluir el geofence del workplace en lugar del usuario
- Se agregaron campos `WorkplaceId` y `WorkplaceName` en el UserDto de respuesta

### Modificaciones en AttendanceController
**Archivo**: `Controllers/AttendanceController.cs`

**Cambios**:
- Se actualizó para obtener la configuración de geofence desde el workplace del usuario
- Se agregó validación para usuarios sin workplace asignado
- Se actualizaron los mensajes de log y error

## Modificaciones en ApplicationDbContext
**Archivo**: `Data/ApplicationDbContext.cs`

**Cambios**:
- Se agregó `DbSet<Workplace>` 
- Se actualizaron las relaciones en `OnModelCreating`:
  - GeofenceConfig ahora tiene relación 1:1 con Workplace
  - User tiene relación N:1 con Workplace (OnDelete.Restrict)
- Se actualizó el `SeedData`:
  - Se crea un workplace de ejemplo
  - El administrador no tiene workplace (WorkplaceId = null)
  - El usuario de prueba tiene workplace asignado (WorkplaceId = 1)
  - El geofence ahora pertenece al workplace en lugar del usuario

## Migración de Base de Datos

**Migración creada**: `InitialCreateWithWorkplace`

**Cambios en la base de datos**:
1. Se creó la tabla `Workplaces`
2. Se modificó la tabla `Users` para agregar `WorkplaceId` (nullable)
3. Se modificó la tabla `GeofenceConfigs` para cambiar `UserId` por `WorkplaceId`
4. Se actualizaron los índices correspondientes
5. Se actualizaron las claves foráneas

**Datos iniciales (seed)**:
- 1 Workplace: "Oficina Central"
- 1 Administrador sin workplace
- 1 Usuario vinculado al workplace
- 1 GeofenceConfig vinculado al workplace

## Reglas de Negocio Implementadas

1. **Workplaces y Usuarios**:
   - Un workplace puede tener N usuarios
   - Un usuario solo puede estar en 1 workplace
   - Los administradores no pueden tener workplace asignado
   - Los usuarios regulares DEBEN tener un workplace asignado

2. **Workplaces y Geofence**:
   - Un workplace tiene 1 configuración de geofence
   - Un geofence pertenece a 1 workplace
   - No se puede eliminar un workplace si tiene usuarios asignados

3. **Asistencia**:
   - Los usuarios registran asistencia basándose en el geofence de su workplace
   - Un usuario sin workplace no puede registrar asistencia
   - Un workplace sin geofence configurado no permite registro de asistencia

## Validaciones Agregadas

- Validación de rol al crear/actualizar usuarios
- Validación de existencia de workplace al asignar usuarios
- Validación de workplace activo
- Validación de que administradores no tengan workplace
- Validación de que usuarios regulares tengan workplace
- Validación de workplace con usuarios antes de eliminar

## Testing Recomendado

1. **Crear Workplace con Geofence**:
   ```json
   POST /api/Workplace
   {
     "name": "Oficina Norte",
     "address": "Calle Principal 456",
     "phone": "5555555678",
     "zip": "02000",
     "centerLatitude": 19.432608,
     "centerLongitude": -99.133209,
     "radiusInMeters": 200,
     "locationName": "Oficina Norte - Zona Segura"
   }
   ```

2. **Actualizar Workplace y Geofence**:
   ```json
   PUT /api/Workplace/1
   {
     "name": "Oficina Norte - Actualizada",
     "phone": "5555559999",
     "centerLatitude": 19.432700,
     "radiusInMeters": 250
   }
   ```

3. **Crear Usuario con Workplace**:
   ```json
   POST /api/Users
   {
     "username": "usuario2",
     "password": "User123!",
     "fullName": "Juan Pérez",
     "email": "juan@checador.com",
     "role": "Usuario",
     "workplaceId": 1
   }
   ```

4. **Consultar Geofence por Workplace**:
   ```
   GET /api/Geofence/workplace/1
   ```


## Notas Importantes

- Se eliminó la base de datos anterior y se recreó con la nueva estructura
- Las migraciones anteriores fueron eliminadas
- **IMPORTANTE**: Un Workplace no puede existir sin su GeofenceConfig. Al crear un workplace, automáticamente se crea su geofence
- El GeofenceController ahora es solo para consultas. La gestión (crear/actualizar) se hace desde WorkplaceController
- Se recomienda actualizar la documentación de la API
- Se recomienda actualizar los tests unitarios y de integración
- La aplicación móvil necesitará actualizarse para trabajar con la nueva estructura
- Al crear un workplace, es obligatorio proporcionar todos los datos del geofence en el mismo request


# Ejemplos de Uso - API Workplace con Geofence

## Descripción
Este documento contiene ejemplos prácticos de cómo usar la API de Workplace que ahora integra la creación y gestión de GeofenceConfig.

---

## 1. Crear un Workplace con Geofence

**Endpoint**: `POST /api/Workplace`  
**Rol requerido**: Administrador  
**Descripción**: Crea un nuevo workplace junto con su configuración de geofence. Ambos se crean en una sola operación.

### Request:
```json
{
  "name": "Oficina Sur",
  "address": "Av. Insurgentes Sur 1234, Col. Del Valle",
  "phone": "5555551234",
  "zip": "03100",
  "centerLatitude": 19.368425,
  "centerLongitude": -99.165000,
  "radiusInMeters": 150,
  "locationName": "Oficina Sur - Área de Registro"
}
```

### Response (201 Created):
```json
{
  "id": 2,
  "name": "Oficina Sur",
  "address": "Av. Insurgentes Sur 1234, Col. Del Valle",
  "phone": "5555551234",
  "zip": "03100",
  "createdAt": "2024-02-11T10:30:00Z",
  "isActive": true,
  "geofenceConfig": {
    "id": 2,
    "centerLatitude": 19.368425,
    "centerLongitude": -99.165000,
    "radiusInMeters": 150,
    "locationName": "Oficina Sur - Área de Registro",
    "updatedAt": "2024-02-11T10:30:00Z"
  }
}
```

---

## 2. Actualizar un Workplace

**Endpoint**: `PUT /api/Workplace/{id}`  
**Rol requerido**: Administrador  
**Descripción**: Actualiza la información del workplace y/o su geofence. Todos los campos son opcionales.

### Ejemplo 1: Actualizar solo información del Workplace

```json
PUT /api/Workplace/2
{
  "name": "Oficina Sur - Renovada",
  "phone": "5555559999"
}
```

### Ejemplo 2: Actualizar solo el Geofence

```json
PUT /api/Workplace/2
{
  "centerLatitude": 19.368500,
  "centerLongitude": -99.165100,
  "radiusInMeters": 200
}
```

### Ejemplo 3: Actualizar ambos

```json
PUT /api/Workplace/2
{
  "name": "Oficina Sur - Renovada",
  "address": "Av. Insurgentes Sur 1234-B, Col. Del Valle",
  "phone": "5555559999",
  "centerLatitude": 19.368500,
  "centerLongitude": -99.165100,
  "radiusInMeters": 200,
  "locationName": "Oficina Sur - Nueva Zona"
}
```

### Response (200 OK):
```json
{
  "id": 2,
  "name": "Oficina Sur - Renovada",
  "address": "Av. Insurgentes Sur 1234-B, Col. Del Valle",
  "phone": "5555559999",
  "zip": "03100",
  "createdAt": "2024-02-11T10:30:00Z",
  "isActive": true,
  "geofenceConfig": {
    "id": 2,
    "centerLatitude": 19.368500,
    "centerLongitude": -99.165100,
    "radiusInMeters": 200,
    "locationName": "Oficina Sur - Nueva Zona",
    "updatedAt": "2024-02-11T11:45:00Z"
  }
}
```

---

## 3. Obtener todos los Workplaces

**Endpoint**: `GET /api/Workplace`  
**Rol requerido**: Administrador

### Request:
```
GET /api/Workplace
```

### Response (200 OK):
```json
[
  {
    "id": 1,
    "name": "Oficina Central",
    "address": "Av. Principal 123, Col. Centro",
    "phone": "5555551234",
    "zip": "01000",
    "createdAt": "2024-01-01T00:00:00Z",
    "isActive": true,
    "geofenceConfig": {
      "id": 1,
      "centerLatitude": 19.432608,
      "centerLongitude": -99.133209,
      "radiusInMeters": 200,
      "locationName": "Oficina Central",
      "updatedAt": "2024-01-01T00:00:00Z"
    }
  },
  {
    "id": 2,
    "name": "Oficina Sur",
    "address": "Av. Insurgentes Sur 1234, Col. Del Valle",
    "phone": "5555551234",
    "zip": "03100",
    "createdAt": "2024-02-11T10:30:00Z",
    "isActive": true,
    "geofenceConfig": {
      "id": 2,
      "centerLatitude": 19.368425,
      "centerLongitude": -99.165000,
      "radiusInMeters": 150,
      "locationName": "Oficina Sur - Área de Registro",
      "updatedAt": "2024-02-11T10:30:00Z"
    }
  }
]
```

### Con parámetro includeInactive:
```
GET /api/Workplace?includeInactive=true
```

---

## 4. Obtener un Workplace específico

**Endpoint**: `GET /api/Workplace/{id}`  
**Rol requerido**: Administrador

### Request:
```
GET /api/Workplace/1
```

### Response (200 OK):
```json
{
  "id": 1,
  "name": "Oficina Central",
  "address": "Av. Principal 123, Col. Centro",
  "phone": "5555551234",
  "zip": "01000",
  "createdAt": "2024-01-01T00:00:00Z",
  "isActive": true,
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

---

## 5. Obtener usuarios de un Workplace

**Endpoint**: `GET /api/Workplace/{id}/users`  
**Rol requerido**: Administrador

### Request:
```
GET /api/Workplace/1/users
```

### Response (200 OK):
```json
[
  {
    "id": 2,
    "username": "usuario1",
    "fullName": "Usuario de Prueba",
    "email": "usuario1@checador.com",
    "role": "Usuario",
    "workplaceId": 1,
    "workplaceName": "Oficina Central",
    "isActive": true,
    "createdAt": "2024-01-01T00:00:00Z",
    "lastLogin": "2024-02-11T09:00:00Z"
  },
  {
    "id": 3,
    "username": "usuario2",
    "fullName": "Juan Pérez",
    "email": "juan@checador.com",
    "role": "Usuario",
    "workplaceId": 1,
    "workplaceName": "Oficina Central",
    "isActive": true,
    "createdAt": "2024-02-10T00:00:00Z",
    "lastLogin": null
  }
]
```

---

## 6. Desactivar un Workplace

**Endpoint**: `DELETE /api/Workplace/{id}`  
**Rol requerido**: Administrador  
**Descripción**: Desactiva un workplace (soft delete). No elimina el registro de la base de datos.

### Request:
```
DELETE /api/Workplace/2
```

### Response (204 No Content):
Sin contenido en el body.

### Response en caso de error (400 Bad Request):
```json
{
  "message": "No se puede eliminar el workplace porque tiene usuarios asignados"
}
```

---

## 7. Consultar solo el Geofence

Si solo necesitas consultar la información del geofence, puedes usar los endpoints del GeofenceController:

### Por Workplace ID:
```
GET /api/Geofence/workplace/1
```

### Response (200 OK):
```json
{
  "id": 1,
  "centerLatitude": 19.432608,
  "centerLongitude": -99.133209,
  "radiusInMeters": 200,
  "locationName": "Oficina Central",
  "updatedAt": "2024-01-01T00:00:00Z"
}
```

### Por Geofence ID:
```
GET /api/Geofence/1
```

---

## 8. Crear Usuario vinculado a Workplace

**Endpoint**: `POST /api/Users`  
**Rol requerido**: Administrador  
**Importante**: Los usuarios regulares DEBEN tener un workplace asignado.

### Request:
```json
{
  "username": "maria.lopez",
  "password": "Maria123!",
  "fullName": "María López García",
  "email": "maria.lopez@checador.com",
  "role": "Usuario",
  "workplaceId": 1
}
```

### Response (201 Created):
```json
{
  "id": 4,
  "username": "maria.lopez",
  "fullName": "María López García",
  "email": "maria.lopez@checador.com",
  "role": "Usuario",
  "workplaceId": 1,
  "workplaceName": "Oficina Central",
  "isActive": true,
  "createdAt": "2024-02-11T12:00:00Z",
  "lastLogin": null
}
```

---

## 9. Flujo Completo: Configurar una Nueva Oficina

### Paso 1: Crear el Workplace con Geofence
```json
POST /api/Workplace
{
  "name": "Oficina Norte",
  "address": "Av. Reforma 500, Col. Polanco",
  "phone": "5555552000",
  "zip": "11560",
  "centerLatitude": 19.433333,
  "centerLongitude": -99.191667,
  "radiusInMeters": 180,
  "locationName": "Oficina Norte - Entrada Principal"
}
```

### Paso 2: Crear usuarios para esa oficina
```json
POST /api/Users
{
  "username": "carlos.mendez",
  "password": "Carlos123!",
  "fullName": "Carlos Méndez",
  "email": "carlos@checador.com",
  "role": "Usuario",
  "workplaceId": 3
}
```

### Paso 3: Los usuarios ahora pueden hacer login y registrar asistencia
```json
POST /api/Auth/login
{
  "username": "carlos.mendez",
  "password": "Carlos123!"
}
```

La respuesta del login incluirá el geofence del workplace:
```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "message": "Login exitoso",
  "user": {
    "id": 5,
    "username": "carlos.mendez",
    "fullName": "Carlos Méndez",
    "email": "carlos@checador.com",
    "role": "Usuario",
    "workplaceId": 3,
    "workplaceName": "Oficina Norte",
    "isActive": true,
    "createdAt": "2024-02-11T13:00:00Z",
    "lastLogin": "2024-02-11T13:05:00Z"
  },
  "geofenceConfig": {
    "id": 3,
    "centerLatitude": 19.433333,
    "centerLongitude": -99.191667,
    "radiusInMeters": 180,
    "locationName": "Oficina Norte - Entrada Principal",
    "updatedAt": "2024-02-11T12:55:00Z"
  }
}
```

---

## Validaciones y Reglas de Negocio

### Al crear un Workplace:
- ? Todos los campos del workplace son obligatorios
- ? Todos los campos del geofence son obligatorios
- ? Se crean ambas entidades en una sola transacción

### Al actualizar un Workplace:
- ? Todos los campos son opcionales
- ? Se pueden actualizar campos del workplace sin tocar el geofence
- ? Se pueden actualizar campos del geofence sin tocar el workplace
- ? Se puede actualizar ambos en un solo request

### Al eliminar un Workplace:
- ? No se puede eliminar si tiene usuarios asignados
- ? Al desactivar, solo se marca como `isActive = false`
- ? El geofence permanece vinculado

### Usuarios y Workplaces:
- ? Los usuarios con rol "Usuario" DEBEN tener workplace
- ? Los administradores NO pueden tener workplace
- ? Un usuario solo puede estar en un workplace a la vez

---

## Errores Comunes

### Error: Campos del geofence faltantes al crear
```json
{
  "errors": {
    "CenterLatitude": ["La latitud del centro es requerida"],
    "RadiusInMeters": ["El radio es requerido"]
  }
}
```

### Error: Intentar eliminar workplace con usuarios
```json
{
  "message": "No se puede eliminar el workplace porque tiene usuarios asignados"
}
```

### Error: Intentar crear usuario sin workplace
```json
{
  "message": "Los usuarios deben tener un workplace asignado"
}
```

---

## Notas Adicionales

1. **Autenticación**: Todos los endpoints requieren un token JWT válido con rol "Administrador"
2. **Coordenadas**: Las coordenadas deben estar en formato decimal (no grados/minutos/segundos)
3. **Radio**: El radio del geofence debe estar entre 10 y 10,000 metros
4. **Soft Delete**: Los workplaces eliminados se marcan como inactivos, no se borran físicamente

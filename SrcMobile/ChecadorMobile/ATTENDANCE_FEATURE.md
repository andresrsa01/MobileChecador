# Nueva Funcionalidad: Registro de Asistencia

## Archivos Creados

### Modelos
- **Models/AttendanceRequest.cs**: Modelo para enviar datos de asistencia al API
  - UserId: ID del usuario
  - Latitude: Latitud de la ubicación
  - Longitude: Longitud de la ubicación
  - Timestamp: Fecha y hora del registro

- **Models/AttendanceResponse.cs**: Modelo para recibir respuesta del API
  - Success: Indica si la operación fue exitosa
  - Message: Mensaje descriptivo
  - AttendanceId: ID del registro de asistencia

### ViewModel
- **ViewModels/NewAttendanceViewModel.cs**: Lógica de negocio
  - Obtiene la ubicación actual del dispositivo
  - Envía los datos al API
  - Maneja errores y permisos
  - Muestra mensajes de estado al usuario

### Vistas
- **Views/NewAttendancePage.xaml**: Interfaz de usuario
  - Botón para registrar asistencia
  - Indicador de carga
  - Muestra coordenadas actuales
  - Mensajes de estado

- **Views/NewAttendancePage.xaml.cs**: Code-behind de la vista

### Servicios
- **Actualización en Services/IApiService.cs**: 
  - Nuevo endpoint: `RegisterAttendanceAsync`
  - Ruta API: `/api/attendance/register` (POST)

### Configuración de Permisos
- **Platforms/Android/AndroidManifest.xml**: Permisos de ubicación para Android
- **Platforms/iOS/Info.plist**: Permisos de ubicación para iOS
- **Platforms/MacCatalyst/Info.plist**: Permisos de ubicación para Mac

### Registro de Dependencias
- **MauiProgram.cs**: Registrados `NewAttendanceViewModel` y `NewAttendancePage`
- **AppShell.xaml**: Ruta registrada como `newAttendance`

## Funcionalidad

Al presionar el botón "Registrar Asistencia":
1. Se solicitan permisos de ubicación (si no están otorgados)
2. Se obtiene la ubicación actual del dispositivo
3. Se envía al servidor:
   - ID del usuario autenticado
   - Coordenadas de ubicación (latitud/longitud)
   - Timestamp actual
4. Se muestra el resultado al usuario

## Endpoint API (Por Definir)

```csharp
POST /api/attendance/register
Content-Type: application/json

{
  "userId": 123,
  "latitude": 19.4326,
  "longitude": -99.1332,
  "timestamp": "2024-01-15T10:30:00Z"
}

Response:
{
  "success": true,
  "message": "Asistencia registrada correctamente",
  "attendanceId": 456
}
```

## Navegación

La página está disponible en el menú lateral bajo "Nueva Asistencia" y también se puede acceder programáticamente usando:

```csharp
await _navigationService.NavigateToAsync("///newAttendance");
```

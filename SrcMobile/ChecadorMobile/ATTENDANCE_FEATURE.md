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

- **ViewModels/LoginViewModel.cs**: Actualizado
  - Verifica permisos de ubicación después del login exitoso
  - Solicita permisos al usuario al iniciar sesión
  - Proporciona instrucciones para habilitar permisos en iOS

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

### Después del Login (Nuevo)
**Verificación Automática de Permisos al Iniciar Sesión:**

1. Después de un login exitoso, se verifica automáticamente el estado de los permisos de ubicación
2. Si los permisos NO están otorgados:
   - Se muestra un diálogo preguntando al usuario si desea habilitarlos
   - Opciones: "Sí" o "Más tarde"
3. Si el usuario elige "Sí":
   - Se solicitan los permisos del sistema
   - **Permisos otorgados**: Mensaje de confirmación
   - **Permisos denegados en iOS**: Instrucciones para ir a Configuración
   - **Permisos denegados en Android**: Mensaje indicando cómo habilitarlos
4. Si el usuario elige "Más tarde":
   - Se muestra un recordatorio de que necesitará los permisos para registrar asistencia
5. Si los permisos ya están otorgados:
   - No se muestra ningún mensaje (experiencia fluida)

### Al Abrir la Página de Registro
1. Se verifica automáticamente el estado de los permisos de ubicación
2. Se verifica la conexión a Internet disponible
3. Si no están otorgados los permisos, se muestra una alerta visual amarilla
4. Si no hay conexión a Internet, se muestra una alerta visual roja
5. El estado de conexión se actualiza en tiempo real si cambia durante el uso

### Al Presionar el Botón "Registrar Asistencia"
1. **Validación de conexión a Internet** - Verifica que haya conexión disponible
   - Si no hay conexión, muestra mensaje y detiene el proceso
2. **Validación de permisos de ubicación** - Solicita permisos si no están otorgados
   - En Android: Se muestra el diálogo de permisos del sistema
   - En iOS: Se muestra el diálogo de permisos o se informa al usuario que debe ir a Configuración si fue previamente denegado
3. Si los permisos son denegados, se muestra un mensaje claro indicando cómo habilitarlos
4. **Obtención de ubicación** - Se obtiene la ubicación actual del dispositivo con validación de GPS activo
5. **Validación pre-envío** - Se verifica nuevamente la conexión a Internet antes de enviar
6. **Envío al servidor**:
   - ID del usuario autenticado
   - Coordenadas de ubicación (latitud/longitud)
   - Timestamp actual
7. Se muestra el resultado al usuario con mensajes descriptivos

### Validaciones Implementadas

#### Conexión a Internet
- **Verificación inicial**: Al cargar la página se verifica la conectividad
- **Verificación pre-proceso**: Antes de iniciar el registro, se valida la conexión
- **Verificación pre-envío**: Antes de enviar a la API, se valida nuevamente la conexión
- **Monitoreo en tiempo real**: Se suscribe a eventos de cambio de conectividad
- **Alerta visual roja**: Se muestra cuando no hay conexión disponible
- **Mensajes específicos**:
  - Sin conexión inicial
  - Pérdida de conexión durante el proceso
  - Error de conexión con el servidor (HttpRequestException)

#### Permisos de Ubicación
- **Verificación post-login**: Después de iniciar sesión exitosamente, se verifica automáticamente el estado de los permisos
- **Solicitud proactiva**: Se pregunta al usuario si desea habilitar permisos inmediatamente después del login
- **Verificación inicial**: Al cargar la página se verifica el estado de los permisos
- **Validación antes de registro**: Antes de obtener la ubicación, se valida que los permisos estén otorgados
- **Alerta visual amarilla**: Se muestra cuando no hay permisos
- **Mensajes claros**: Se proporcionan mensajes específicos según el estado y plataforma:
  - Permisos no otorgados
  - Permisos denegados (con instrucciones para iOS)
  - GPS desactivado
  - Dispositivo sin soporte de geolocalización
- **Experiencia de usuario mejorada**: 
  - Opción "Más tarde" para no interrumpir el flujo
  - Mensajes de recordatorio amigables
  - Instrucciones específicas por plataforma

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

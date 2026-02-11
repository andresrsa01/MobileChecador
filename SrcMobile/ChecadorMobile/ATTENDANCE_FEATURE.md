# Nueva Funcionalidad: Registro de Asistencia

## Archivos Creados

### Modelos
- **Models/AttendanceRequest.cs**: Modelo para enviar datos de asistencia al API
  - UserId: ID del usuario
  - Latitude: Latitud de la ubicacion
  - Longitude: Longitud de la ubicacion
  - Timestamp: Fecha y hora del registro

- **Models/AttendanceResponse.cs**: Modelo para recibir respuesta del API
  - Success: Indica si la operacion fue exitosa
  - Message: Mensaje descriptivo
  - AttendanceId: ID del registro de asistencia

### ViewModel
- **ViewModels/NewAttendanceViewModel.cs**: Logica de negocio
  - Obtiene la ubicacion actual del dispositivo
  - Envia los datos al API
  - Maneja errores y permisos
  - Muestra mensajes de estado al usuario

- **ViewModels/LoginViewModel.cs**: Actualizado
  - Verifica permisos de ubicacion despues del login exitoso
  - Solicita permisos al usuario al iniciar sesion
  - Proporciona instrucciones para habilitar permisos en iOS

### Vistas
- **Views/NewAttendancePage.xaml**: Interfaz de usuario
  - Boton para registrar asistencia
  - Indicador de carga
  - Muestra coordenadas actuales
  - Mensajes de estado

- **Views/NewAttendancePage.xaml.cs**: Code-behind de la vista

### Servicios
- **Actualizacion en Services/IApiService.cs**: 
  - Nuevo endpoint: `RegisterAttendanceAsync`
  - Ruta API: `/api/attendance/register` (POST)

### Configuracion de Permisos
- **Platforms/Android/AndroidManifest.xml**: Permisos de ubicacion para Android
- **Platforms/iOS/Info.plist**: Permisos de ubicacion para iOS
- **Platforms/MacCatalyst/Info.plist**: Permisos de ubicacion para Mac

### Registro de Dependencias
- **MauiProgram.cs**: Registrados `NewAttendanceViewModel` y `NewAttendancePage`
- **AppShell.xaml**: Ruta registrada como `newAttendance`

## Funcionalidad

### Despues del Login (Nuevo)
**Verificacion Automatica de Permisos al Iniciar Sesion:**

1. Despues de un login exitoso, se verifica automaticamente el estado de los permisos de ubicacion
2. Si los permisos NO estan otorgados:
   - Se muestra un dialogo preguntando al usuario si desea habilitarlos
   - Opciones: "Si" o "Mas tarde"
3. Si el usuario elige "Si":
   - Se solicitan los permisos del sistema
   - **Permisos otorgados**: Mensaje de confirmacion
   - **Permisos denegados en iOS**: Instrucciones para ir a Configuracion
   - **Permisos denegados en Android**: Mensaje indicando como habilitarlos
4. Si el usuario elige "Mas tarde":
   - Se muestra un recordatorio de que necesitara los permisos para registrar asistencia
5. Si los permisos ya estan otorgados:
   - No se muestra ningun mensaje (experiencia fluida)

### Al Abrir la Pagina de Registro
1. Se verifica automaticamente el estado de los permisos de ubicacion
2. Se verifica la conexion a Internet disponible
3. Si no estan otorgados los permisos, se muestra una alerta visual amarilla
4. Si no hay conexion a Internet, se muestra una alerta visual roja
5. El estado de conexion se actualiza en tiempo real si cambia durante el uso

### Al Presionar el Boton "Registrar Asistencia"
1. **Validacion de conexion a Internet** - Verifica que haya conexion disponible
   - Si no hay conexion, muestra mensaje y detiene el proceso
2. **Validacion de permisos de ubicacion** - Solicita permisos si no estan otorgados
   - En Android: Se muestra el dialogo de permisos del sistema
   - En iOS: Se muestra el dialogo de permisos o se informa al usuario que debe ir a Configuracion si fue previamente denegado
3. Si los permisos son denegados, se muestra un mensaje claro indicando como habilitarlos
4. **Obtencion de ubicacion** - Se obtiene la ubicacion actual del dispositivo con validacion de GPS activo
5. **Validacion pre-envio** - Se verifica nuevamente la conexion a Internet antes de enviar
6. **Envio al servidor**:
   - ID del usuario autenticado
   - Coordenadas de ubicacion (latitud/longitud)
   - Timestamp actual
7. Se muestra el resultado al usuario con mensajes descriptivos

### Validaciones Implementadas

#### Conexion a Internet
- **Verificacion inicial**: Al cargar la pagina se verifica la conectividad
- **Verificacion pre-proceso**: Antes de iniciar el registro, se valida la conexion
- **Verificacion pre-envio**: Antes de enviar a la API, se valida nuevamente la conexion
- **Monitoreo en tiempo real**: Se suscribe a eventos de cambio de conectividad
- **Alerta visual roja**: Se muestra cuando no hay conexion disponible
- **Mensajes especificos**:
  - Sin conexion inicial
  - Perdida de conexion durante el proceso
  - Error de conexion con el servidor (HttpRequestException)

#### Permisos de Ubicacion
- **Verificacion post-login**: Despues de iniciar sesion exitosamente, se verifica automaticamente el estado de los permisos
- **Solicitud proactiva**: Se pregunta al usuario si desea habilitar permisos inmediatamente despues del login
- **Verificacion inicial**: Al cargar la pagina se verifica el estado de los permisos
- **Validacion antes de registro**: Antes de obtener la ubicacion, se valida que los permisos esten otorgados
- **Alerta visual amarilla**: Se muestra cuando no hay permisos
- **Mensajes claros**: Se proporcionan mensajes especificos segun el estado y plataforma:
  - Permisos no otorgados
  - Permisos denegados (con instrucciones para iOS)
  - GPS desactivado
  - Dispositivo sin soporte de geolocalizacion
- **Experiencia de usuario mejorada**: 
  - Opcion "Mas tarde" para no interrumpir el flujo
  - Mensajes de recordatorio amigables
  - Instrucciones especificas por plataforma

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

## Navegacion

La pagina esta disponible en el menu lateral bajo "Nueva Asistencia" y tambien se puede acceder programaticamente usando:

```csharp
await _navigationService.NavigateToAsync("///newAttendance");
```

# Flujo de Permisos de Ubicación

## Resumen
Se implementó un sistema completo de verificación y solicitud de permisos de ubicación en múltiples puntos de la aplicación para garantizar una experiencia de usuario óptima.

## Puntos de Verificación

### 1?? Después del Login (LoginViewModel)
**Cuándo**: Inmediatamente después de un login exitoso

**Flujo**:
```
Usuario hace login exitosamente
    ?
Se navega al AppShell (página principal)
    ?
Se verifica el estado de permisos de ubicación
    ?
¿Permisos otorgados?
    ?
    ?? SÍ ? No se muestra nada (flujo continúa)
    ?
    ?? NO ? Se muestra diálogo: "¿Deseas habilitar permisos ahora?"
            ?
            ?? Usuario elige "Sí"
            ?   ?
            ?   Se solicitan permisos al sistema
            ?   ?
            ?   ?? Otorgados ? Mensaje de confirmación
            ?   ?? Denegados (iOS) ? Instrucciones para ir a Configuración
            ?   ?? Denegados (Android) ? Mensaje de cómo habilitarlos
            ?
            ?? Usuario elige "Más tarde"
                ?
                Mensaje recordatorio
```

### 2?? En la Página de Registro de Asistencia (NewAttendanceViewModel)
**Cuándo**: Al abrir la página (`OnAppearing`)

**Flujo**:
```
Se abre NewAttendancePage
    ?
InitializeAsync() se ejecuta
    ?
CheckLocationPermissionAsync()
    ?
¿Permisos otorgados?
    ?
    ?? SÍ ? StatusMessage = "Presiona el botón..."
    ?        + Alerta amarilla oculta
    ?
    ?? NO ? StatusMessage = "?? Se requieren permisos..."
            + Alerta amarilla visible
```

### 3?? Al Registrar Asistencia (Botón)
**Cuándo**: Usuario presiona "Registrar Asistencia"

**Flujo**:
```
Usuario presiona botón
    ?
Verificar conexión a Internet
    ?
¿Hay conexión?
    ?
    ?? NO ? Mostrar alerta y detener
    ?
    ?? SÍ ? RequestLocationPermissionAsync()
            ?
            ¿Permisos otorgados?
            ?
            ?? SÍ ? Continuar con registro
            ?
            ?? NO ? Solicitar permisos
                    ?
                    ?? Otorgados ? Continuar
                    ?? Denegados (iOS) ? Mostrar instrucciones + Detener
                    ?? Denegados (Android) ? Mostrar mensaje + Detener
```

## Mensajes y Alertas

### Post-Login
| Escenario | Título | Mensaje | Botones |
|-----------|--------|---------|---------|
| Sin permisos | "Permisos de Ubicación" | "La aplicación necesita acceso a tu ubicación para registrar asistencias. ¿Deseas habilitar los permisos ahora?" | "Sí" / "Más tarde" |
| Permisos otorgados | "Permisos Otorgados" | "Los permisos de ubicación han sido habilitados correctamente." | "OK" |
| Permisos denegados (iOS) | "Permisos Denegados" | "Para habilitar los permisos de ubicación, ve a: Configuracion > MobileChecador > Ubicación" | "Entendido" |
| Permisos denegados (Android) | "Permisos Denegados" | "Los permisos de ubicación fueron denegados. Puedes habilitarlos más tarde en la configuración de tu dispositivo." | "OK" |
| Más tarde | "Recordatorio" | "Recuerda que necesitarás habilitar los permisos de ubicación para poder registrar tu asistencia." | "OK" |

### En Página de Registro
| Escenario | Elemento UI | Color | Texto |
|-----------|-------------|-------|-------|
| Sin permisos | Frame alerta | Amarillo (#FFF3CD) | "?? Permisos de Ubicación Requeridos" |
| Sin Internet | Frame alerta | Rojo (#F8D7DA) | "?? Sin Conexión a Internet" |
| Con permisos | StatusMessage | Gris | "Presiona el botón para registrar tu asistencia" |
| Sin permisos | StatusMessage | - | "?? Se requieren permisos de ubicación para registrar asistencia" |

### Al Registrar
| Escenario | Título | Mensaje |
|-----------|--------|---------|
| Sin conexión | "Sin Conexión" | "No hay conexión a Internet. Por favor, verifica tu conexión e intenta nuevamente." |
| Sin permisos | "Permisos Requeridos" | "Para registrar tu asistencia, necesitas habilitar los permisos de ubicación en la configuración de tu dispositivo." |
| GPS desactivado | "Error de Ubicación" | "No se pudo obtener tu ubicación actual. Verifica que el GPS esté activado." |
| iOS permisos denegados | "Permisos de Ubicación" | "Los permisos de ubicación están deshabilitados. Por favor, ve a Configuración > MobileChecador > Ubicación y habilítalos." |

## Ventajas del Flujo Implementado

### ? Experiencia de Usuario
- **Proactivo**: Solicita permisos inmediatamente después del login
- **No intrusivo**: Opción "Más tarde" para no interrumpir
- **Informativo**: Mensajes claros y específicos por plataforma
- **Visual**: Alertas de colores para identificar rápidamente problemas

### ? Robustez
- **Múltiples verificaciones**: En login, al abrir página, y al registrar
- **Manejo de errores**: Try-catch para evitar crashes
- **Tiempo real**: Actualización automática del estado de permisos

### ? Plataforma Específica
- **iOS**: Detecta permisos previamente denegados e instruye ir a Configuración
- **Android**: Permite solicitar permisos múltiples veces
- **Cross-platform**: Usa APIs de .NET MAUI para máxima compatibilidad

## Archivos Modificados

1. **ViewModels/LoginViewModel.cs**
   - Método `CheckLocationPermissionsAfterLoginAsync()` agregado
   - Se llama después de login exitoso

2. **ViewModels/NewAttendanceViewModel.cs**
   - Propiedad `HasLocationPermission` observable
   - Método `InitializeAsync()` para verificación inicial
   - Método `RequestLocationPermissionAsync()` mejorado
   - Alertas visuales sincronizadas con propiedades

3. **Views/NewAttendancePage.xaml**
   - Frame amarillo para alertas de permisos
   - Frame rojo para alertas de conexión
   - Bindings a propiedades observables

4. **Views/NewAttendancePage.xaml.cs**
   - Override `OnAppearing()` para llamar `InitializeAsync()`

## Testing Recomendado

### Escenarios de Prueba

1. **Login sin permisos previos**
   - Verificar que aparezca diálogo después del login
   - Probar "Sí" y verificar solicitud de permisos
   - Probar "Más tarde" y verificar mensaje recordatorio

2. **Login con permisos otorgados**
   - Verificar que NO aparezca ningún diálogo
   - Flujo debe ser fluido

3. **Abrir página de registro**
   - Sin permisos: Verificar alerta amarilla
   - Con permisos: Verificar sin alertas

4. **Registrar asistencia**
   - Sin permisos: Verificar solicitud
   - Sin Internet: Verificar detención del proceso
   - Con todo correcto: Verificar registro exitoso

5. **iOS - Permisos denegados previamente**
   - Verificar mensaje específico de iOS
   - Verificar instrucciones de Configuración

6. **Android - Solicitud múltiple**
   - Verificar que se puede solicitar más de una vez
   - Verificar mensaje apropiado al denegar

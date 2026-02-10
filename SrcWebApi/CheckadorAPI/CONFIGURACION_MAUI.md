# Configuración de la Aplicación MAUI para Conectarse a la API

## Actualizar la URL Base del Servicio API

### 1. Actualizar IApiService (si es necesario)

La interfaz `IApiService` en la aplicación MAUI ya está configurada para trabajar con esta API. No necesitas cambiar nada en la interfaz.

### 2. Configurar la URL Base en MauiProgram.cs

Localiza el archivo `MauiProgram.cs` en tu proyecto MAUI y actualiza la configuración de Refit:

```csharp
// ANTES DE LA CONFIGURACIÓN ACTUAL
// Busca donde se registra IApiService con Refit

// NUEVA CONFIGURACIÓN
builder.Services.AddRefitClient<IApiService>()
    .ConfigureHttpClient(c =>
    {
        #if DEBUG
            #if ANDROID
                // Para Android Emulator
                c.BaseAddress = new Uri("http://10.0.2.2:5000");
            #elif IOS
                // Para iOS Simulator
                c.BaseAddress = new Uri("http://localhost:5000");
            #else
                // Para Windows o dispositivo físico (cambiar por tu IP local)
                c.BaseAddress = new Uri("http://192.168.1.XXX:5000");
            #endif
        #else
            // Para producción
            c.BaseAddress = new Uri("https://tu-servidor.com");
        #endif

        c.Timeout = TimeSpan.FromSeconds(30);
    })
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        #if DEBUG
        // IMPORTANTE: Solo para desarrollo local
        // NO usar en producción
        return new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = 
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        #else
        return new HttpClientHandler();
        #endif
    });
```

## Obtener tu IP Local

### Windows
```powershell
# PowerShell
Get-NetIPAddress -AddressFamily IPv4 | Where-Object {$_.InterfaceAlias -notmatch "Loopback"}

# CMD
ipconfig
```

### macOS/Linux
```bash
ifconfig | grep "inet "
# o
ip addr show
```

Busca la IP que comience con `192.168.x.x` o `10.0.x.x`

## Configurar Firewall en Windows

Para permitir que dispositivos en tu red accedan a la API:

```powershell
# Ejecutar como Administrador
New-NetFirewallRule -DisplayName "Checador API" `
    -Direction Inbound `
    -LocalPort 5000,5001 `
    -Protocol TCP `
    -Action Allow
```

## Iniciar la API

### Opción 1: Visual Studio
1. Abre el proyecto `CheckadorAPI` en Visual Studio
2. Presiona F5 o click en "Run"
3. La API se iniciará en `https://localhost:5001`

### Opción 2: Línea de Comandos
```bash
cd CheckadorAPI
dotnet run
```

### Opción 3: En una IP Específica (para dispositivos físicos)
```bash
cd CheckadorAPI
dotnet run --urls "http://0.0.0.0:5000;https://0.0.0.0:5001"
```

## Probar la Conexión desde MAUI

### Método 1: Desde el Login
1. Inicia la API
2. Inicia la app MAUI
3. Usa las credenciales:
   - Usuario: `usuario1`
   - Contraseña: `User123!`

### Método 2: Crear un Método de Prueba

En tu `ApiService` o donde sea apropiado:

```csharp
public async Task<bool> TestConnectionAsync()
{
    try
    {
        var response = await _httpClient.GetAsync("/swagger/index.html");
        return response.IsSuccessStatusCode;
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Connection test failed: {ex.Message}");
        return false;
    }
}
```

## Solución de Problemas Comunes

### ? "Unable to connect to the remote server"

**Causa**: La app no puede alcanzar la API

**Soluciones**:
1. Verifica que la API esté ejecutándose
2. Para Android Emulator, usa `http://10.0.2.2:5000`
3. Para iOS Simulator, usa `http://localhost:5000`
4. Para dispositivo físico:
   - Verifica que estés en la misma red WiFi
   - Usa tu IP local (ej: `http://192.168.1.100:5000`)
   - Verifica el firewall de Windows

### ? "No connection could be made because the target machine actively refused it"

**Causa**: La API no está corriendo o el puerto es incorrecto

**Soluciones**:
1. Inicia la API con `dotnet run`
2. Verifica que el puerto sea el correcto (5000 para HTTP, 5001 para HTTPS)
3. Verifica en el navegador: `http://localhost:5000/swagger`

### ? "The SSL connection could not be established"

**Causa**: Problema con el certificado SSL en desarrollo

**Soluciones**:
1. Usa HTTP en lugar de HTTPS en desarrollo: `http://10.0.2.2:5000`
2. O configura el handler para ignorar certificados (solo desarrollo):
```csharp
ServerCertificateCustomValidationCallback = 
    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
```

### ? "Usuario o contraseña incorrectos"

**Causa**: API funcionando pero credenciales incorrectas

**Soluciones**:
1. Verifica que uses las credenciales correctas:
   - Usuario: `usuario1`
   - Contraseña: `User123!` (con mayúscula en U y P)
2. Verifica que la base de datos esté inicializada:
   ```bash
   cd CheckadorAPI
   dotnet ef database update
   ```

### ? "No hay configuración de geofence"

**Causa**: El usuario no tiene un geofence asignado

**Soluciones**:
1. El usuario `usuario1` ya tiene un geofence por defecto
2. Si creaste un nuevo usuario, debes asignarle un geofence:
   ```bash
   # Usar Swagger o hacer una llamada POST a /api/geofence
   ```

## Configuración para Dispositivo Físico

### Android

1. **Conecta tu dispositivo y PC a la misma red WiFi**

2. **Obtén tu IP local**: 
   ```powershell
   ipconfig
   ```
   Ejemplo: `192.168.1.100`

3. **Configura en MauiProgram.cs**:
   ```csharp
   #if ANDROID
   c.BaseAddress = new Uri("http://192.168.1.100:5000");
   #endif
   ```

4. **Inicia la API**:
   ```bash
   dotnet run --urls "http://0.0.0.0:5000"
   ```

5. **Permite el firewall** (Windows):
   ```powershell
   New-NetFirewallRule -DisplayName "Checador API" -Direction Inbound -LocalPort 5000 -Protocol TCP -Action Allow
   ```

### iOS

Similar a Android, pero iOS tiene restricciones adicionales de seguridad:

1. **Usa HTTPS o habilita ATS exception** en `Info.plist`:
```xml
<key>NSAppTransportSecurity</key>
<dict>
    <key>NSAllowsLocalNetworking</key>
    <true/>
    <key>NSAllowsArbitraryLoads</key>
    <true/>
</dict>
```

## Verificar que Todo Funciona

### 1. Probar la API directamente

Abre el navegador en:
- `http://localhost:5000/swagger` (en tu PC)
- `http://TU_IP:5000/swagger` (desde otro dispositivo)

### 2. Probar Login desde Swagger

1. Expande `POST /api/auth/login`
2. Click en "Try it out"
3. Ingresa:
   ```json
   {
     "username": "usuario1",
     "password": "User123!"
   }
   ```
4. Click "Execute"
5. Deberías recibir un token JWT

### 3. Probar desde la App MAUI

1. Inicia la app
2. Ingresa las credenciales
3. Deberías poder iniciar sesión y ver la página principal

## Próximos Pasos

Una vez que la conexión funcione:

1. ? Verifica que el login funcione
2. ? Prueba el registro de asistencia (necesitas estar en las coordenadas del geofence o modificarlo)
3. ? Verifica que el perfil se cargue correctamente
4. ? Prueba cerrar sesión y volver a entrar

## Configuración Avanzada

### Usar una Base de Datos Real (No LocalDB)

Edita `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tu-servidor;Database=CheckadorDB;User Id=tu-usuario;Password=tu-password;TrustServerCertificate=True"
  }
}
```

### Habilitar Logging Detallado

Edita `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

### CORS para Desarrollo

Si necesitas permitir orígenes específicos, edita `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("Development", builder =>
    {
        builder.WithOrigins("http://localhost:3000", "http://192.168.1.100")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Luego en el pipeline:
app.UseCors("Development");
```

## Checklist de Verificación

Antes de empezar a desarrollar, verifica que:

- [ ] La API compila sin errores: `dotnet build`
- [ ] La base de datos está inicializada: `dotnet ef database update`
- [ ] La API se ejecuta: `dotnet run`
- [ ] Swagger es accesible: `http://localhost:5000/swagger`
- [ ] El login funciona desde Swagger
- [ ] Tu IP local está configurada en MAUI (si usas dispositivo físico)
- [ ] El firewall permite conexiones en el puerto 5000
- [ ] La app MAUI puede conectarse a la API
- [ ] El login funciona desde la app MAUI

## Recursos Adicionales

- **README.md**: Documentación completa de la API
- **EXAMPLES.md**: Ejemplos de todas las llamadas a la API
- **RESUMEN.md**: Resumen ejecutivo del proyecto
- **test-api.ps1**: Script de testing automatizado
- **Swagger UI**: `http://localhost:5000/swagger`

## Soporte

Si encuentras problemas que no están cubiertos aquí:

1. Revisa los logs de la API en la consola
2. Revisa los logs de la app MAUI en Output de Visual Studio
3. Usa el script de testing: `.\test-api.ps1`
4. Verifica la conectividad básica con `ping` o `telnet`

---

**Importante**: Recuerda cambiar todas las configuraciones de seguridad cuando pases a producción (certificados SSL, CORS, secretos, etc.)

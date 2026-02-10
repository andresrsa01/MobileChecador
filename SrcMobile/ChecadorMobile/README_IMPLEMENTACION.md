# ?? MauiGenCartaPorte - Implementación MVVM

## ? Implementación Completada

Se ha implementado exitosamente el patrón **MVVM** con **MVVM Toolkit** en el proyecto .NET MAUI.

---

## ?? Paquetes NuGet Instalados

- ? **CommunityToolkit.Mvvm** (v8.4.0) - MVVM Toolkit
- ? **sqlite-net-pcl** (v1.9.172) - SQLite ORM
- ? **SQLitePCLRaw.bundle_green** (v2.1.11) - SQLite nativo

---

## ??? Arquitectura Implementada

```
MauiGenCartaPorte/
?
??? ?? Models/
?   ??? User.cs                      # Modelo de usuario con SQLite
?
??? ?? Services/
?   ??? IDatabaseService.cs          # Interface del servicio de BD
?   ??? DatabaseService.cs           # Implementación SQLite
?   ??? IAuthenticationService.cs    # Interface de autenticación
?   ??? AuthenticationService.cs     # Implementación de auth
?   ??? INavigationService.cs        # Interface de navegación
?   ??? NavigationService.cs         # Implementación de navegación
?
??? ?? ViewModels/
?   ??? LoginViewModel.cs            # ViewModel del login
?   ??? MainViewModel.cs             # ViewModel página principal
?   ??? ProfileViewModel.cs          # ViewModel del perfil
?
??? ?? Views/
?   ??? LoginPage.xaml/.cs           # Página de inicio de sesión
?   ??? MainPage.xaml/.cs            # Página principal
?   ??? ProfilePage.xaml/.cs         # Página de perfil
?
??? ?? Converters/
?   ??? InvertedBoolConverter.cs     # Invierte valores booleanos
?   ??? StringIsNotNullOrEmptyConverter.cs  # Valida strings
?
??? AppShell.xaml/.cs                # Shell con menú hamburguesa
??? App.xaml/.cs                     # Configuración de la app
??? MauiProgram.cs                   # Registro de DI
```

---

## ?? Características Implementadas

### 1. ?? Sistema de Autenticación
- Login con usuario y contraseña
- Validación de credenciales contra SQLite
- Sesión persistente con CurrentUser
- Usuario demo precargado:
  - **Usuario:** `admin`
  - **Contraseña:** `admin123`

### 2. ?? Menú Hamburguesa (Flyout)
- Header personalizado con logo
- Secciones organizadas:
  - Inicio
  - Mi Perfil
  - Cartas Porte (con subsecciones)
  - Configuración
- Footer con versión de la app
- Navegación fluida entre páginas

### 3. ?? Páginas Implementadas

#### LoginPage
- Formulario de inicio de sesión
- Validación de campos
- Mensajes de error
- Indicador de carga
- Diseño moderno y responsive

#### MainPage
- Página de inicio con bienvenida
- Grid de acciones rápidas
- Cards con iconos
- Sección de información

#### ProfilePage
- Información del usuario autenticado
- Avatar con emoji
- Datos organizados en cards
- Botón de cerrar sesión
- Formato de fechas

### 4. ?? Base de Datos SQLite
- Tabla de usuarios con campos:
  - Id (PK, AutoIncrement)
  - Username (Único)
  - Password (Hash en producción)
  - FullName
  - Email
  - CreatedAt
  - LastLogin
  - IsActive
- Inicialización automática
- Usuario demo precargado
- CRUD completo implementado

### 5. ?? Inyección de Dependencias
- Servicios registrados como Singleton
- ViewModels registrados como Transient
- Views registradas como Transient
- DI nativa de .NET MAUI

### 6. ??? Servicios Implementados

#### DatabaseService
- Gestión completa de SQLite
- Métodos CRUD para usuarios
- Validación de credenciales
- Actualización de último login

#### AuthenticationService
- Login/Logout
- Usuario actual (CurrentUser)
- Estado de autenticación (IsAuthenticated)

#### NavigationService
- Navegación programática
- Integración con Shell
- Rutas registradas

---

## ?? Diseño UI/UX

### Estilos Aplicados
- Uso de recursos dinámicos (DynamicResource)
- Paleta de colores del tema
- Frames con sombras
- Bordes redondeados (CornerRadius)
- Espaciado consistente
- Responsive design

### Converters Implementados
- **InvertedBoolConverter**: Para habilitar/deshabilitar controles
- **StringIsNotNullOrEmptyConverter**: Para mostrar mensajes de error

---

## ?? Flujo de Navegación

```
1. App Inicia
   ?
2. AppShell se carga
   ?
3. LoginPage (///login) - Sin Flyout
   ?
4. [Usuario ingresa credenciales]
   ?
5. Validación contra SQLite
   ?
6. MainPage (///main) - Con Flyout visible
   ??? ProfilePage
   ??? Configuración
   ??? Otras secciones
```

---

## ?? Rutas Registradas

- `///login` - Página de login (sin flyout)
- `///main` - Página principal
- `///profile` - Página de perfil
- `newcarta` - Nueva carta (placeholder)
- `listcartas` - Lista de cartas (placeholder)
- `settings` - Configuración (placeholder)

---

## ?? Configuración de Dependencias

### MauiProgram.cs
```csharp
// Servicios (Singleton)
builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
builder.Services.AddSingleton<INavigationService, NavigationService>();

// ViewModels (Transient)
builder.Services.AddTransient<LoginViewModel>();
builder.Services.AddTransient<MainViewModel>();
builder.Services.AddTransient<ProfileViewModel>();

// Views (Transient)
builder.Services.AddTransient<LoginPage>();
builder.Services.AddTransient<MainPage>();
builder.Services.AddTransient<ProfilePage>();
```

---

## ?? Modelo de Datos Actual

### User
```csharp
- Id: int (PK, AI)
- Username: string (Unique, MaxLength: 100)
- Password: string (MaxLength: 255) 
- FullName: string (MaxLength: 200)
- Email: string (MaxLength: 200)
- CreatedAt: DateTime
- LastLogin: DateTime?
- IsActive: bool
```

---

## ?? Uso de MVVM Toolkit

### Ejemplo en ViewModels:

```csharp
// Propiedad observable
[ObservableProperty]
private string username = string.Empty;
// Genera automáticamente: public string Username { get; set; }
// con INotifyPropertyChanged

// Comando
[RelayCommand]
private async Task LoginAsync()
{
    // Lógica del comando
}
// Genera automáticamente: public IAsyncRelayCommand LoginCommand { get; }
```

---

## ?? Cómo Probar la Aplicación

1. **Ejecutar la aplicación**
2. **En LoginPage:**
   - Usuario: `admin`
   - Contraseña: `admin123`
3. **Hacer clic en "Iniciar Sesión"**
4. **Explorar el menú hamburguesa:**
   - Deslizar desde la izquierda o tocar el ícono ?
5. **Navegar a "Mi Perfil":**
   - Ver información del usuario
   - Probar "Cerrar Sesión"

---

## ?? Documentación Adicional

Ver archivo: **`INSTRUCCIONES_MVVM.md`** para:
- ? Guía completa para agregar nuevas páginas
- ? Ejemplos de código paso a paso
- ? Mejores prácticas
- ? Tips y trucos
- ? Operaciones SQLite avanzadas
- ? Uso completo del MVVM Toolkit

---

## ?? Seguridad (Producción)

?? **IMPORTANTE**: En un entorno de producción:

1. **NO almacenar passwords en texto plano**
   - Usar hashing (BCrypt, PBKDF2, etc.)
   - Ejemplo: `BCrypt.Net.BCrypt.HashPassword(password)`

2. **Implementar validación de entrada**
   - Sanitizar datos de usuario
   - Validar longitudes y formatos

3. **Usar HTTPS para APIs**
   - Comunicación segura con backend

4. **Implementar tokens JWT**
   - Para sesiones persistentes seguras

5. **Cifrar la base de datos SQLite**
   - Usar SQLCipher para bases de datos sensibles

---

## ?? Troubleshooting

### Error: "La base de datos no se crea"
- Verificar permisos de FileSystem.AppDataDirectory
- Asegurarse de que InitializeAsync() se llama

### Error: "No navega después del login"
- Verificar rutas en AppShell
- Revisar que los servicios estén registrados

### Error: "ViewModel no recibe datos"
- Verificar BindingContext en el code-behind
- Asegurar x:DataType en XAML

---

## ?? Próximas Mejoras Sugeridas

1. ? ~~Implementar patrón MVVM~~
2. ? ~~Agregar SQLite~~
3. ? ~~Crear sistema de login~~
4. ? ~~Implementar menú hamburguesa~~
5. ? Agregar páginas CRUD para CartaPorte
6. ? Implementar búsqueda y filtros
7. ? Agregar validación de formularios
8. ? Implementar sincronización con API
9. ? Agregar manejo de imágenes
10. ? Implementar notificaciones push
11. ? Agregar modo offline
12. ? Implementar exportación a PDF

---

## ?? Contribución

Para contribuir al proyecto:

1. Seguir la estructura MVVM establecida
2. Usar MVVM Toolkit para ViewModels
3. Registrar servicios en MauiProgram.cs
4. Documentar código complejo
5. Seguir convenciones de nombres
6. Probar en múltiples plataformas

---

## ?? Soporte

Para preguntas o problemas:
1. Revisar `INSTRUCCIONES_MVVM.md`
2. Consultar documentación de MAUI
3. Revisar ejemplos en el código

---

## ?? Licencia

[Especificar licencia del proyecto]

---

**Versión:** 1.0.0  
**Última actualización:** 2024  
**Framework:** .NET 10.0  
**Plataformas:** Android, iOS, macOS, Windows

---

? **¡Proyecto listo para desarrollo!** ?

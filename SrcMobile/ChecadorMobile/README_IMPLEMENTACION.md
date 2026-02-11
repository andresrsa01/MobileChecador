# ?? MauiGenCartaPorte - Implementacion MVVM

## ? Implementacion Completada

Se ha implementado exitosamente el patron **MVVM** con **MVVM Toolkit** en el proyecto .NET MAUI.

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
?   ??? DatabaseService.cs           # Implementacion SQLite
?   ??? IAuthenticationService.cs    # Interface de autenticacion
?   ??? AuthenticationService.cs     # Implementacion de auth
?   ??? INavigationService.cs        # Interface de navegacion
?   ??? NavigationService.cs         # Implementacion de navegacion
?
??? ?? ViewModels/
?   ??? LoginViewModel.cs            # ViewModel del login
?   ??? MainViewModel.cs             # ViewModel pagina principal
?   ??? ProfileViewModel.cs          # ViewModel del perfil
?
??? ?? Views/
?   ??? LoginPage.xaml/.cs           # Pagina de inicio de sesion
?   ??? MainPage.xaml/.cs            # Pagina principal
?   ??? ProfilePage.xaml/.cs         # Pagina de perfil
?
??? ?? Converters/
?   ??? InvertedBoolConverter.cs     # Invierte valores booleanos
?   ??? StringIsNotNullOrEmptyConverter.cs  # Valida strings
?
??? AppShell.xaml/.cs                # Shell con menu hamburguesa
??? App.xaml/.cs                     # Configuracion de la app
??? MauiProgram.cs                   # Registro de DI
```

---

## ?? Caracteristicas Implementadas

### 1. ?? Sistema de Autenticacion
- Login con usuario y contraseña
- Validacion de credenciales contra SQLite
- Sesion persistente con CurrentUser
- Usuario demo precargado:
  - **Usuario:** `admin`
  - **Contraseña:** `admin123`

### 2. ?? Menu Hamburguesa (Flyout)
- Header personalizado con logo
- Secciones organizadas:
  - Inicio
  - Mi Perfil
  - Cartas Porte (con subsecciones)
  - Configuracion
- Footer con version de la app
- Navegacion fluida entre paginas

### 3. ?? Paginas Implementadas

#### LoginPage
- Formulario de inicio de sesion
- Validacion de campos
- Mensajes de error
- Indicador de carga
- Diseño moderno y responsive

#### MainPage
- Pagina de inicio con bienvenida
- Grid de acciones rapidas
- Cards con iconos
- Seccion de informacion

#### ProfilePage
- Informacion del usuario autenticado
- Avatar con emoji
- Datos organizados en cards
- Boton de cerrar sesion
- Formato de fechas

### 4. ?? Base de Datos SQLite
- Tabla de usuarios con campos:
  - Id (PK, AutoIncrement)
  - Username (unico)
  - Password (Hash en produccion)
  - FullName
  - Email
  - CreatedAt
  - LastLogin
  - IsActive
- Inicializacion automatica
- Usuario demo precargado
- CRUD completo implementado

### 5. ?? Inyeccion de Dependencias
- Servicios registrados como Singleton
- ViewModels registrados como Transient
- Views registradas como Transient
- DI nativa de .NET MAUI

### 6. ??? Servicios Implementados

#### DatabaseService
- Gestion completa de SQLite
- Metodos CRUD para usuarios
- Validacion de credenciales
- Actualizacion de ultimo login

#### AuthenticationService
- Login/Logout
- Usuario actual (CurrentUser)
- Estado de autenticacion (IsAuthenticated)

#### NavigationService
- Navegacion programatica
- Integracion con Shell
- Rutas registradas

---

## ?? Diseño UI/UX

### Estilos Aplicados
- Uso de recursos dinamicos (DynamicResource)
- Paleta de colores del tema
- Frames con sombras
- Bordes redondeados (CornerRadius)
- Espaciado consistente
- Responsive design

### Converters Implementados
- **InvertedBoolConverter**: Para habilitar/deshabilitar controles
- **StringIsNotNullOrEmptyConverter**: Para mostrar mensajes de error

---

## ?? Flujo de Navegacion

```
1. App Inicia
   ?
2. AppShell se carga
   ?
3. LoginPage (///login) - Sin Flyout
   ?
4. [Usuario ingresa credenciales]
   ?
5. Validacion contra SQLite
   ?
6. MainPage (///main) - Con Flyout visible
   ??? ProfilePage
   ??? Configuracion
   ??? Otras secciones
```

---

## ?? Rutas Registradas

- `///login` - Pagina de login (sin flyout)
- `///main` - Pagina principal
- `///profile` - Pagina de perfil
- `newcarta` - Nueva carta (placeholder)
- `listcartas` - Lista de cartas (placeholder)
- `settings` - Configuracion (placeholder)

---

## ?? Configuracion de Dependencias

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
// Genera automaticamente: public string Username { get; set; }
// con INotifyPropertyChanged

// Comando
[RelayCommand]
private async Task LoginAsync()
{
    // Logica del comando
}
// Genera automaticamente: public IAsyncRelayCommand LoginCommand { get; }
```

---

## ?? Como Probar la Aplicacion

1. **Ejecutar la aplicacion**
2. **En LoginPage:**
   - Usuario: `admin`
   - Contraseña: `admin123`
3. **Hacer clic en "Iniciar Sesion"**
4. **Explorar el menu hamburguesa:**
   - Deslizar desde la izquierda o tocar el icono ?
5. **Navegar a "Mi Perfil":**
   - Ver informacion del usuario
   - Probar "Cerrar Sesion"

---

## ?? Documentacion Adicional

Ver archivo: **`INSTRUCCIONES_MVVM.md`** para:
- ? Guia completa para agregar nuevas paginas
- ? Ejemplos de codigo paso a paso
- ? Mejores practicas
- ? Tips y trucos
- ? Operaciones SQLite avanzadas
- ? Uso completo del MVVM Toolkit

---

## ?? Seguridad (Produccion)

?? **IMPORTANTE**: En un entorno de produccion:

1. **NO almacenar passwords en texto plano**
   - Usar hashing (BCrypt, PBKDF2, etc.)
   - Ejemplo: `BCrypt.Net.BCrypt.HashPassword(password)`

2. **Implementar validacion de entrada**
   - Sanitizar datos de usuario
   - Validar longitudes y formatos

3. **Usar HTTPS para APIs**
   - Comunicacion segura con backend

4. **Implementar tokens JWT**
   - Para sesiones persistentes seguras

5. **Cifrar la base de datos SQLite**
   - Usar SQLCipher para bases de datos sensibles

---

## ?? Troubleshooting

### Error: "La base de datos no se crea"
- Verificar permisos de FileSystem.AppDataDirectory
- Asegurarse de que InitializeAsync() se llama

### Error: "No navega despues del login"
- Verificar rutas en AppShell
- Revisar que los servicios esten registrados

### Error: "ViewModel no recibe datos"
- Verificar BindingContext en el code-behind
- Asegurar x:DataType en XAML

---

## ?? Proximas Mejoras Sugeridas

1. ? ~~Implementar patron MVVM~~
2. ? ~~Agregar SQLite~~
3. ? ~~Crear sistema de login~~
4. ? ~~Implementar menu hamburguesa~~
5. ? Agregar paginas CRUD para CartaPorte
6. ? Implementar busqueda y filtros
7. ? Agregar validacion de formularios
8. ? Implementar sincronizacion con API
9. ? Agregar manejo de imagenes
10. ? Implementar notificaciones push
11. ? Agregar modo offline
12. ? Implementar exportacion a PDF

---

## ?? Contribucion

Para contribuir al proyecto:

1. Seguir la estructura MVVM establecida
2. Usar MVVM Toolkit para ViewModels
3. Registrar servicios en MauiProgram.cs
4. Documentar codigo complejo
5. Seguir convenciones de nombres
6. Probar en multiples plataformas

---

## ?? Soporte

Para preguntas o problemas:
1. Revisar `INSTRUCCIONES_MVVM.md`
2. Consultar documentacion de MAUI
3. Revisar ejemplos en el codigo

---

## ?? Licencia

[Especificar licencia del proyecto]

---

**Version:** 1.0.0  
**ultima actualizacion:** 2024  
**Framework:** .NET 10.0  
**Plataformas:** Android, iOS, macOS, Windows

---

? **¡Proyecto listo para desarrollo!** ?

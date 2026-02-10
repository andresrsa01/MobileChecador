# Guía de Despliegue - CheckadorAPI

## Opciones de Despliegue

Esta guía cubre diferentes opciones para desplegar la API en producción.

---

## 1. IIS (Windows Server)

### Requisitos Previos
- Windows Server 2016 o superior
- IIS 10 o superior instalado
- .NET 9.0 Hosting Bundle

### Pasos de Instalación

#### 1.1 Instalar .NET 9.0 Hosting Bundle
```powershell
# Descargar desde: https://dotnet.microsoft.com/download/dotnet/9.0
# Instalar: dotnet-hosting-9.0.x-win.exe
```

#### 1.2 Publicar la Aplicación
```bash
cd CheckadorAPI
dotnet publish -c Release -o C:\inetpub\CheckadorAPI
```

#### 1.3 Crear Application Pool
```powershell
Import-Module WebAdministration

# Crear Application Pool
New-WebAppPool -Name "CheckadorAPIPool"
Set-ItemProperty IIS:\AppPools\CheckadorAPIPool -Name managedRuntimeVersion -Value ""
Set-ItemProperty IIS:\AppPools\CheckadorAPIPool -Name enable32BitAppOnWin64 -Value $false
```

#### 1.4 Crear Sitio Web
```powershell
# Crear directorio si no existe
New-Item -ItemType Directory -Path C:\inetpub\CheckadorAPI -Force

# Crear sitio web
New-WebSite -Name "CheckadorAPI" `
    -Port 80 `
    -PhysicalPath "C:\inetpub\CheckadorAPI" `
    -ApplicationPool "CheckadorAPIPool"
```

#### 1.5 Configurar Permisos
```powershell
$acl = Get-Acl "C:\inetpub\CheckadorAPI"
$permission = "IIS AppPool\CheckadorAPIPool","FullControl","Allow"
$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule $permission
$acl.SetAccessRule($accessRule)
Set-Acl "C:\inetpub\CheckadorAPI" $acl
```

#### 1.6 Configurar web.config
El archivo `web.config` se genera automáticamente, pero puedes personalizarlo:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet" 
                  arguments=".\CheckadorAPI.dll" 
                  stdoutLogEnabled="true" 
                  stdoutLogFile=".\logs\stdout" 
                  hostingModel="inprocess" />
    </system.webServer>
  </location>
</configuration>
```

---

## 2. Azure App Service

### Opción A: Desde Visual Studio

1. **Clic derecho en el proyecto** ? **Publish**
2. **Target**: Azure ? Azure App Service (Windows o Linux)
3. **Seleccionar suscripción** y crear nuevo App Service
4. **Configurar**:
   - Name: checador-api
   - Subscription: Tu suscripción
   - Resource Group: Nuevo o existente
   - Hosting Plan: Crear nuevo
   - Pricing Tier: B1 o superior
5. **Publish**

### Opción B: Azure CLI

```bash
# Login
az login

# Crear Resource Group
az group create --name CheckadorAPI-RG --location eastus

# Crear App Service Plan
az appservice plan create --name CheckadorAPI-Plan \
    --resource-group CheckadorAPI-RG \
    --sku B1 \
    --is-linux

# Crear Web App
az webapp create --name checador-api \
    --resource-group CheckadorAPI-RG \
    --plan CheckadorAPI-Plan \
    --runtime "DOTNET|9.0"

# Deploy
dotnet publish -c Release
cd bin/Release/net9.0/publish
zip -r publish.zip .
az webapp deployment source config-zip \
    --resource-group CheckadorAPI-RG \
    --name checador-api \
    --src publish.zip
```

### Configurar Base de Datos en Azure

```bash
# Crear Azure SQL Server
az sql server create \
    --name checador-sql-server \
    --resource-group CheckadorAPI-RG \
    --location eastus \
    --admin-user sqladmin \
    --admin-password "TuPassword123!"

# Crear Database
az sql db create \
    --resource-group CheckadorAPI-RG \
    --server checador-sql-server \
    --name CheckadorDB \
    --service-objective S0

# Configurar Firewall
az sql server firewall-rule create \
    --resource-group CheckadorAPI-RG \
    --server checador-sql-server \
    --name AllowAzureServices \
    --start-ip-address 0.0.0.0 \
    --end-ip-address 0.0.0.0
```

### Configurar Connection String en Azure

```bash
# Obtener connection string
az sql db show-connection-string \
    --client ado.net \
    --server checador-sql-server \
    --name CheckadorDB

# Configurar en App Service
az webapp config connection-string set \
    --resource-group CheckadorAPI-RG \
    --name checador-api \
    --settings DefaultConnection="Server=tcp:checador-sql-server.database.windows.net,1433;Initial Catalog=CheckadorDB;Persist Security Info=False;User ID=sqladmin;Password=TuPassword123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" \
    --connection-string-type SQLAzure
```

### Configurar Variables de Entorno

```bash
az webapp config appsettings set \
    --resource-group CheckadorAPI-RG \
    --name checador-api \
    --settings Jwt__Key="TuClaveSecretaParaProduccion123456789012345678901234567890"
```

---

## 3. Docker

### Dockerfile (ya incluido)

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["CheckadorAPI.csproj", "./"]
RUN dotnet restore "CheckadorAPI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "CheckadorAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CheckadorAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CheckadorAPI.dll"]
```

### Construir y Ejecutar

```bash
# Construir imagen
docker build -t checador-api:latest .

# Ejecutar contenedor
docker run -d -p 5000:80 -p 5001:443 \
    -e ConnectionStrings__DefaultConnection="Server=host.docker.internal;Database=CheckadorDB;User Id=sa;Password=Password123!;TrustServerCertificate=True" \
    -e Jwt__Key="TuClaveSecretaParaProduccion123456789012345678901234567890" \
    --name checador-api \
    checador-api:latest
```

### Docker Compose

Crear `docker-compose.yml`:

```yaml
version: '3.8'

services:
  api:
    build: .
    ports:
      - "5000:80"
      - "5001:443"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=CheckadorDB;User Id=sa;Password=Password123!;TrustServerCertificate=True
      - Jwt__Key=TuClaveSecretaParaProduccion123456789012345678901234567890
    depends_on:
      - sqlserver
    networks:
      - checador-network

  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=Password123!
    ports:
      - "1433:1433"
    volumes:
      - sqlserver-data:/var/opt/mssql
    networks:
      - checador-network

networks:
  checador-network:
    driver: bridge

volumes:
  sqlserver-data:
```

Ejecutar:
```bash
docker-compose up -d
```

---

## 4. Linux Server (Ubuntu/Debian)

### 4.1 Instalar .NET 9.0

```bash
# Agregar repositorio de Microsoft
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Instalar .NET SDK
sudo apt-get update
sudo apt-get install -y dotnet-sdk-9.0
```

### 4.2 Publicar Aplicación

```bash
# En tu máquina de desarrollo
dotnet publish -c Release -o ./publish -r linux-x64 --self-contained false

# Copiar al servidor
scp -r ./publish/* usuario@servidor:/var/www/checador-api/
```

### 4.3 Crear Servicio Systemd

Crear `/etc/systemd/system/checador-api.service`:

```ini
[Unit]
Description=Checador API
After=network.target

[Service]
Type=notify
WorkingDirectory=/var/www/checador-api
ExecStart=/usr/bin/dotnet /var/www/checador-api/CheckadorAPI.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=checador-api
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment="ConnectionStrings__DefaultConnection=Server=localhost;Database=CheckadorDB;User Id=sa;Password=Password123!;TrustServerCertificate=True"
Environment="Jwt__Key=TuClaveSecretaParaProduccion123456789012345678901234567890"

[Install]
WantedBy=multi-user.target
```

Habilitar y ejecutar:

```bash
sudo systemctl enable checador-api
sudo systemctl start checador-api
sudo systemctl status checador-api
```

### 4.4 Configurar Nginx como Reverse Proxy

Instalar Nginx:
```bash
sudo apt-get install nginx
```

Crear `/etc/nginx/sites-available/checador-api`:

```nginx
server {
    listen 80;
    server_name api.tudominio.com;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

Habilitar sitio:
```bash
sudo ln -s /etc/nginx/sites-available/checador-api /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx
```

### 4.5 Configurar SSL con Let's Encrypt

```bash
# Instalar Certbot
sudo apt-get install certbot python3-certbot-nginx

# Obtener certificado
sudo certbot --nginx -d api.tudominio.com

# Renovación automática
sudo certbot renew --dry-run
```

---

## 5. Configuración de Producción

### appsettings.Production.json

Crear `appsettings.Production.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Error"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=SERVIDOR_PRODUCCION;Database=CheckadorDB;User Id=usuario;Password=password;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "CAMBIAR_POR_CLAVE_SEGURA_DE_PRODUCCION_CON_AL_MENOS_64_CARACTERES",
    "Issuer": "CheckadorAPI",
    "Audience": "CheckadorMobileApp"
  },
  "AllowedHosts": "*"
}
```

### Modificar Program.cs para Producción

```csharp
// Deshabilitar Swagger en producción
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS restrictivo en producción
if (app.Environment.IsProduction())
{
    app.UseCors(builder =>
    {
        builder.WithOrigins("https://tudominio.com")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
}
```

---

## 6. Checklist de Despliegue

Antes de desplegar a producción:

### Seguridad
- [ ] Cambiar JWT Key a una clave fuerte y única
- [ ] Configurar cadena de conexión segura
- [ ] Deshabilitar Swagger
- [ ] Configurar CORS para dominios específicos
- [ ] Configurar HTTPS con certificado válido
- [ ] Cambiar contraseñas por defecto de usuarios
- [ ] Revisar permisos de archivos y directorios
- [ ] Configurar rate limiting
- [ ] Habilitar logging apropiado

### Base de Datos
- [ ] Backup de base de datos configurado
- [ ] Índices optimizados
- [ ] Conexiones con pooling
- [ ] Plan de mantenimiento configurado
- [ ] Monitoreo de performance

### Aplicación
- [ ] Variables de entorno configuradas
- [ ] Logging a archivos o servicio externo
- [ ] Health checks implementados
- [ ] Monitoreo de errores (Application Insights, Sentry, etc.)
- [ ] Documentación de API actualizada
- [ ] Tests ejecutados exitosamente

### Infraestructura
- [ ] Firewall configurado
- [ ] Backup automático configurado
- [ ] Escalamiento planificado
- [ ] Dominio y DNS configurados
- [ ] CDN configurado (si aplica)
- [ ] Balanceador de carga (si aplica)

---

## 7. Monitoreo

### Application Insights (Azure)

```csharp
// Instalar paquete
// dotnet add package Microsoft.ApplicationInsights.AspNetCore

// En Program.cs
builder.Services.AddApplicationInsightsTelemetry();
```

### Serilog

```csharp
// Instalar paquetes
// dotnet add package Serilog.AspNetCore
// dotnet add package Serilog.Sinks.File

// En Program.cs
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
```

---

## 8. Backup y Recuperación

### Backup de Base de Datos SQL Server

```sql
-- Backup completo
BACKUP DATABASE CheckadorDB 
TO DISK = 'C:\Backups\CheckadorDB_Full.bak'
WITH INIT, COMPRESSION;

-- Backup diferencial
BACKUP DATABASE CheckadorDB 
TO DISK = 'C:\Backups\CheckadorDB_Diff.bak'
WITH DIFFERENTIAL, COMPRESSION;
```

### Script PowerShell para Backup Automatizado

```powershell
$date = Get-Date -Format "yyyyMMdd_HHmmss"
$backupPath = "C:\Backups\CheckadorDB_$date.bak"

Invoke-Sqlcmd -Query @"
BACKUP DATABASE CheckadorDB 
TO DISK = '$backupPath'
WITH INIT, COMPRESSION;
"@ -ServerInstance "localhost" -Username "sa" -Password "Password123!"

# Limpiar backups antiguos (más de 7 días)
Get-ChildItem "C:\Backups" -Filter "CheckadorDB_*.bak" | 
    Where-Object { $_.LastWriteTime -lt (Get-Date).AddDays(-7) } | 
    Remove-Item -Force
```

Configurar en Task Scheduler para ejecutar diariamente.

---

## 9. Solución de Problemas en Producción

### Ver Logs en IIS
```powershell
# Habilitar stdout logging en web.config
# Logs estarán en: C:\inetpub\CheckadorAPI\logs\
Get-Content "C:\inetpub\CheckadorAPI\logs\stdout_*.log" -Tail 50
```

### Ver Logs en Linux
```bash
# Ver logs del servicio
sudo journalctl -u checador-api -f

# Ver logs de Nginx
sudo tail -f /var/log/nginx/error.log
```

### Verificar Estado del Servicio
```bash
# Linux
sudo systemctl status checador-api

# Ver puertos en uso
sudo netstat -tulpn | grep :5000
```

---

## 10. Actualizaciones

### Actualizar en IIS
```bash
# 1. Publicar nueva versión
dotnet publish -c Release -o C:\Temp\CheckadorAPI_New

# 2. Detener App Pool
Stop-WebAppPool -Name CheckadorAPIPool

# 3. Reemplazar archivos
Copy-Item "C:\Temp\CheckadorAPI_New\*" "C:\inetpub\CheckadorAPI\" -Recurse -Force

# 4. Iniciar App Pool
Start-WebAppPool -Name CheckadorAPIPool
```

### Actualizar en Linux
```bash
# 1. Detener servicio
sudo systemctl stop checador-api

# 2. Copiar nuevos archivos
sudo cp -r ./publish/* /var/www/checador-api/

# 3. Iniciar servicio
sudo systemctl start checador-api
```

---

## Contacto y Soporte

Para asistencia con el despliegue, contacta al equipo de desarrollo.

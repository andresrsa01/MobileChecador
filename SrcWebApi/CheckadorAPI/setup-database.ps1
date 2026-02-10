# Script de inicialización de la base de datos
# Ejecutar desde la raíz del proyecto CheckadorAPI

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "Checador API - Database Setup" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

# Verificar si dotnet-ef está instalado
Write-Host "Verificando instalación de dotnet-ef..." -ForegroundColor Yellow
$efVersion = dotnet ef --version 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "dotnet-ef no está instalado. Instalando..." -ForegroundColor Red
    dotnet tool install --global dotnet-ef
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Error al instalar dotnet-ef" -ForegroundColor Red
        exit 1
    }
    Write-Host "dotnet-ef instalado correctamente" -ForegroundColor Green
} else {
    Write-Host "dotnet-ef versión: $efVersion" -ForegroundColor Green
}
Write-Host ""

# Eliminar base de datos existente (opcional)
Write-Host "¿Deseas eliminar la base de datos existente? (S/N): " -ForegroundColor Yellow -NoNewline
$response = Read-Host
if ($response -eq "S" -or $response -eq "s") {
    Write-Host "Eliminando base de datos..." -ForegroundColor Yellow
    dotnet ef database drop --force
    Write-Host "Base de datos eliminada" -ForegroundColor Green
}
Write-Host ""

# Aplicar migraciones
Write-Host "Aplicando migraciones..." -ForegroundColor Yellow
dotnet ef database update
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error al aplicar migraciones" -ForegroundColor Red
    exit 1
}
Write-Host "Migraciones aplicadas correctamente" -ForegroundColor Green
Write-Host ""

# Mostrar información de usuarios por defecto
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "Base de datos inicializada" -ForegroundColor Green
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Usuarios creados:" -ForegroundColor Yellow
Write-Host "  1. Administrador" -ForegroundColor White
Write-Host "     Usuario: admin" -ForegroundColor White
Write-Host "     Contraseña: Admin123!" -ForegroundColor White
Write-Host ""
Write-Host "  2. Usuario de prueba" -ForegroundColor White
Write-Host "     Usuario: usuario1" -ForegroundColor White
Write-Host "     Contraseña: User123!" -ForegroundColor White
Write-Host "     Geofence: Oficina Central (Zócalo CDMX)" -ForegroundColor White
Write-Host ""
Write-Host "Para iniciar la API, ejecuta: dotnet run" -ForegroundColor Cyan
Write-Host "La API estará disponible en: https://localhost:5001" -ForegroundColor Cyan
Write-Host "Documentación Swagger: https://localhost:5001/swagger" -ForegroundColor Cyan
Write-Host ""

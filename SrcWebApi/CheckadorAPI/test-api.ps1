# API Testing Script
# Este script contiene ejemplos de llamadas a la API usando curl

$baseUrl = "https://localhost:5001"

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "Checador API - Testing Script" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

# 1. Login como administrador
Write-Host "1. Login como Administrador..." -ForegroundColor Yellow
$loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" `
    -Method Post `
    -ContentType "application/json" `
    -Body (@{
        username = "admin"
        password = "Admin123!"
    } | ConvertTo-Json) `
    -SkipCertificateCheck

if ($loginResponse.success) {
    Write-Host "   ? Login exitoso" -ForegroundColor Green
    $adminToken = $loginResponse.token
    Write-Host "   Token: $($adminToken.Substring(0, 30))..." -ForegroundColor Gray
} else {
    Write-Host "   ? Login fallido: $($loginResponse.message)" -ForegroundColor Red
    exit 1
}
Write-Host ""

# 2. Obtener lista de usuarios
Write-Host "2. Obteniendo lista de usuarios..." -ForegroundColor Yellow
try {
    $users = Invoke-RestMethod -Uri "$baseUrl/api/users" `
        -Method Get `
        -Headers @{
            "Authorization" = "Bearer $adminToken"
        } `
        -SkipCertificateCheck

    Write-Host "   ? Usuarios obtenidos: $($users.Count)" -ForegroundColor Green
    foreach ($user in $users) {
        Write-Host "     - $($user.username) ($($user.role))" -ForegroundColor Gray
    }
} catch {
    Write-Host "   ? Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# 3. Login como usuario normal
Write-Host "3. Login como Usuario..." -ForegroundColor Yellow
$userLoginResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" `
    -Method Post `
    -ContentType "application/json" `
    -Body (@{
        username = "usuario1"
        password = "User123!"
    } | ConvertTo-Json) `
    -SkipCertificateCheck

if ($userLoginResponse.success) {
    Write-Host "   ? Login exitoso" -ForegroundColor Green
    $userToken = $userLoginResponse.token
    $userId = $userLoginResponse.user.id
    
    if ($userLoginResponse.geofenceConfig) {
        Write-Host "   Geofence: $($userLoginResponse.geofenceConfig.locationName)" -ForegroundColor Gray
        Write-Host "   Radio: $($userLoginResponse.geofenceConfig.radiusInMeters)m" -ForegroundColor Gray
    }
} else {
    Write-Host "   ? Login fallido: $($userLoginResponse.message)" -ForegroundColor Red
}
Write-Host ""

# 4. Registrar asistencia (dentro del geofence)
Write-Host "4. Registrando asistencia..." -ForegroundColor Yellow
try {
    $attendanceResponse = Invoke-RestMethod -Uri "$baseUrl/api/attendance/register" `
        -Method Post `
        -ContentType "application/json" `
        -Headers @{
            "Authorization" = "Bearer $userToken"
        } `
        -Body (@{
            userId = $userId
            latitude = 19.432608
            longitude = -99.133209
            timestamp = (Get-Date).ToUniversalTime().ToString("o")
        } | ConvertTo-Json) `
        -SkipCertificateCheck

    if ($attendanceResponse.success) {
        Write-Host "   ? Asistencia registrada: ID $($attendanceResponse.attendanceId)" -ForegroundColor Green
    } else {
        Write-Host "   ? Error: $($attendanceResponse.message)" -ForegroundColor Red
    }
} catch {
    Write-Host "   ? Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# 5. Obtener asistencias del usuario
Write-Host "5. Obteniendo asistencias del usuario..." -ForegroundColor Yellow
try {
    $attendances = Invoke-RestMethod -Uri "$baseUrl/api/attendance/user/$userId" `
        -Method Get `
        -Headers @{
            "Authorization" = "Bearer $userToken"
        } `
        -SkipCertificateCheck

    Write-Host "   ? Asistencias encontradas: $($attendances.Count)" -ForegroundColor Green
    foreach ($att in $attendances) {
        Write-Host "     - ID: $($att.id), Fecha: $($att.timestamp)" -ForegroundColor Gray
    }
} catch {
    Write-Host "   ? Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# 6. Obtener asistencias del día (como admin)
Write-Host "6. Obteniendo asistencias del día (Admin)..." -ForegroundColor Yellow
try {
    $todayAttendances = Invoke-RestMethod -Uri "$baseUrl/api/attendance/today" `
        -Method Get `
        -Headers @{
            "Authorization" = "Bearer $adminToken"
        } `
        -SkipCertificateCheck

    Write-Host "   ? Asistencias de hoy: $($todayAttendances.Count)" -ForegroundColor Green
} catch {
    Write-Host "   ? Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "Testing completado" -ForegroundColor Green
Write-Host "==================================" -ForegroundColor Cyan

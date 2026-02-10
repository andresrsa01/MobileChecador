namespace MauiAppChecador.Helpers;

public static class GeofenceHelper
{
    /// <summary>
    /// Calcula la distancia en metros entre dos puntos geográficos usando la fórmula de Haversine
    /// </summary>
    public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double earthRadiusKm = 6371.0;

        var dLat = DegreesToRadians(lat2 - lat1);
        var dLon = DegreesToRadians(lon2 - lon1);

        lat1 = DegreesToRadians(lat1);
        lat2 = DegreesToRadians(lat2);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c * 1000; // Convertir a metros
    }

    /// <summary>
    /// Verifica si una ubicación está dentro del geofence
    /// </summary>
    public static bool IsWithinGeofence(double currentLat, double currentLon, double centerLat, double centerLon, double radiusInMeters)
    {
        var distance = CalculateDistance(currentLat, currentLon, centerLat, centerLon);
        return distance <= radiusInMeters;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}

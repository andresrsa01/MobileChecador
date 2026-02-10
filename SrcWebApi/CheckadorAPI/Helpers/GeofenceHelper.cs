namespace CheckadorAPI.Helpers;

public static class GeofenceHelper
{
    private const double EarthRadiusKm = 6371.0;

    /// <summary>
    /// Calcula la distancia entre dos coordenadas usando la fórmula de Haversine
    /// </summary>
    public static double CalculateDistance(
        double lat1, double lon1,
        double lat2, double lon2)
    {
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        var distanceInKm = EarthRadiusKm * c;
        return distanceInKm * 1000; // Convertir a metros
    }

    /// <summary>
    /// Verifica si una ubicación está dentro del geofence
    /// </summary>
    public static bool IsWithinGeofence(
        double currentLat, double currentLon,
        double centerLat, double centerLon,
        double radiusInMeters)
    {
        var distance = CalculateDistance(currentLat, currentLon, centerLat, centerLon);
        return distance <= radiusInMeters;
    }

    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}

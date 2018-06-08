using System;
using System.Collections.Generic;
using Tabi.DataObjects;

namespace Tabi
{
    public static class Util
    {
        public static PositionEntry AveragePosition(IList<PositionEntry> positions)
        {
            if (positions.Count == 1)
            {
                return positions[0];
            }

            double x = 0;
            double y = 0;
            double z = 0;

            foreach (var position in positions)
            {
                var latitude = position.Latitude * Math.PI / 180;
                var longitude = position.Longitude * Math.PI / 180;

                x += Math.Cos(latitude) * Math.Cos(longitude);
                y += Math.Cos(latitude) * Math.Sin(longitude);
                z += Math.Sin(latitude);
            }

            var total = positions.Count;

            x = x / total;
            y = y / total;
            z = z / total;

            var centralLongitude = Math.Atan2(y, x);
            var centralSquareRoot = Math.Sqrt(x * x + y * y);
            var centralLatitude = Math.Atan2(z, centralSquareRoot);

            return new PositionEntry() { Latitude = centralLatitude * 180 / Math.PI, Longitude = centralLongitude * 180 / Math.PI };
        }

        public static double DistanceBetween(double lat1, double lon1, double lat2, double lon2)
        {
            if (double.IsNaN(lat1) || double.IsNaN(lon1) || double.IsNaN(lat2) ||
                double.IsNaN(lon2))
            {
                throw new ArgumentException("Argument latitude or longitude is not a number");
            }

            var d1 = lat1 * (Math.PI / 180.0);
            var num1 = lon1 * (Math.PI / 180.0);
            var d2 = lat2 * (Math.PI / 180.0);
            var num2 = lon2 * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                     Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }

        public static (double latitude, double longitude) AddMetersToPosition(double latitude, double longitude,
           double distance)
        {
            const int radius = 6378137;
            double dLat = distance / radius;
            double dLon = distance / (radius * Math.Cos(Math.PI * latitude / 180));
            return (latitude + dLat * 180 / Math.PI, longitude + dLon * 180 / Math.PI);
        }
    }
}

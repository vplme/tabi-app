using CoreLocation;
using Tabi.DataObjects;

namespace Tabi.iOS.Helpers
{
    public static class CLLocationExtensionMethods
    {
        public static PositionEntry ToPosition(this CLLocation location)
        {
            PositionEntry p = new PositionEntry();
            p.Accuracy = location.HorizontalAccuracy;
            p.Latitude = location.Coordinate.Latitude;
            p.Longitude = location.Coordinate.Longitude;

            p.Speed = location.Speed;
            p.Course = location.Course;

            p.Altitude = location.Altitude;
            p.VerticalAccuracy = location.VerticalAccuracy;

            p.Timestamp = location.Timestamp.ToDateTime();

            return p;
        }

    }
}

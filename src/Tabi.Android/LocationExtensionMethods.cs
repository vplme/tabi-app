using Android.Locations;
using Tabi.DataObjects;

namespace Tabi.Droid
{
    public static class LocationExtensionMethods
    {
        public static PositionEntry ToPositionEntry(this Location location)
        {
			PositionEntry p = new PositionEntry();
			p.Accuracy = location.Accuracy;
			p.Latitude = location.Latitude;
			p.Longitude = location.Longitude;
            p.Course = location.Bearing;

            p.Altitude = location.HasAltitude ? location.Altitude : 0;

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                p.VerticalAccuracy = location.HasVerticalAccuracy ? location.VerticalAccuracyMeters : 0;
            }

            p.Speed = location.Speed;
	        p.Timestamp = Util.TimeLongToDateTime(location.Time);
            return p;
        }
    }
}

using System;
using Android.Locations;
using Tabi.Model;

namespace Tabi.Droid
{
    public static class LocationExtensionMethods
    {
        public static Position ToPosition(this Location location)
        {
			Position p = new Position();
			p.Accuracy = location.Accuracy;
			p.Latitude = location.Latitude;
			p.Longitude = location.Longitude;
			p.Speed = location.Speed;
            p.Timestamp = Util.TimeLongToDateTime(location.Time);
            return p;
        }
    }
}

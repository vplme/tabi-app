using System;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Tabi.Model;

namespace Tabi.Droid
{
    public class LocationListener : Java.Lang.Object, Android.Gms.Location.ILocationListener, Android.Locations.ILocationListener
    {

        public LocationListener()
        {
        }


        public void OnLocationChanged(Location location)
        {
            Position p = location.ToPosition();
        }

        public void OnProviderDisabled(string provider)
        {
            
        }

        public void OnProviderEnabled(string provider)
        {
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
        }
    }
}

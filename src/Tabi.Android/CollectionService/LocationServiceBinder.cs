using Android.OS;

namespace Tabi.Droid
{
    public class LocationServiceBinder : Binder
    {
        LocationService service;

        public LocationServiceBinder(LocationService service)
        {
            this.service = service;
        }

        public LocationService GetLocationService()
        {
            return service;
        }

    }
}

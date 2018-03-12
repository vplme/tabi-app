using Android.OS;

namespace Tabi.Droid.CollectionService
{
    public class SensorServiceBinder : Binder
    {
        public SensorService Service { get; }

        public SensorServiceBinder(SensorService service)
        {
            Service = service;
        }

    }
}
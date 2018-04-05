using Android.OS;

namespace Tabi.Droid.CollectionService
{
    public class SensorMeasurementSessionServiceBinder : Binder
    {
        public SensorMeasurementSessionService Service { get; }

        public SensorMeasurementSessionServiceBinder(SensorMeasurementSessionService service)
        {
            Service = service;
        }
    }
}
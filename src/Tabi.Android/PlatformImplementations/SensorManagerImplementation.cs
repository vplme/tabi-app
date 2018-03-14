using Android.App;
using Android.Content;
using Tabi.Droid.CollectionService;
using Tabi.Droid.PlatformImplementations;
using Tabi.Shared.Sensors;

[assembly: Xamarin.Forms.Dependency(typeof(SensorManagerImplementation))]
namespace Tabi.Droid.PlatformImplementations
{

    class SensorManagerImplementation : ISensorManager
    {
        public bool IsListening { get; private set; }

        public void StartSensorUpdates()
        {
            // service for accelerometer, gyroscope and magnetometer (reading < 1 second)
            Intent sensorServiceIntent = new Intent(Application.Context, typeof(SensorService));
            Application.Context.StartService(sensorServiceIntent);

            //service for measurements periodically -> 1 minute
            Intent sensorMeasurementSessionServiceIntent = new Intent(Application.Context, typeof(SensorMeasurementSessionService));
            Application.Context.StartService(sensorMeasurementSessionServiceIntent);


            IsListening = true;
        }

        public void StopSensorUpdates()
        {
            //stop sensorService
            Intent sensorServiceIntent = new Intent(Application.Context, typeof(SensorService));
            Application.Context.StopService(sensorServiceIntent);

            //stop sensorMeasurementSessionServiceIntent
            Intent sensorMeasurementSessionServiceIntent = new Intent(Application.Context, typeof(SensorMeasurementSessionService));
            Application.Context.StopService(sensorMeasurementSessionServiceIntent);

            IsListening = false;
        }
    }
}
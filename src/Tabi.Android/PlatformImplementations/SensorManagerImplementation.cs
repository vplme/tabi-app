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
            
            //service for sensorfusion data (reading < 1 second)
            Intent sensorFusionServiceIntent = new Intent(Application.Context, typeof(SensorFusionService));
            
            //service for measurements periodically -> 1 minute
            Intent sensorMeasurementSessionServiceIntent = new Intent(Application.Context, typeof(SensorMeasurementSessionService));
            
            Intent assignSensorDataToTrackServiceIntent = new Intent(Application.Context, typeof(AssignSensorDataToTrackService));
            

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                Application.Context.StartForegroundService(sensorServiceIntent);
                Application.Context.StartForegroundService(sensorFusionServiceIntent);
                Application.Context.StartForegroundService(sensorMeasurementSessionServiceIntent);
                Application.Context.StartForegroundService(assignSensorDataToTrackServiceIntent);
            }
            else
            {
                Application.Context.StartService(sensorServiceIntent);
                Application.Context.StartService(sensorFusionServiceIntent);
                Application.Context.StartService(sensorMeasurementSessionServiceIntent);
                Application.Context.StartService(assignSensorDataToTrackServiceIntent);
            }

            IsListening = true;
        }

        public void StopSensorUpdates()
        {
            //stop sensorService
            Intent sensorServiceIntent = new Intent(Application.Context, typeof(SensorService));
            Application.Context.StopService(sensorServiceIntent);

            //stop service for sensorfusion data
            Intent sensorFusionServiceIntent = new Intent(Application.Context, typeof(SensorFusionService));
            Application.Context.StopService(sensorFusionServiceIntent);

            //stop sensorMeasurementSessionServiceIntent
            Intent sensorMeasurementSessionServiceIntent = new Intent(Application.Context, typeof(SensorMeasurementSessionService));
            Application.Context.StopService(sensorMeasurementSessionServiceIntent);

            //stop service for assigning sensordata to tracks
            Intent assignSensorDataToTrackServiceIntent = new Intent(Application.Context, typeof(AssignSensorDataToTrackService));
            Application.Context.StopService(assignSensorDataToTrackServiceIntent);

            IsListening = false;
        }
    }
}
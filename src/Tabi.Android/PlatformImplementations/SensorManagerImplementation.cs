using System;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
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

            

            //this code was to try periods by using alarmManager -> replaced by timer in service 
            //AlarmManager scheduler = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
            //Intent intent = new Intent(Application.Context, typeof(SensorMeasurementSessionService) );
            //PendingIntent scheduledIntent = PendingIntent.GetService(Application.Context, 0, intent, PendingIntentFlags.UpdateCurrent);
            //scheduler.SetInexactRepeating(AlarmType.RtcWakeup, DateTime.Now.Millisecond, TimeSpan.FromMinutes(1),scheduledIntent);

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
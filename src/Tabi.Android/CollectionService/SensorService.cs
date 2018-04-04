using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Hardware;
using Tabi.DataStorage;
using Tabi.DataObjects;
using System.Threading.Tasks;
using static Android.OS.PowerManager;

namespace Tabi.Droid.CollectionService
{
    [Service]
    public class SensorService : Service, ISensorEventListener
    {
        private readonly ISensorRepository<Accelerometer> _accelerometerRepository;
        private readonly ISensorRepository<Gyroscope> _gyroscopeRepository;
        private readonly ISensorRepository<Magnetometer> _magnetometerRepository;

        private SensorServiceBinder _binder;

        public SensorService()
        {
            _accelerometerRepository = App.RepoManager.AccelerometerRepository;
            _gyroscopeRepository = App.RepoManager.GyroscopeRepository;
            _magnetometerRepository = App.RepoManager.MagnetometerRepository;
        }


        public override IBinder OnBind(Intent intent)
        {
            // service binder is used to communicate with the service
            _binder = new SensorServiceBinder(this);
            return _binder;
        }

        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                //!!!possibly needed for android 8+
                // maybe convert it into foreground service and uncomment code below
                Notification.Builder builder = new Notification.Builder(Application.Context, "com.tabi.sensor");
                builder.SetContentTitle("sensor");
                builder.SetContentText("sensor service");
                builder.SetAutoCancel(true);

                Notification notification = builder.Build();
                StartForeground(1, notification);
            }

            PowerManager sv = (Android.OS.PowerManager)GetSystemService(PowerService);
            WakeLock wklock = sv.NewWakeLock(WakeLockFlags.Partial, "TABI_sensor_service");
            wklock.Acquire();

            //register sensors for listening
            var sensorManager = (SensorManager)Application.Context.GetSystemService(Context.SensorService);

                Sensor accelerometer = sensorManager.GetDefaultSensor(SensorType.Accelerometer);
                sensorManager.RegisterListener(this, accelerometer, SensorDelay.Normal);

                Sensor gyroscope = sensorManager.GetDefaultSensor(SensorType.Gyroscope);
                sensorManager.RegisterListener(this, gyroscope, SensorDelay.Normal);

                Sensor magnetometer = sensorManager.GetDefaultSensor(SensorType.MagneticField);
                sensorManager.RegisterListener(this, magnetometer, SensorDelay.Normal);

            return StartCommandResult.Sticky;
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            switch (sensor.Type)
            {
                case SensorType.Accelerometer:
                    Console.WriteLine("Accelerometer accuracy: " + accuracy);
                    break;
                case SensorType.Gyroscope:
                    Console.WriteLine("Gyroscope accuracy: " + accuracy);
                    break;
                case SensorType.MagneticField:
                    Console.WriteLine("Magnetometer accuracy: " + accuracy);
                    break;
                default:
                    break;
            }
        }

        public void OnSensorChanged(SensorEvent e)
        {
                //start gathering data and push to SqliteDB
                switch (e.Sensor.Type)
                {
                    case SensorType.Accelerometer:
                        Task.Run(() =>
                        {
                            _accelerometerRepository.Add(new Accelerometer()
                            {
                                Timestamp = DateTimeOffset.Now,
                                X = e.Values[0],
                                Y = e.Values[1],
                                Z = e.Values[2],
                            });
                        });
                        break;
                    case SensorType.Gyroscope:
                        Task.Run(() =>
                        {
                            _gyroscopeRepository.Add(new Gyroscope()
                            {
                                Timestamp = DateTimeOffset.Now,
                                X = e.Values[0],
                                Y = e.Values[1],
                                Z = e.Values[2],
                            });
                        });
                        break;
                    case SensorType.MagneticField:
                        Task.Run(() =>
                        {
                            _magnetometerRepository.Add(new Magnetometer()
                            {
                                Timestamp = DateTimeOffset.Now,
                                X = e.Values[0],
                                Y = e.Values[1],
                                Z = e.Values[2],
                            });
                        });
                        break;
                    default:
                        break;
                }
        }
    }
}
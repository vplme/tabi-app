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
using Autofac;

namespace Tabi.Droid.CollectionService
{
    [Service]
    public class SensorService : Service, ISensorEventListener
    {
        private readonly IRepoManager _repoManager;
        private readonly ISensorRepository<Accelerometer> _accelerometerRepository;
        private readonly ISensorRepository<Gyroscope> _gyroscopeRepository;
        private readonly ISensorRepository<Magnetometer> _magnetometerRepository;
        private DateTime _startTimestamp;

        private SensorServiceBinder _binder;

        public SensorService()
        {
            _repoManager = App.Container.Resolve<IRepoManager>();

            _accelerometerRepository = _repoManager.AccelerometerRepository;
            _gyroscopeRepository = _repoManager.GyroscopeRepository;
            _magnetometerRepository = _repoManager.MagnetometerRepository;
        }


        public override IBinder OnBind(Intent intent)
        {
            // service binder is used to communicate with the service
            _binder = new SensorServiceBinder(this);
            return _binder;
        }

        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            Log.Info("Sensor Service started");

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

            _startTimestamp = DateTime.Now;

            return StartCommandResult.Sticky;
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            switch (sensor.Type)
            {
                case SensorType.Accelerometer:
                    Log.Debug("Accelerometer accuracy: " + accuracy);
                    break;
                case SensorType.Gyroscope:
                    Log.Debug("Gyroscope accuracy: " + accuracy);
                    break;
                case SensorType.MagneticField:
                    Log.Debug("Magnetometer accuracy: " + accuracy);
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

        public override void OnDestroy()
        {
            base.OnDestroy();

            Log.Info($"Sensor Service was destroyed. Ran for: {DateTime.Now - _startTimestamp}");
        }
    }
}
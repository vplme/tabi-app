using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Hardware;
using Tabi.DataStorage;

namespace Tabi.Droid.CollectionService
{
    [Service]
    public class SensorService : Service, ISensorEventListener
    {
        private readonly IAccelerometerRepository _accelerometerRepository;
        private readonly IGyroscopeRepository _gyroscopeRepository;
        private readonly IMagnetometerRepository _magnetometerRepository;

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
            //register sensors for listening
            
            var sensorManager = (SensorManager)Application.Context.GetSystemService(Context.SensorService);

            Sensor accelerometer = sensorManager.GetDefaultSensor(SensorType.Accelerometer);
            sensorManager.RegisterListener(this, accelerometer, SensorDelay.Normal);

            Sensor gyroscope = sensorManager.GetDefaultSensor(SensorType.Gyroscope);
            sensorManager.RegisterListener(this, gyroscope, SensorDelay.Normal);

            Sensor magnetometer = sensorManager.GetDefaultSensor(SensorType.MagneticField);
            sensorManager.RegisterListener(this, magnetometer, SensorDelay.Normal);


            //sensor fusion
            Sensor rotationVector = sensorManager.GetDefaultSensor(SensorType.RotationVector);
            sensorManager.RegisterListener(this, rotationVector, SensorDelay.Normal);
            
            Sensor linearAccSensor = sensorManager.GetDefaultSensor(SensorType.LinearAcceleration);
            sensorManager.RegisterListener(this, linearAccSensor, SensorDelay.Normal);

            Sensor gravitySensor = sensorManager.GetDefaultSensor(SensorType.Gravity);
            sensorManager.RegisterListener(this, gravitySensor, SensorDelay.Normal);

            

            
            //var significantMotionSensor = sensorManager.GetDefaultSensor(SensorType.SignificantMotion);

            return StartCommandResult.Sticky;
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            
        }

        public void OnSensorChanged(SensorEvent e)
        {
            //start gathering data and push to SqliteDB
            switch (e.Sensor.Type)
            {
                case SensorType.Accelerometer:
                    _accelerometerRepository.Add(new DataObjects.Accelerometer()
                    {
                        Timestamp = DateTimeOffset.Now,
                        X = e.Values[0],
                        Y = e.Values[1],
                        Z = e.Values[2],
                    });
                    break;
                case SensorType.Gyroscope:
                    _gyroscopeRepository.Add(new DataObjects.Gyroscope()
                    {
                        Timestamp = DateTimeOffset.Now,
                        X = e.Values[0],
                        Y = e.Values[1],
                        Z = e.Values[2],
                    });
                    break;
                case SensorType.MagneticField:
                    _magnetometerRepository.Add(new DataObjects.Magnetometer()
                    {
                        Timestamp = DateTimeOffset.Now,
                        X = e.Values[0],
                        Y = e.Values[1],
                        Z = e.Values[2],
                    });
                    break;
                case SensorType.RotationVector:
                    
                    break;
                case SensorType.Gravity:
                    break;
                case SensorType.LinearAcceleration:
                    break;
                default:
                    break;
            }
        }

        void SessionMeasurementTrigger(TriggerEvent e)
        {

        }
    }
}
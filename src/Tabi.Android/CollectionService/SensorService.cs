using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Hardware;
using Android.Views;
using Android.Widget;
using Tabi.DataStorage;
using Tabi.DataStorage.SqliteNet;
using System.Threading;

namespace Tabi.Droid.CollectionService
{
    [Service]
    public class SensorService : Service, ISensorEventListener
    {
        private readonly IAccelerometerRepository _accelerometerRepository;
        private readonly IGyroscopeRepository _gyroscopeRepository;
        private readonly IMagnetometerRepository _magnetometerRepository;

        SensorServiceBinder binder;

        public SensorService()
        {
            _accelerometerRepository = App.RepoManager.AccelerometerRepository;
            _gyroscopeRepository = App.RepoManager.GyroscopeRepository;
            _magnetometerRepository = App.RepoManager.MagnetometerRepository;
        }


        public override IBinder OnBind(Intent intent)
        {
            // service binder is used to communicate with the service
            binder = new SensorServiceBinder(this);
            return binder;
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
            
            //var linearAccSensor = sensorManager.GetDefaultSensor(SensorType.LinearAcceleration);
            //var gravitySensor = sensorManager.GetDefaultSensor(SensorType.Gravity);
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
                        Xvalue = e.Values[1],
                        Yvalue = e.Values[2],
                        Zvalue = e.Values[3],
                    });
                    break;
                case SensorType.GeomagneticRotationVector:
                    break;
                case SensorType.Gravity:
                    
                    break;
                case SensorType.Gyroscope:
                    _gyroscopeRepository.Add(new DataObjects.Gyroscope()
                    {
                        Timestamp = DateTimeOffset.Now,
                        Xvalue = e.Values[1],
                        Yvalue = e.Values[2],
                        Zvalue = e.Values[3],
                    });
                    break;
                case SensorType.GyroscopeUncalibrated:
                    break;
                case SensorType.LinearAcceleration:
                    break;
                case SensorType.LowLatencyOffbodyDetect:
                    break;
                case SensorType.MagneticField:
                    _magnetometerRepository.Add(new DataObjects.Magnetometer()
                    {
                        Timestamp = DateTimeOffset.Now,
                        Xvalue = e.Values[1],
                        Yvalue = e.Values[2],
                        Zvalue = e.Values[3],
                    });
                    break;
                case SensorType.MagneticFieldUncalibrated:
                    break;
                case SensorType.MotionDetect:
                    break;
                case SensorType.RelativeHumidity:
                    break;
                case SensorType.RotationVector:
                    break;
                case SensorType.SignificantMotion:
                    break;
                case SensorType.StationaryDetect:
                    break;
                default:
                    break;
            }
            throw new NotImplementedException();
        }

        void SessionMeasurementTrigger(TriggerEvent e)
        {

        }
    }
}
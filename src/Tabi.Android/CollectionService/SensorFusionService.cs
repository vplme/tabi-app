using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Tabi.DataObjects;
using Tabi.DataStorage;

namespace Tabi.Droid.CollectionService
{
    [Service]
    public class SensorFusionService : Service, ISensorEventListener
    {
        private readonly ISensorRepository<LinearAcceleration> _linearAccelerationRepository;
        private readonly ISensorRepository<Orientation> _orientationRepository;
        private readonly ISensorRepository<Quaternion> _quaternionRepository;
        private readonly ISensorRepository<Gravity> _gravityRepository;

        private SensorFusionServiceBinder _binder; 

        public SensorFusionService()
        {
            _linearAccelerationRepository = App.RepoManager.LinearAccelerationRepository;
            _orientationRepository = App.RepoManager.OrientationRepository;
            _quaternionRepository = App.RepoManager.QuaternionRepository;
            _gravityRepository = App.RepoManager.GravityRepository;
        }

        public override IBinder OnBind(Intent intent)
        {
            // service binder is used to communicate with the service
            _binder = new SensorFusionServiceBinder(this);
            return _binder;
        }

        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            Task.Run(() =>
            {
                var sensorManager = (SensorManager)Application.Context.GetSystemService(Context.SensorService);
                //sensor fusion
                //linear acceleration
                Sensor linearAcceleration = sensorManager.GetDefaultSensor(SensorType.LinearAcceleration);
                sensorManager.RegisterListener(this, linearAcceleration, SensorDelay.Normal);

                //gravity
                Sensor gravity = sensorManager.GetDefaultSensor(SensorType.Gravity);
                sensorManager.RegisterListener(this, gravity, SensorDelay.Normal);

                //pitch yaw roll / orientation
                Sensor orientation = sensorManager.GetDefaultSensor(SensorType.Orientation);
                sensorManager.RegisterListener(this, orientation, SensorDelay.Normal);

                //quaternion
                Sensor rotationVector = sensorManager.GetDefaultSensor(SensorType.RotationVector);
                sensorManager.RegisterListener(this, rotationVector, SensorDelay.Normal);
            });

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
                case SensorType.Orientation:
                    Task.Run(() =>
                    {
                        _orientationRepository.Add(new Orientation()
                        {
                            Timestamp = DateTimeOffset.Now,
                            X = e.Values[0],
                            Y = e.Values[1],
                            Z = e.Values[2]
                        });
                    });
                    break;
                case SensorType.Gravity:
                    Task.Run(() =>
                    {
                        _gravityRepository.Add(new Gravity()
                        {
                            Timestamp = DateTimeOffset.Now,
                            X = e.Values[0],
                            Y = e.Values[1],
                            Z = e.Values[2]
                        });
                    });
                    break;
                case SensorType.LinearAcceleration:
                    Task.Run(() =>
                    {
                        _linearAccelerationRepository.Add(new LinearAcceleration()
                        {
                            Timestamp = DateTimeOffset.Now,
                            X = e.Values[0],
                            Y = e.Values[1],
                            Z = e.Values[2]
                        });
                    });
                    break;
                case SensorType.RotationVector:
                    Task.Run(() =>
                    {
                        _quaternionRepository.Add(new Quaternion
                        {
                            Timestamp = DateTimeOffset.Now,
                            X = e.Values[0],
                            Y = e.Values[1],
                            Z = e.Values[2],
                            W = e.Values[3]
                        });
                    });
                    break;
                default:
                    break;
            }
        }
    }
}
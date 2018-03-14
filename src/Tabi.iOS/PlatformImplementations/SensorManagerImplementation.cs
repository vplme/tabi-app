using System;
using System.Timers;
using CoreLocation;
using CoreMotion;
using Foundation;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.iOS.PlatformImplementations;
using Tabi.Shared.Sensors;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(SensorManagerImplementation))]
namespace Tabi.iOS.PlatformImplementations
{
    public class SensorManagerImplementation : ISensorManager
    {
        private readonly ISensorMeasurementSessionRepository _sensorMeasurementSessionRepository;
        private readonly IHeadingRepository _headingRepository;
        private readonly ISensorRepository<Accelerometer> _accelerometerRepository;
        private readonly ISensorRepository<Gyroscope> _gyroscoperRepository;
        private readonly ISensorRepository<Magnetometer> _magnetoometerRepository;
        private readonly ISensorRepository<LinearAcceleration> _linearAccelerationRepository;
        private readonly ISensorRepository<RotationVector> _rotationVectorRepository;
        private readonly ISensorRepository<Quaternion> _quaternionRepository;
        private readonly ISensorRepository<Gravity> _gravityRepository;

        private readonly CMMotionManager _cMMotionManager;
        private readonly CMPedometer _cMPedometer;
        private readonly CLLocationManager _cLLocationManager;
        

        public bool IsListening { get; private set; }

        public SensorManagerImplementation()
        {
            _sensorMeasurementSessionRepository = App.RepoManager.SensorMeasurementSessionRepository;
            _headingRepository = App.RepoManager.HeadingRepository;
            _accelerometerRepository = App.RepoManager.AccelerometerRepository;
            _gyroscoperRepository = App.RepoManager.GyroscopeRepository;
            _magnetoometerRepository = App.RepoManager.MagnetometerRepository;
            _linearAccelerationRepository = App.RepoManager.LinearAccelerationRepository;
            _rotationVectorRepository = App.RepoManager.RotationVectorRepository;
            _quaternionRepository = App.RepoManager.QuaternionRepository;
            _gravityRepository = App.RepoManager.GravityRepository;

            _cMMotionManager = new CMMotionManager()
            {
                DeviceMotionUpdateInterval = 0.1
            };

            _cMPedometer = new CMPedometer();

            _cLLocationManager = new CLLocationManager()
            {
                DesiredAccuracy = CLLocation.AccuracyBest,
                HeadingFilter = 1
            };

            StartSensorUpdates();
        }

        public void StartSensorUpdates()
        {
            //get motion sensors
            //Raw accelerometer
            _cMMotionManager.StartAccelerometerUpdates(NSOperationQueue.CurrentQueue, (data, error) =>
            {
                _accelerometerRepository.Add(new Accelerometer()
                {
                    Timestamp = DateTimeOffset.Now,
                    X = Convert.ToSingle(data.Acceleration.X),
                    Y = Convert.ToSingle(data.Acceleration.Y),
                    Z = Convert.ToSingle(data.Acceleration.Z)
                });
            });

            //gyroscope
            _cMMotionManager.StartGyroUpdates(NSOperationQueue.CurrentQueue, (data, error) =>
            {

                _gyroscoperRepository.Add(new Gyroscope()
                {
                    Timestamp = DateTimeOffset.Now,
                    X= Convert.ToSingle(data.RotationRate.x),
                    Y= Convert.ToSingle(data.RotationRate.y),
                    Z= Convert.ToSingle(data.RotationRate.z)
                });
            });

            //Raw magnetometer
            //_cMMotionManager.StartMagnetometerUpdates(NSOperationQueue.CurrentQueue, (data, error) =>
            //{
            //    _magnetoometerRepository.Add(new Magnetometer()
            //    {
            //        Timestamp = DateTimeOffset.Now,
            //        X= Convert.ToSingle(data.MagneticField.X),
            //        Y= Convert.ToSingle(data.MagneticField.Y),
            //        Z= Convert.ToSingle(data.MagneticField.Z)
            //    });
            //});

            
            _cMMotionManager.StartDeviceMotionUpdates(NSOperationQueue.CurrentQueue, (data, error) =>
            {
                //Linear acceleration (acceleration - gravity)
                DateTimeOffset timestamp = DateTimeOffset.Now;

                _accelerometerRepository.Add(new Accelerometer()
                {
                    Timestamp = timestamp,
                    X = Convert.ToSingle(data.UserAcceleration.X),
                    Y = Convert.ToSingle(data.UserAcceleration.Y),
                    Z = Convert.ToSingle(data.UserAcceleration.Z)
                });


                //calibrated magnetic field
                _magnetoometerRepository.Add(new Magnetometer()
                {
                    Timestamp = timestamp,
                    X = Convert.ToSingle(data.MagneticField.Field.X),
                    Y = Convert.ToSingle(data.MagneticField.Field.Y),
                    Z = Convert.ToSingle(data.MagneticField.Field.Z)
                });

                _linearAccelerationRepository.Add(new LinearAcceleration()
                {
                    Timestamp = timestamp,
                    X = Convert.ToSingle(data.UserAcceleration.X),
                    Y = Convert.ToSingle(data.UserAcceleration.Y),
                    Z = Convert.ToSingle(data.UserAcceleration.Z)
                });

                _rotationVectorRepository.Add(new RotationVector()
                {
                    Timestamp = timestamp,
                    X = Convert.ToSingle(data.Attitude.Roll),
                    Y = Convert.ToSingle(data.Attitude.Pitch),
                    Z = Convert.ToSingle(data.Attitude.Yaw)
                });

                _quaternionRepository.Add(new Quaternion()
                {
                    Timestamp = timestamp,
                    X = Convert.ToSingle(data.Attitude.Quaternion.x),
                    Y = Convert.ToSingle(data.Attitude.Quaternion.y),
                    Z = Convert.ToSingle(data.Attitude.Quaternion.z),
                    W = Convert.ToSingle(data.Attitude.Quaternion.w)
                });

                _gravityRepository.Add(new Gravity()
                {
                    Timestamp = timestamp,
                    X = Convert.ToSingle(data.Gravity.X),
                    Y = Convert.ToSingle(data.Gravity.Y),
                    Z = Convert.ToSingle(data.Gravity.Z)
                });


                var heading = data.Heading; // compass

            });

            _cLLocationManager.

            // service for measurements once per minute
            UIDevice.CurrentDevice.BatteryMonitoringEnabled = true;

            Timer timer = new Timer(60000);
            timer.AutoReset = true;
            timer.Elapsed += TimerElapsed;
            timer.Start();
            

            IsListening = true;
        }


        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            SensorMeasurementSession sensorMeasurementSession = new SensorMeasurementSession
            {
                Timestamp = DateTimeOffset.Now
            };

            //pedometer
            _cMPedometer.StartPedometerUpdates(NSDate.Now, (data, error) =>
            {
                sensorMeasurementSession.Pedometer = data.NumberOfSteps.Int32Value;
            });

            //compass
            sensorMeasurementSession.Compass = Convert.ToInt32(_cLLocationManager.Heading.MagneticHeading);

            // proximity
            sensorMeasurementSession.Proximity = UIDevice.CurrentDevice.ProximityState;

            //ambient light
            sensorMeasurementSession.AmbientLight = Convert.ToInt32(UIScreen.MainScreen.Brightness); // works only with dynamic brightness?

            //battery
            sensorMeasurementSession.BatteryLevel = Convert.ToInt32(UIDevice.CurrentDevice.BatteryLevel * 100F);
            
            //sensorMeasurementSession.BatteryStatus = 
            switch (UIDevice.CurrentDevice.BatteryState)    
            {
                case UIDeviceBatteryState.Unknown:
                    sensorMeasurementSession.BatteryStatus = BatteryStatus.Unknown;
                    break;
                case UIDeviceBatteryState.Unplugged:
                    sensorMeasurementSession.BatteryStatus = BatteryStatus.NotCharging;
                    break;
                case UIDeviceBatteryState.Charging:
                    sensorMeasurementSession.BatteryStatus = BatteryStatus.Charging;
                    break;
                case UIDeviceBatteryState.Full:
                    sensorMeasurementSession.BatteryStatus = BatteryStatus.Full;
                    break;
                default:
                    break;
            }

            _sensorMeasurementSessionRepository.Add(sensorMeasurementSession);
        }

        public void StopSensorUpdates()
        {
            IsListening = false;
        }
    }
}
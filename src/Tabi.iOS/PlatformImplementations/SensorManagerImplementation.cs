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

namespace Tabi.iOS.PlatformImplementations
{
    public class SensorManagerImplementation : ISensorManager
    {
        private readonly IRepoManager _repoManager;

        private readonly ISensorMeasurementSessionRepository _sensorMeasurementSessionRepository;
        private readonly ISensorRepository<Accelerometer> _accelerometerRepository;
        private readonly ISensorRepository<Gyroscope> _gyroscoperRepository;
        private readonly ISensorRepository<Magnetometer> _magnetometerRepository;
        private readonly ISensorRepository<LinearAcceleration> _linearAccelerationRepository;
        private readonly ISensorRepository<Orientation> _orientationRepository;
        private readonly ISensorRepository<Quaternion> _quaternionRepository;
        private readonly ISensorRepository<Gravity> _gravityRepository;

        private readonly CMMotionManager _motionManager;
        private readonly CMPedometer _pedometer;
        private NSOperationQueue _queue;
        private readonly CLLocationManager _locationManager;

        private int _pedometerInt;

        public bool IsListening { get; private set; }

        public SensorManagerImplementation(IRepoManager repoManager, CMMotionManager motionManager, CLLocationManager locationManager, CMPedometer pedometer)
        {
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
            _motionManager = motionManager ?? throw new ArgumentNullException(nameof(motionManager));
            _locationManager = locationManager ?? throw new ArgumentNullException(nameof(locationManager));
            _pedometer = pedometer ?? throw new ArgumentNullException(nameof(pedometer));

            _queue = new NSOperationQueue();

            _sensorMeasurementSessionRepository = _repoManager.SensorMeasurementSessionRepository;
            _accelerometerRepository = _repoManager.AccelerometerRepository;
            _gyroscoperRepository = _repoManager.GyroscopeRepository;
            _magnetometerRepository = _repoManager.MagnetometerRepository;
            _linearAccelerationRepository = _repoManager.LinearAccelerationRepository;
            _orientationRepository = _repoManager.OrientationRepository;
            _quaternionRepository = _repoManager.QuaternionRepository;
            _gravityRepository = _repoManager.GravityRepository;

            locationManager.DesiredAccuracy = CLLocation.AccuracyBest;
            locationManager.HeadingFilter = 1;
            locationManager.AllowsBackgroundLocationUpdates = true;

            motionManager.DeviceMotionUpdateInterval = 0.1;
            motionManager.AccelerometerUpdateInterval = 0.1;
            motionManager.GyroUpdateInterval = 0.1;
            motionManager.MagnetometerUpdateInterval = 0.1;
        }

        void HandleCMAccelerometerHandler(CMAccelerometerData data, NSError error)
        {
            _accelerometerRepository.Add(new Accelerometer()
            {
                Timestamp = DateTimeOffset.Now,
                X = Convert.ToSingle(data.Acceleration.X),
                Y = Convert.ToSingle(data.Acceleration.Y),
                Z = Convert.ToSingle(data.Acceleration.Z)
            });
        }

        void HandleCMGyroHandler(CMGyroData gyroData, NSError error)
        {
            _gyroscoperRepository.Add(new Gyroscope()
            {
                Timestamp = DateTimeOffset.Now,
                X = Convert.ToSingle(gyroData.RotationRate.x),
                Y = Convert.ToSingle(gyroData.RotationRate.y),
                Z = Convert.ToSingle(gyroData.RotationRate.z)
            });
        }

        void HandleCMMagnetometerHandler(CMMagnetometerData data, NSError error)
        {
            _magnetometerRepository.Add(new Magnetometer()
            {
                Timestamp = DateTimeOffset.Now,
                X = Convert.ToSingle(data.MagneticField.X),
                Y = Convert.ToSingle(data.MagneticField.Y),
                Z = Convert.ToSingle(data.MagneticField.Z)
            });
        }

        void HandleCMDeviceMotionHandler(CMDeviceMotion data, NSError error)
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
            //Console.WriteLine("inserted linear acceleratin data");

            //calibrated magnetic field
            _magnetometerRepository.Add(new Magnetometer()
            {
                Timestamp = timestamp,
                X = Convert.ToSingle(data.MagneticField.Field.X),
                Y = Convert.ToSingle(data.MagneticField.Field.Y),
                Z = Convert.ToSingle(data.MagneticField.Field.Z)
            });
            //Console.WriteLine("inserted callibrated magnetometerdata");

            _orientationRepository.Add(new Orientation()
            {
                Timestamp = timestamp,
                X = Convert.ToSingle(data.Attitude.Roll),
                Y = Convert.ToSingle(data.Attitude.Pitch),
                Z = Convert.ToSingle(data.Attitude.Yaw)
            });
            //Console.WriteLine("inserted attitude");

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
        }

        void HandleCMPedometer(CMPedometerData data, NSError error)
        {
            _pedometerInt = data.NumberOfSteps.Int32Value;
        }


        public void StartSensorUpdates()
        {
            // Raw accelerometer
            if (_motionManager.AccelerometerAvailable)
            {
                _motionManager.StartAccelerometerUpdates(_queue, HandleCMAccelerometerHandler);
            }

            if (_motionManager.GyroAvailable)
            {
                _motionManager.StartGyroUpdates(_queue, HandleCMGyroHandler);
            }

            if (_motionManager.MagnetometerAvailable)
            {
                _motionManager.StartMagnetometerUpdates(_queue, HandleCMMagnetometerHandler);
            }

            if (_motionManager.DeviceMotionAvailable)
            {
                _motionManager.StartDeviceMotionUpdates(_queue, HandleCMDeviceMotionHandler);
                _pedometer.StartPedometerUpdates(NSDate.Now, HandleCMPedometer);
            }

            //start updating headings
            _locationManager.StartUpdatingHeading();

            //// service for measurements once per minute
            UIDevice.CurrentDevice.BatteryMonitoringEnabled = true;

            Timer timer = new Timer(60000);
            timer.AutoReset = true;
            timer.Elapsed += SensorMeasurementSessionTimerElapsed;
            timer.Start();


            IsListening = true;
        }


        private void SensorMeasurementSessionTimerElapsed(object sender, ElapsedEventArgs e)
        {
            SensorMeasurementSession sensorMeasurementSession = new SensorMeasurementSession
            {
                Timestamp = DateTimeOffset.Now
            };

            //pedometer
            sensorMeasurementSession.Pedometer = _pedometerInt;

            //compass
            sensorMeasurementSession.Compass = Convert.ToInt32(_locationManager.Heading.MagneticHeading);

            // proximity
            sensorMeasurementSession.Proximity = UIDevice.CurrentDevice.ProximityState;

            //ambient light
            //sensorMeasurementSession.AmbientLight = Convert.ToInt32(UIScreen.MainScreen.Brightness); // works only with dynamic brightness?
            //Console.WriteLine(UIScreen.MainScreen.Brightness);


            //battery
            sensorMeasurementSession.BatteryLevel = Convert.ToInt32(UIDevice.CurrentDevice.BatteryLevel * 100F);

            //sensorMeasurementSession.BatteryStatus = 
            switch (UIDevice.CurrentDevice.BatteryState)
            {
                case UIDeviceBatteryState.Unknown:
                    sensorMeasurementSession.BatteryStatus = BatteryEntryState.Unknown;
                    break;
                case UIDeviceBatteryState.Unplugged:
                    sensorMeasurementSession.BatteryStatus = BatteryEntryState.NotCharging;
                    break;
                case UIDeviceBatteryState.Charging:
                    sensorMeasurementSession.BatteryStatus = BatteryEntryState.Charging;
                    break;
                case UIDeviceBatteryState.Full:
                    sensorMeasurementSession.BatteryStatus = BatteryEntryState.Full;
                    break;
                default:
                    break;
            }

            _sensorMeasurementSessionRepository.Add(sensorMeasurementSession);
            Console.WriteLine("added sensormeasurementsession");
        }

        public void StopSensorUpdates()
        {
            if (_motionManager.AccelerometerAvailable)
            {
                _motionManager.StopAccelerometerUpdates();
            }

            if (_motionManager.GyroAvailable)
            {
                _motionManager.StopGyroUpdates();
            }

            if (_motionManager.MagnetometerAvailable)
            {
                _motionManager.StopMagnetometerUpdates();
            }

            if (_motionManager.DeviceMotionAvailable)
            {
                _motionManager.StopDeviceMotionUpdates();
                _pedometer.StopPedometerUpdates();
            }

            IsListening = false;
        }
    }
}
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
        private readonly IAccelerometerRepository _accelerometerRepository;
        private readonly IGyroscopeRepository _gyroscoperRepository;
        private readonly IMagnetometerRepository _magnetoometerRepository;
        private readonly ISensorMeasurementSessionRepository _sensorMeasurementSessionRepository;
        private readonly CMMotionManager _cMMotionManager;
        private readonly CMPedometer _cMPedometer;
        private readonly CLLocationManager _cLLocationManager;

        public bool IsListening { get; private set; }

        public SensorManagerImplementation()
        {
            _accelerometerRepository = App.RepoManager.AccelerometerRepository;
            _gyroscoperRepository = App.RepoManager.GyroscopeRepository;
            _magnetoometerRepository = App.RepoManager.MagnetometerRepository;
            _sensorMeasurementSessionRepository = App.RepoManager.SensorMeasurementSessionRepository;
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
        }

        public void StartSensorUpdates()
        {
            //get motion sensors

            //_cMMotionManager.StartAccelerometerUpdates(NSOperationQueue.CurrentQueue, (data, error) =>
            //{
            //    _accelerometerRepository.Add(new DataObjects.Accelerometer()
            //    {
            //        Timestamp = DateTimeOffset.Now,
            //        Xvalue = Convert.ToSingle(data.Acceleration.X),
            //        Yvalue = Convert.ToSingle(data.Acceleration.Y),
            //        Zvalue = Convert.ToSingle(data.Acceleration.Z)
            //    });
            //});

            //_cMMotionManager.StartGyroUpdates(NSOperationQueue.CurrentQueue, (data, error) =>
            //{

            //    _gyroscoperRepository.Add(new DataObjects.Gyroscope()
            //    {
            //        Timestamp = DateTimeOffset.Now,
            //        Xvalue = Convert.ToSingle(data.RotationRate.x),
            //        Yvalue = Convert.ToSingle(data.RotationRate.y),
            //        Zvalue = Convert.ToSingle(data.RotationRate.z)
            //    });
            //});

            //_cMMotionManager.StartMagnetometerUpdates(NSOperationQueue.CurrentQueue, (data, error) =>
            //{
            //    _magnetoometerRepository.Add(new DataObjects.Magnetometer()
            //    {
            //        Timestamp = DateTimeOffset.Now,
            //        Xvalue = Convert.ToSingle(data.MagneticField.X),
            //        Yvalue = Convert.ToSingle(data.MagneticField.Y),
            //        Zvalue = Convert.ToSingle(data.MagneticField.Z)
            //    });
            //});

            _cMMotionManager.StartDeviceMotionUpdates(NSOperationQueue.CurrentQueue, (data, error) =>
            {
                DateTimeOffset timestamp = DateTimeOffset.Now;
                _accelerometerRepository.Add(new Accelerometer()
                {
                    Timestamp = timestamp,
                    X = Convert.ToSingle(data.UserAcceleration.X),
                    Y = Convert.ToSingle(data.UserAcceleration.Y),
                    Z = Convert.ToSingle(data.UserAcceleration.Z)
                });

                _gyroscoperRepository.Add(new Gyroscope()
                {
                    Timestamp = DateTimeOffset.Now,
                    X = Convert.ToSingle(data.RotationRate.x),
                    Y = Convert.ToSingle(data.RotationRate.y),
                    Z = Convert.ToSingle(data.RotationRate.z)
                });

                _magnetoometerRepository.Add(new Magnetometer()
                {
                    Timestamp = DateTimeOffset.Now,
                    X = Convert.ToSingle(data.MagneticField.Field.X),
                    Y = Convert.ToSingle(data.MagneticField.Field.Y),
                    Z = Convert.ToSingle(data.MagneticField.Field.Z)
                });

                //var attitude = data.Attitude; //pitch, roll, yaw
                //var heading = data.Heading;
                //var linearAcceleration = data.UserAcceleration;
                //var quaternion = data.RotationRate;
                //var gravity = data.Gravity;
            });

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
            SensorMeasurementSession sensorMeasurementSession = new SensorMeasurementSession();
            sensorMeasurementSession.Timestamp = DateTimeOffset.Now;

            //pedometer
            _cMPedometer.StartPedometerUpdates(NSDate.Now, (data, error) =>
            {
                sensorMeasurementSession.Pedometer = data.NumberOfSteps.Int32Value;
            });

            //compass
            sensorMeasurementSession.Compass = Convert.ToInt32(_cLLocationManager.Heading.TrueHeading * Math.PI / 180D);

            // proximity
            sensorMeasurementSession.Proximity = UIDevice.CurrentDevice.ProximityState;

            //ambient light
            sensorMeasurementSession.AmbientLight = Convert.ToInt32(UIScreen.MainScreen.Brightness);

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
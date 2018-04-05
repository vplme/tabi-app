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
        private readonly ISensorRepository<Accelerometer> _accelerometerRepository;
        private readonly ISensorRepository<Gyroscope> _gyroscoperRepository;
        private readonly ISensorRepository<Magnetometer> _magnetometerRepository;
        private readonly ISensorRepository<LinearAcceleration> _linearAccelerationRepository;
        private readonly ISensorRepository<Orientation> _orientationRepository;
        private readonly ISensorRepository<Quaternion> _quaternionRepository;
        private readonly ISensorRepository<Gravity> _gravityRepository;

        private readonly CMMotionManager _cMMotionManager;
        private readonly CMPedometer _cMPedometer;
        private readonly CLLocationManager _cLLocationManager;

        private int _pedometer;

        public bool IsListening { get; private set; }

        public SensorManagerImplementation()
        {
            //_sensorMeasurementSessionRepository = App.RepoManager.SensorMeasurementSessionRepository;
            //_accelerometerRepository = App.RepoManager.AccelerometerRepository;
            //_gyroscoperRepository = App.RepoManager.GyroscopeRepository;
            //_magnetometerRepository = App.RepoManager.MagnetometerRepository;
            //_linearAccelerationRepository = App.RepoManager.LinearAccelerationRepository;
            //_orientationRepository = App.RepoManager.OrientationRepository;
            //_quaternionRepository = App.RepoManager.QuaternionRepository;
            //_gravityRepository = App.RepoManager.GravityRepository;


            //_cMMotionManager = new CMMotionManager()
            //{
            //    DeviceMotionUpdateInterval = 0.1,
            //    AccelerometerUpdateInterval = 0.1,
            //    GyroUpdateInterval = 0.1,
            //    MagnetometerUpdateInterval = 0.1
            //};
            
            //_cMPedometer = new CMPedometer();
           

            //_cLLocationManager = new CLLocationManager()
            //{
            //    DesiredAccuracy = CLLocation.AccuracyBest,
            //    HeadingFilter = 1,
            //    AllowsBackgroundLocationUpdates = true
            //};
        }


        public void StartSensorUpdates()
        {
            //get motion sensors
            //Raw accelerometer
        //    _cMMotionManager.StartAccelerometerUpdates(NSOperationQueue.CurrentQueue, (data, error) =>
        //    {
        //        _accelerometerRepository.Add(new Accelerometer()
        //        {
        //            Timestamp = DateTimeOffset.Now,
        //            X = Convert.ToSingle(data.Acceleration.X),
        //            Y = Convert.ToSingle(data.Acceleration.Y),
        //            Z = Convert.ToSingle(data.Acceleration.Z)
        //        });
        //        //Console.WriteLine("inserted accelerometerdata");
        //    });

        //    ////gyroscope
        //    _cMMotionManager.StartGyroUpdates(NSOperationQueue.CurrentQueue, (data, error) =>
        //    {

        //        _gyroscoperRepository.Add(new Gyroscope()
        //        {
        //            Timestamp = DateTimeOffset.Now,
        //            X = Convert.ToSingle(data.RotationRate.x),
        //            Y = Convert.ToSingle(data.RotationRate.y),
        //            Z = Convert.ToSingle(data.RotationRate.z)
        //        });
        //        //Console.WriteLine("inserted gyroscopedata");
        //    });

        //    ////Raw magnetometer
        //    _cMMotionManager.StartMagnetometerUpdates(NSOperationQueue.CurrentQueue, (data, error) =>
        //    {
        //        _magnetometerRepository.Add(new Magnetometer()
        //        {
        //            Timestamp = DateTimeOffset.Now,
        //            X = Convert.ToSingle(data.MagneticField.X),
        //            Y = Convert.ToSingle(data.MagneticField.Y),
        //            Z = Convert.ToSingle(data.MagneticField.Z)
        //        });
        //        //Console.WriteLine("inserted raw magnetometerdata");
        //    });


        //    _cMMotionManager.StartDeviceMotionUpdates(NSOperationQueue.CurrentQueue, (data, error) =>
        //    {
        //        //Linear acceleration (acceleration - gravity)
        //        DateTimeOffset timestamp = DateTimeOffset.Now;

        //        _accelerometerRepository.Add(new Accelerometer()
        //        {
        //            Timestamp = timestamp,
        //            X = Convert.ToSingle(data.UserAcceleration.X),
        //            Y = Convert.ToSingle(data.UserAcceleration.Y),
        //            Z = Convert.ToSingle(data.UserAcceleration.Z)
        //        });
        //        //Console.WriteLine("inserted linear acceleratin data");

        //        //calibrated magnetic field
        //        _magnetometerRepository.Add(new Magnetometer()
        //        {
        //            Timestamp = timestamp,
        //            X = Convert.ToSingle(data.MagneticField.Field.X),
        //            Y = Convert.ToSingle(data.MagneticField.Field.Y),
        //            Z = Convert.ToSingle(data.MagneticField.Field.Z)
        //        });
        //        //Console.WriteLine("inserted callibrated magnetometerdata");

        //        _orientationRepository.Add(new Orientation()
        //        {
        //            Timestamp = timestamp,
        //            X = Convert.ToSingle(data.Attitude.Roll),
        //            Y = Convert.ToSingle(data.Attitude.Pitch),
        //            Z = Convert.ToSingle(data.Attitude.Yaw)
        //        });
        //        //Console.WriteLine("inserted attitude");

        //        _quaternionRepository.Add(new Quaternion()
        //        {
        //            Timestamp = timestamp,
        //            X = Convert.ToSingle(data.Attitude.Quaternion.x),
        //            Y = Convert.ToSingle(data.Attitude.Quaternion.y),
        //            Z = Convert.ToSingle(data.Attitude.Quaternion.z),
        //            W = Convert.ToSingle(data.Attitude.Quaternion.w)
        //        });
        //        //Console.WriteLine("inserted quaternion");

        //        _gravityRepository.Add(new Gravity()
        //        {
        //            Timestamp = timestamp,
        //            X = Convert.ToSingle(data.Gravity.X),
        //            Y = Convert.ToSingle(data.Gravity.Y),
        //            Z = Convert.ToSingle(data.Gravity.Z)
        //        });
        //        //Console.WriteLine("inserted gravity");

        //    });

        //    _cMPedometer.StartPedometerUpdates(NSDate.Now, (data, error) =>
        //    {
        //        _pedometer = data.NumberOfSteps.Int32Value;
        //        Console.WriteLine("assigned a pedometer");
        //    });

        //    //start updating headings
        //    _cLLocationManager.StartUpdatingHeading();

        //    //// service for measurements once per minute
        //    UIDevice.CurrentDevice.BatteryMonitoringEnabled = true;

        //    Timer timer = new Timer(60000);
        //    timer.AutoReset = true;
        //    timer.Elapsed += SensorMeasurementSessionTimerElapsed;
        //    timer.Start();


        //    IsListening = true;
        //}


        //private void SensorMeasurementSessionTimerElapsed(object sender, ElapsedEventArgs e)
        //{
        //    SensorMeasurementSession sensorMeasurementSession = new SensorMeasurementSession
        //    {
        //        Timestamp = DateTimeOffset.Now
        //    };

        //    //pedometer
        //    sensorMeasurementSession.Pedometer = _pedometer;

        //    //compass
        //    sensorMeasurementSession.Compass = Convert.ToInt32(_cLLocationManager.Heading.MagneticHeading);

        //    // proximity
        //    sensorMeasurementSession.Proximity = UIDevice.CurrentDevice.ProximityState;

        //    //ambient light
        //    //sensorMeasurementSession.AmbientLight = Convert.ToInt32(UIScreen.MainScreen.Brightness); // works only with dynamic brightness?
        //    //Console.WriteLine(UIScreen.MainScreen.Brightness);


        //    //battery
        //    sensorMeasurementSession.BatteryLevel = Convert.ToInt32(UIDevice.CurrentDevice.BatteryLevel * 100F);

        //    //sensorMeasurementSession.BatteryStatus = 
        //    switch (UIDevice.CurrentDevice.BatteryState)
        //    {
        //        case UIDeviceBatteryState.Unknown:
        //            sensorMeasurementSession.BatteryStatus = BatteryEntryState.Unknown;
        //            break;
        //        case UIDeviceBatteryState.Unplugged:
        //            sensorMeasurementSession.BatteryStatus = BatteryEntryState.NotCharging;
        //            break;
        //        case UIDeviceBatteryState.Charging:
        //            sensorMeasurementSession.BatteryStatus = BatteryEntryState.Charging;
        //            break;
        //        case UIDeviceBatteryState.Full:
        //            sensorMeasurementSession.BatteryStatus = BatteryEntryState.Full;
        //            break;
        //        default:
        //            break;
        //    }

        //    _sensorMeasurementSessionRepository.Add(sensorMeasurementSession);
        //    Console.WriteLine("added sensormeasurementsession");
        }

        public void StopSensorUpdates()
        {
            IsListening = false;
        }
    }
}
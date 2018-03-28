using Tabi.DataObjects;

namespace Tabi.DataStorage
{
    public interface IRepoManager
    {
        IUserRepository UserRepository { get; }
        IDeviceRepository DeviceRepository { get; }
        IPositionEntryRepository PositionEntryRepository { get; }
        IMotionEntryRepository MotionEntryRepository { get; }
        IStopRepository StopRepository { get; }
        IStopVisitRepository StopVisitRepository { get; }
        IBatteryEntryRepository BatteryEntryRepository { get; }
        ITrackEntryRepository TrackEntryRepository { get; }
        ILogEntryRepository LogEntryRepository { get; }
        ITransportationModeRepository TransportationModeRepository {get;}

        // session sensors
        ISensorMeasurementSessionRepository SensorMeasurementSessionRepository { get; }
        // motion sensors
        ISensorRepository<Accelerometer> AccelerometerRepository { get; }
        ISensorRepository<Gyroscope> GyroscopeRepository { get; }
        ISensorRepository<Magnetometer> MagnetometerRepository { get; }
        ISensorRepository<LinearAcceleration> LinearAccelerationRepository { get; }
        ISensorRepository<Orientation> OrientationRepository { get; }
        ISensorRepository<Quaternion> QuaternionRepository { get; }
        ISensorRepository<Gravity> GravityRepository { get; }


        void SaveChanges();
    }
}

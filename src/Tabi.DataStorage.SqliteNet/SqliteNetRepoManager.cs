using System;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetRepoManager : IRepoManager
    {
        private static SQLiteConnection _connection;

        public SqliteNetRepoManager(SQLiteConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));

            _connection.CreateTable<User>();
            _connection.CreateTable<Device>();
            _connection.CreateTable<PositionEntry>();
            _connection.CreateTable<Stop>();
            _connection.CreateTable<StopVisit>();
            _connection.CreateTable<BatteryEntry>();
            _connection.CreateTable<TrackEntry>();
            _connection.CreateTable<TransportationModeEntry>();
            _connection.CreateTable<TransportationModeTracks>();
            _connection.CreateTable<LogEntry>();

            _connection.CreateTable<SensorMeasurementSession>();
            _connection.CreateTable<Accelerometer>();
            _connection.CreateTable<Gyroscope>();
            _connection.CreateTable<Magnetometer>();
            _connection.CreateTable<LinearAcceleration>();
            _connection.CreateTable<Orientation>();
            _connection.CreateTable<Quaternion>();
            _connection.CreateTable<Gravity>();



            PositionEntryRepository = new SqliteNetPositionEntryRepository(_connection);
            DeviceRepository = new SqliteNetDeviceRepository(_connection);
            StopRepository = new SqliteNetStopRepository(_connection);
            StopVisitRepository = new SqliteNetStopVisitRepository(_connection);
            BatteryEntryRepository = new SqliteNetBatteryEntryRepository(_connection);
            TrackEntryRepository = new SqliteNetTrackEntryRepository(_connection);
            LogEntryRepository = new SqliteNetLogEntryRepository(_connection);
            TransportationModeRepository = new SqliteNetTransportationModeRepository(_connection);

            //sensor
            SensorMeasurementSessionRepository = new SqliteNetSensorMeasurementSessionRepository(_connection);
            AccelerometerRepository = new SqliteNetSensorRepository<Accelerometer>(_connection); 
            GyroscopeRepository = new SqliteNetSensorRepository<Gyroscope>(_connection);
            MagnetometerRepository = new SqliteNetSensorRepository<Magnetometer>(_connection);
            LinearAccelerationRepository = new SqliteNetSensorRepository<LinearAcceleration>(_connection);
            OrientationRepository = new SqliteNetSensorRepository<Orientation>(_connection);
            QuaternionRepository = new SqliteNetSensorRepository<Quaternion>(_connection);
            GravityRepository = new SqliteNetSensorRepository<Gravity>(_connection);
        }

        public IUserRepository UserRepository { get; private set; }
        public IDeviceRepository DeviceRepository { get; private set; }
        public IPositionEntryRepository PositionEntryRepository { get; private set; }
        public IMotionEntryRepository MotionEntryRepository { get; private set; }
        public IStopRepository StopRepository { get; private set; }
        public IStopVisitRepository StopVisitRepository { get; private set; }
        public IBatteryEntryRepository BatteryEntryRepository { get; private set; }
        public ITrackEntryRepository TrackEntryRepository { get; }
        public ILogEntryRepository LogEntryRepository { get; }
        public ITransportationModeRepository TransportationModeRepository { get; set; }

        //sensor
        public ISensorMeasurementSessionRepository SensorMeasurementSessionRepository { get; }
        // motion sensor
        public ISensorRepository<Accelerometer> AccelerometerRepository { get; }
        public ISensorRepository<Gyroscope> GyroscopeRepository { get; }
        public ISensorRepository<Magnetometer> MagnetometerRepository { get; }
        public ISensorRepository<LinearAcceleration> LinearAccelerationRepository { get; }
        public ISensorRepository<Orientation> OrientationRepository { get; }
        public ISensorRepository<Quaternion> QuaternionRepository { get; }
        public ISensorRepository<Gravity> GravityRepository { get; }


        public void SaveChanges()
        {
            throw new NotImplementedException();
        }
    }
}
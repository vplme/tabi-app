using System;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetRepoManager : IRepoManager
    {
        private static SQLiteConnection conn;

        public SqliteNetRepoManager(string path)
        {
            if (conn == null)
            {
                conn = new SQLiteConnection(
                    path,
                    SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex,
                    false);
            }

            conn.CreateTable<User>();
            conn.CreateTable<Device>();
            conn.CreateTable<PositionEntry>();
            conn.CreateTable<Stop>();
            conn.CreateTable<StopVisit>();
            conn.CreateTable<BatteryEntry>();
            conn.CreateTable<TrackEntry>();
            conn.CreateTable<TransportationMode>();
            conn.CreateTable<TransportationModeTracks>();
            conn.CreateTable<LogEntry>();

            conn.CreateTable<SensorMeasurementSession>();
            conn.CreateTable<Accelerometer>();
            conn.CreateTable<Gyroscope>();
            conn.CreateTable<Magnetometer>();
            conn.CreateTable<LinearAcceleration>();
            conn.CreateTable<Orientation>();
            conn.CreateTable<Quaternion>();
            conn.CreateTable<Gravity>();



            PositionEntryRepository = new SqliteNetPositionEntryRepository(conn);
            DeviceRepository = new SqliteNetDeviceRepository(conn);
            StopRepository = new SqliteNetStopRepository(conn);
            StopVisitRepository = new SqliteNetStopVisitRepository(conn);
            BatteryEntryRepository = new SqliteNetBatteryEntryRepository(conn);
            TrackEntryRepository = new SqliteNetTrackEntryRepository(conn);
            LogEntryRepository = new SqliteNetLogEntryRepository(conn);
            TransportationModeRepository = new SqliteNetTransportationModeRepository(conn);

            //sensor
            SensorMeasurementSessionRepository = new SqliteNetSensorMeasurementSessionRepository(conn);
            AccelerometerRepository = new SqliteNetSensorRepository<Accelerometer>(conn); 
            GyroscopeRepository = new SqliteNetSensorRepository<Gyroscope>(conn);
            MagnetometerRepository = new SqliteNetSensorRepository<Magnetometer>(conn);
            LinearAccelerationRepository = new SqliteNetSensorRepository<LinearAcceleration>(conn);
            OrientationRepository = new SqliteNetSensorRepository<Orientation>(conn);
            QuaternionRepository = new SqliteNetSensorRepository<Quaternion>(conn);
            GravityRepository = new SqliteNetSensorRepository<Gravity>(conn);
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
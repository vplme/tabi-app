using System;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetRepoManager : IRepoManager, IDisposable
    {
        private static SQLiteConnection _connection;

        public SqliteNetRepoManager(SQLiteConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));

            CreateTables();
            InitializeRepositories();
        }

        public IUserRepository UserRepository { get; private set; }
        public IDeviceRepository DeviceRepository { get; private set; }
        public IPositionEntryRepository PositionEntryRepository { get; private set; }
        public IMotionEntryRepository MotionEntryRepository { get; private set; }
        public IStopRepository StopRepository { get; private set; }
        public IStopVisitRepository StopVisitRepository { get; private set; }
        public IBatteryEntryRepository BatteryEntryRepository { get; private set; }
        public ITrackEntryRepository TrackEntryRepository { get; private set; }
        public ILogEntryRepository LogEntryRepository { get; private set; }
        public ITransportationModeRepository TransportationModeRepository { get; private set; }
        public IMotiveRepository MotiveRepository { get; private set; }

        //sensor
        public ISensorMeasurementSessionRepository SensorMeasurementSessionRepository { get; private set; }
        // motion sensor
        public ISensorRepository<Accelerometer> AccelerometerRepository { get; private set; }
        public ISensorRepository<Gyroscope> GyroscopeRepository { get; private set; }
        public ISensorRepository<Magnetometer> MagnetometerRepository { get; private set; }
        public ISensorRepository<LinearAcceleration> LinearAccelerationRepository { get; private set; }
        public ISensorRepository<Orientation> OrientationRepository { get; private set; }
        public ISensorRepository<Quaternion> QuaternionRepository { get; private set; }
        public ISensorRepository<Gravity> GravityRepository { get; private set; }


        /// <summary>
        /// Initializes all the repositories available in the RepoManager with the single SqliteConnection.
        /// </summary>
        private void InitializeRepositories()
        {
            PositionEntryRepository = new SqliteNetPositionEntryRepository(_connection);
            DeviceRepository = new SqliteNetDeviceRepository(_connection);
            StopRepository = new SqliteNetStopRepository(_connection);
            StopVisitRepository = new SqliteNetStopVisitRepository(_connection);
            BatteryEntryRepository = new SqliteNetBatteryEntryRepository(_connection);
            TrackEntryRepository = new SqliteNetTrackEntryRepository(_connection);
            LogEntryRepository = new SqliteNetLogEntryRepository(_connection);
            TransportationModeRepository = new SqliteNetTransportationModeRepository(_connection);
            MotiveRepository = new SqliteNetMotiveRepository(_connection);

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
        /// <summary>
        /// Drops all the tables handled by the RepoManager.
        /// </summary>
        public void ClearDatabase()
        {
            _connection.DropTable<User>();
            _connection.DropTable<Device>();
            _connection.DropTable<PositionEntry>();
            _connection.DropTable<Stop>();
            _connection.DropTable<StopVisit>();
            _connection.DropTable<BatteryEntry>();
            _connection.DropTable<TrackEntry>();
            _connection.DropTable<TransportationModeEntry>();
            _connection.DropTable<TransportationModeTracks>();
            _connection.DropTable<LogEntry>();
            _connection.DropTable<Motive>();

            _connection.DropTable<SensorMeasurementSession>();
            _connection.DropTable<Accelerometer>();
            _connection.DropTable<Gyroscope>();
            _connection.DropTable<Magnetometer>();
            _connection.DropTable<LinearAcceleration>();
            _connection.DropTable<Orientation>();
            _connection.DropTable<Quaternion>();
            _connection.DropTable<Gravity>();
        }

        /// <summary>
        /// Creates all the tables handled by the RepoManager.
        /// </summary>
        private void CreateTables()
        {
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
            _connection.CreateTable<Motive>();

            _connection.CreateTable<SensorMeasurementSession>();
            _connection.CreateTable<Accelerometer>();
            _connection.CreateTable<Gyroscope>();
            _connection.CreateTable<Magnetometer>();
            _connection.CreateTable<LinearAcceleration>();
            _connection.CreateTable<Orientation>();
            _connection.CreateTable<Quaternion>();
            _connection.CreateTable<Gravity>();
        }



        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _connection.Close();
                    _connection.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                disposedValue = true;
            }
        }

        // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SqliteNetRepoManager() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
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

            PositionEntryRepository = new SqliteNetPositionEntryRepository(conn);
            DeviceRepository = new SqliteNetDeviceRepository(conn);
            StopRepository = new SqliteNetStopRepository(conn);
            StopVisitRepository = new SqliteNetStopVisitRepository(conn);
        }

        public IUserRepository UserRepository { get; private set; }
        public IDeviceRepository DeviceRepository { get; private set; }
        public IPositionEntryRepository PositionEntryRepository { get; private set; }
        public IMotionEntryRepository MotionEntryRepository { get; private set; }
        public IStopRepository StopRepository { get; private set; }
        public IStopVisitRepository StopVisitRepository { get; private set; }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }
    }
}

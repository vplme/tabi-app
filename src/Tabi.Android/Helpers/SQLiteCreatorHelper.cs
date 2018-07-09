using System;
using PCLStorage;
using SQLite;

namespace Tabi.Droid.Helpers
{
    public static class SQLiteCreatorHelper
    {
        private static SQLiteConnection _connection;

        public static SQLiteConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = CreateConnection();
                }
                return _connection;
            }
        }

        private static SQLiteConnection CreateConnection()
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            var dbPath = PortablePath.Combine(rootFolder.Path, "tabi.db");
            SQLiteConnection connection = new SQLiteConnection(
                dbPath,
                    SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex,
                    false);

            return connection;
        }
    }
}

using System;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetBatteryEntryRepository : SqliteNetRepository<BatteryEntry>, IBatteryEntryRepository
    {
        public SqliteNetBatteryEntryRepository(SQLiteConnection conn) : base(conn)
        {
        }
    }
}

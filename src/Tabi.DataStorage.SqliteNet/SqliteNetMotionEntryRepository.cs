using System;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetMotionEntryRepository : SqliteNetRepository<MotionEntry>, IMotionEntryRepository
    {
        public SqliteNetMotionEntryRepository(SQLiteConnection conn) : base(conn)
        {
        }
    }
}

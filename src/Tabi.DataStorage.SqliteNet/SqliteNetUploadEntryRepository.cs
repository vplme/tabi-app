using System;
using System.Linq;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetUploadEntryRepository : SqliteNetRepository<UploadEntry>, IUploadEntryRepository
    {
        public SqliteNetUploadEntryRepository(SQLiteConnection conn) : base(conn)
        {
        }

        public UploadEntry GetLastUploadEntry(UploadType type)
        {
            UploadEntry result = connection.Table<UploadEntry>().Where(e => e.Type == type).OrderBy(e => e.Timestamp).LastOrDefault();

            return result ?? new UploadEntry();
        }
    }
}

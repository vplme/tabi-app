using System.Linq;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetTrackEntryRepository : SqliteNetRepository<TrackEntry>, ITrackEntryRepository
    {
        public SqliteNetTrackEntryRepository(SQLiteConnection conn) : base(conn)
        {
        }

        public void ClearAll()
        {
            connection.DeleteAll<TrackEntry>();
        }

        public TrackEntry LastTrackEntry()
        {
            return connection.Table<TrackEntry>().LastOrDefault();
        }
    }
}
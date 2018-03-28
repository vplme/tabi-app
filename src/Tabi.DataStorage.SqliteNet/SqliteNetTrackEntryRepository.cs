using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;
using SQLiteNetExtensions.Extensions;
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

        public TrackEntry GetWithChildren(Guid id)
        {
            return connection.GetWithChildren<TrackEntry>(id);
        }

        public bool UpdateWithChildren(TrackEntry track)
        {
            try
            {
                connection.UpdateWithChildren(track);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public TrackEntry LastCompletedTrackEntry()
        {
            try
            {
                return connection.Table<TrackEntry>().Where(x => x.NextStopId != 0).OrderBy(x => x.StartTime).Last();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new TrackEntry();
            }
        }

        public IEnumerable<TrackEntry> GetRangeByEndTime(DateTimeOffset begin, DateTimeOffset end)
        {
            try
            {
                return connection.Table<TrackEntry>().Where(x => x.EndTime >= begin && x.EndTime <= end);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<TrackEntry>();
            }
        }
    }
}
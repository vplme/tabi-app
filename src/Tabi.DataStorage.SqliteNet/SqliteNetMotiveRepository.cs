using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetMotiveRepository : SqliteNetRepository<Motive>, IMotiveRepository
    {
        public SqliteNetMotiveRepository(SQLiteConnection conn) : base(conn)
        {
        }

        public Motive GetByStopVisitId(int stopVisitId)
        {
            return connection.Table<Motive>().Where(m => m.StopVisitId == stopVisitId).OrderBy(sv => sv.Timestamp).LastOrDefault();
        }

        public IEnumerable<Motive> StopMotivesAfter(DateTimeOffset begin)
        {
            return connection.Table<Motive>()
                             .Where(x => x.StopVisitId != 0 && x.Timestamp > begin)
                             .OrderBy(x => x.Timestamp);
        }

        public IEnumerable<Motive> TrackMotivesAfter(DateTimeOffset begin) => 
            connection.Table<Motive>()
                      .Where(x => x.TrackId != 0 && x.Timestamp > begin)
                      .OrderBy(x => x.Timestamp);


        public Motive GetByTrackId(int trackId) =>
            connection.Table<Motive>()
                      .Where(m => m.TrackId == trackId)
                      .OrderBy(sv => sv.Timestamp)
                      .LastOrDefault();
    }
}

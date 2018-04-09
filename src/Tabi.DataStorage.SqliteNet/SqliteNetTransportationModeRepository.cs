using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetTransportationModeRepository : SqliteNetRepository<TransportationModeEntry>, ITransportationModeRepository
    {
        public SqliteNetTransportationModeRepository(SQLiteConnection conn) : base(conn)
        {
        }

        public IEnumerable<TransportationModeEntry> GetRange(DateTimeOffset startTime, DateTimeOffset endTime)
        {
            return connection.Table<TransportationModeEntry>().Where(x => x.Timestamp > startTime && x.Timestamp < endTime);
        }

        public IEnumerable<TransportationModeEntry> GetTransportModesByTrackId(Guid trackId)
        {
            return connection.Table<TransportationModeEntry>().Where(x => x.TrackId == trackId);
        }
    }
}

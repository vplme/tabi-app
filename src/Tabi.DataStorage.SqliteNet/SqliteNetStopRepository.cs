using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetStopRepository : SqliteNetRepository<Stop>, IStopRepository
    {
        public SqliteNetStopRepository(SQLiteConnection conn) : base(conn)
        {
        }

        public void Update(Stop s)
        {
            connection.Update(s);
        }

        public IList<Stop> After(DateTimeOffset begin) => connection.Table<Stop>()
                             .Where(x => x.Timestamp > begin)
                             .OrderBy(x => x.Timestamp)
                             .ToList();

        public IEnumerable<Stop> NearestStops(double latitude, double longitude, double radius)
        {

            (double maximumLatitude, double maximumLongitude) =
                Util.AddMetersToPosition(latitude, longitude, radius);
            (double minimumLatitude, double minimumLongitude) =
                Util.AddMetersToPosition(latitude, longitude, -radius);


            return connection.Table<Stop>().Where(s => (s.Latitude >= minimumLatitude &&
                                                           s.Latitude <= maximumLatitude &&
                                                           s.Longitude >= minimumLongitude &&
                                                           s.Longitude <= maximumLongitude));
        }
    }
}

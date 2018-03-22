using System;
using System.Collections.Generic;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetSensorMeasurementSessionRepository : SqliteNetRepository<SensorMeasurementSession>, ISensorMeasurementSessionRepository
    {
        public SqliteNetSensorMeasurementSessionRepository(SQLiteConnection conn) : base(conn)
        {

        }

        public IEnumerable<SensorMeasurementSession> GetRange(DateTimeOffset begin, DateTimeOffset end)
        {
            try
            {
                return connection.Table<SensorMeasurementSession>().Where(x => x.Timestamp >= begin && x.Timestamp <= end);            
            }
            catch (Exception e)
            {
                //log error?
                return new List<SensorMeasurementSession>();
            }
        }

        public bool RemoveRangeBeforeTimestamp(DateTimeOffset timestamp)
        {
            try
            {
                return connection.Table<SensorMeasurementSession>().Delete(x => x.Timestamp < timestamp) > 0;
            }
            catch (Exception e)
            {

                return false;
            }
        }

        public bool UpdateTrackKey(TrackEntry trackEntry)
        {
            try
            {
                var query = $"UPDATE  SensorMeasurementSession" +
                                   " SET TrackEntryKey = '" + trackEntry.UniqueKey.ToString() + "' " +
                                   "WHERE Timestamp > '" + trackEntry.StartTime + "' AND Timestamp < '" + trackEntry.EndTime + "'";

                Console.WriteLine(query);

                connection.Execute(query);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}

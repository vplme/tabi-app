using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetSensorRepository<MotionSensor> : SqliteNetRepository<MotionSensor>, ISensorRepository<MotionSensor> where MotionSensor : DataObjects.MotionSensor, new()
    {
        public SqliteNetSensorRepository(SQLiteConnection conn) : base(conn)
        {

        }

        public IEnumerable<MotionSensor> GetRange(DateTimeOffset begin, DateTimeOffset end)
        {
            try
            {
                return connection.Table<MotionSensor>().Where(x => x.Timestamp >= begin && x.Timestamp <= end);
            }
            catch (Exception e)
            {
                //log error?
                Console.WriteLine();
                return new List<MotionSensor>();
            }
        }

        public bool RemoveRangeBeforeTimestamp(DateTimeOffset timestamp)
        {
            try
            {
                return connection.Table<MotionSensor>().Delete(x => x.Timestamp < timestamp) > 0;
            }
            catch (Exception e)
            {
                //log error?
                Console.WriteLine(e);
                return false;
            }
        }

        public bool UpdateTrackKey(TrackEntry trackEntry, string tableName)
        {
            long startDateTime = trackEntry.StartTime.Ticks;
            long endDateTime = trackEntry.EndTime.Ticks;

            try
            {
                var query = $"UPDATE " + tableName +
                                   " SET TrackEntryKey = '" + trackEntry.Id.ToString() + "' " +
                                   "WHERE Timestamp BETWEEN " + startDateTime + " AND " + endDateTime;

                //Console.WriteLine(query);

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

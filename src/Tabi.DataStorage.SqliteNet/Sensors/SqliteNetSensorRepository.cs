using System;
using System.Collections.Generic;
using SQLite;

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
                return false;
            }
        }
    }
}

using System;
using SQLite;

namespace Tabi.DataObjects
{
    public class SensorMeasurementSession
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int TrackEntryId { get; set; }

        [Ignore]
        public TrackEntry TrackEntry { get; set; }

        public DateTimeOffset Timestamp { get; set; }
        public int AmbientLight { get; set; }
        public int Pedometer { get; set; }
        public bool Proximity{ get; set; }
        public int Compass { get; set; }
        public int BatteryLevel { get; set; }
        public BatteryEntryState BatteryStatus { get; set; }
        public Guid TrackEntryKey { get; set; }
    }
}

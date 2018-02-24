using System;
using System.Collections.Generic;
using System.Text;
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
        public int Compass { get; set; }
        public int BatteryLevel { get; set; }
        public string BatteryStatus { get; set; }
        public string ConnectionType { get; set; }
    }
}

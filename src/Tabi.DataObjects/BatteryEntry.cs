using System;
using SQLite;

namespace Tabi.DataObjects
{
    public class BatteryEntry
    {
        [PrimaryKey]
        public int Id { get; set; }

        public Guid DeviceId { get; set; }
        [Ignore]
        public Device Device { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public int BatteryLevel { get; set; }

        public BatteryEntryState State { get; set; }
    }

    public enum BatteryEntryState { Discharging = 0, Charging = 1, Full = 2, NotCharging = 3, Unknown = 4 }
}

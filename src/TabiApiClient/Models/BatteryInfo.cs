using System;
namespace TabiApiClient.Models
{
    public class BatteryInfo
    {
        public int DeviceId { get; set; }

        public int BatteryLevel { get; set; }

        public BatteryState State { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }

    public enum BatteryState { Discharging = 0, Charging = 1, Full = 2, NotCharging = 3, Unknown = 4 }
}

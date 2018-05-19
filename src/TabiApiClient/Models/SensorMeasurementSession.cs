using System;
namespace TabiApiClient.Models
{
    public class SensorMeasurementSession
    {
        public int Id { get; set; }

        public int DeviceId { get; set; }

        public int AmbientLight { get; set; }

        public int Pedometer { get; set; }

        public bool Proximity { get; set; }

        public int Compass { get; set; }

        public int BatteryLevel { get; set; }

        public int BatteryState { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}

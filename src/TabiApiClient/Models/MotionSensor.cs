using System;
namespace TabiApiClient.Models
{
    public class MotionSensor
    {
        public int Id { get; set; }

        public int DeviceId { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; } 

        public DateTimeOffset Timestamp { get; set; }
    }
}

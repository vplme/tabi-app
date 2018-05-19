using System;
namespace TabiApiClient.Models
{
    public class PositionEntry
    {
        public int Id { get; set; }

        public int DeviceId { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double Accuracy { get; set; }

        public double Speed { get; set; }

        public double Altitude { get; set; }

        public double Heading { get; set; }

        public double DesiredAccuracy { get; set; }

        public double DistanceBetweenPreviousPosition { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}

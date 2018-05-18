using System;
namespace TabiApiClient.Models
{
    public class Stop
    {
        public int Id { get; set; }

        public int DeviceId { get; set; }

        public int LocalId { get; set; }

        public string Name { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}
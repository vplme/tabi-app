using System;

namespace TabiApiClient.Models
{
    public class TransportationMode
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public int TrackId { get; set; }
        public int LocalTrackId { get; set; }

        public bool Walk { get; set; }
        public bool Run { get; set; }
        public bool MobilityScooter { get; set; }
        public bool Car { get; set; }
        public bool Bike { get; set; }
        public bool Moped { get; set; }
        public bool Scooter { get; set; }
        public bool Motorcycle { get; set; }
        public bool Train { get; set; }
        public bool Subway { get; set; }
        public bool Tram { get; set; }
        public bool Bus { get; set; }
        public bool Other { get; set; }

        public DateTimeOffset Timestamp { get; set; }
    }
}

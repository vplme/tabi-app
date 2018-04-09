using System;
using System.Collections.Generic;
using System.Text;
using Tabi.DataObjects;

namespace TabiApiClient.Models
{
    public class TransportationMode
    {
        public Guid TrackId { get; set; }

        //public string Mode { get; set; }

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

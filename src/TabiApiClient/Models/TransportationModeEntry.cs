using System;
using System.Collections.Generic;

namespace TabiApiClient.Models
{
    public class TransportationMode
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public int TrackId { get; set; }
        public int LocalTrackId { get; set; }

        public IEnumerable<string> ActiveModes { get; set; }

        public DateTimeOffset Timestamp { get; set; }
    }
}

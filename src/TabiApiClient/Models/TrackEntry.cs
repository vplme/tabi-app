using System;
using System.Collections.Generic;
using System.Text;

namespace TabiApiClient.Models
{
    public class TrackEntry
    {
        public Guid Id { get; set; }

        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
    }
}

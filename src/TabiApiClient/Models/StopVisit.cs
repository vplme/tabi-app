using System;
namespace TabiApiClient.Models
{
    public class StopVisit
    {
        public int Id { get; set; }

        public int DeviceId { get; set; }

        public int LocalId { get; set; }

        public int StopId { get; set; }

        public int LocalStopId { get; set; }

        public DateTimeOffset BeginTimestamp { get; set; }

        public DateTimeOffset EndTimestamp { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}

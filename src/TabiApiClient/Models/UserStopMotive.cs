using System;
namespace TabiApiClient.Models
{
    public class UserStopMotive
    {
        public int Id { get; set; }

        public int DeviceId { get; set; }

        public int StopId { get; set; }

        public int LocalStopId { get; set; }

        public string Motive { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}
using System;

namespace TabiApiClient.Models
{
    public class Log
    {
        public int Id { get; set; }

        public string Origin { get; set; }

        public string Event { get; set; }

        public string Message { get; set; }

        public int UserId { get; set; }

        public int DeviceId { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}

using System;
namespace TabiApiClient.Models
{
    public class Attribute
    {
        public int DeviceId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}

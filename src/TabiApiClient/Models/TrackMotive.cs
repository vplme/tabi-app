using System;
namespace TabiApiClient.Models
{
    public class TrackMotive
    {
        public int Id { get; set; }

        public int DeviceId { get; set; }

        public int TrackId { get; set; }

        public int LocalTrackId { get; set; }

        public string Motive { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}
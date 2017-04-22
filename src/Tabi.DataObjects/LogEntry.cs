using System;
namespace Tabi.DataObjects
{
    public class LogEntry
    {
        public int Id { get; set; }

        public Guid DeviceId { get; set; }
        public Device Device { get; set; }

        public DateTime Timestamp { get; set; }
    }
}

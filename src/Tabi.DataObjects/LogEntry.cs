using System;
namespace Tabi.DataObjects
{
    public class LogEntry
    {
        public int Id { get; set; }

        public string Event { get; set; }
        public string Message { get; set; }

        public DateTimeOffset Timestamp { get; set; }
    }
}

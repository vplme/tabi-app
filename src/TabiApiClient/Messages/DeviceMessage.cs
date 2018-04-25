using System;

namespace TabiApiClient.Messages
{
    public class DeviceMessage
    {
        public int ID { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string OperatingSystem { get; set; }
    }
}

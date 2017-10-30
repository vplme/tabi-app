using System;
namespace TabiApiClient.Messages
{
    public class DeviceMessage
    {
        public uint ID { get; set; }
        public string UniqueIdentifier { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string OperatingSystem { get; set; }
    }
}

namespace TabiApiClient.Models
{
    public class Device
    {
        public int Id { get; set; }
        public string UniqueIdentifier { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string OperatingSystem { get; set; }
    }
}

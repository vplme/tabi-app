using System;
namespace TabiApiClient.Messages
{
    public class Ping
    {
        public bool Available { get; set; }
        public string Result { get; set; }
    }
}

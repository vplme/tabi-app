using System;
namespace Tabi.Shared
{
    public class TabiConfiguration
    {
        public bool Developer { get; set; }
        public MobileCenterConfiguration MobileCenter { get; set; }
        public string ApiUrl { get; set; }
        public string CertificateKey { get; set; }
        public SensorMeasurementsConfiguration SensorMeasurements { get; set; }
        public LoggingConfiguration Logging { get; set; }
    }

    public class MobileCenterConfiguration
    {
        public bool Enabled { get; set; }
        public string ApiKey { get; set; }
    }

    public class SensorMeasurementsConfiguration
    {
        public bool Enabled { get; set; }
        public bool UserAdjustable { get; set; }
    }

    public class LoggingConfiguration
    {
        public string LogLevel { get; set; }
    }
}

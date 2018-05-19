using System;
using System.Collections.Generic;

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
        public ApiConfiguration Api { get; set; }
    }

    public class ApiConfiguration 
    {
        public string Url { get; set; }

        /// <summary>
        /// Timeout in seconds
        /// </summary>
        /// <value>Timeout in seconds.</value>
        public int Timeout { get; set; }
        public List<string> CertificateKeys { get; set; }

    }

    public class MobileCenterConfiguration
    {
        public bool Enabled { get; set; }
        public bool Distribute { get; set; }
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

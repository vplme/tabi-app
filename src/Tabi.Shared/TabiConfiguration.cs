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
        public UserInterfaceConfiguration UserInterface { get; set; } = new UserInterfaceConfiguration();
        public LoggingConfiguration Logging { get; set; }
        public ApiConfiguration Api { get; set; }
        public MotiveConfiguration Motive { get; set; }
    }

    public class ApiConfiguration
    {
        public string Url { get; set; }

        /// <summary>
        /// Timeout in seconds
        /// </summary>
        /// <value>Timeout in seconds.</value>
        public int Timeout { get; set; }

        /// <summary>
        /// Gets or sets the sync interval in minutes.
        /// </summary>
        /// <value>Sync interval in minutes.</value>
        public int SyncInterval { get; set; } = 30;

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

    public class StopResolverConfiguration
    {
        public bool Enabled { get; set; }
        public bool UserAdjustable { get; set; }

    }

    public class UserInterfaceConfiguration
    {
        public bool StopNameReplaceAllEnabled { get; set; } = true;

        public bool SuggestPossibleNearbyStopsEnabled { get; set; } = true;

        public double SuggestPossibleNearbyStopsDistance { get; set; } = 200;

        public double SuggestPossibleNearbyStopsCount { get; set; } = 2;
    }

    public class LoggingConfiguration
    {
        public string LogLevel { get; set; }
    }

    public class MotiveConfiguration
    {
        public int ShowAmount { get; set; }

        public List<MotiveOption> Options { get; set; }

        public List<MotiveOption> OtherOptions { get; set; }
    }

    public class MotiveOption
    {
        public string Id { get; set; }

        public string Text { get; set; }
    }
}

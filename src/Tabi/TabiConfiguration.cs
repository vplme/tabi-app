using System.Collections.Generic;
using Autofac;

namespace Tabi
{
    public class TabiConfiguration
    {
        public AppConfiguration App { get; set; }

        public MobileCenterConfiguration MobileCenter { get; set; }

        public NotificationsConfiguration Notifications { get; set; }

        public SupportConfiguration Support { get; set; }

        public SensorMeasurementsConfiguration SensorMeasurements { get; set; }

        public UserInterfaceConfiguration UserInterface { get; set; }

        public LoggingConfiguration Logging { get; set; }

        public ApiConfiguration Api { get; set; }

        public MotiveConfiguration Motive { get; set; }

        public TransportationModeConfiguration TransportMode { get; set; }

        public TabiConfiguration()
        {
            App = new AppConfiguration();
            MobileCenter = new MobileCenterConfiguration();
            Notifications = new NotificationsConfiguration();
            Support = new SupportConfiguration();
            SensorMeasurements = new SensorMeasurementsConfiguration();
            UserInterface = new UserInterfaceConfiguration();
            Logging = new LoggingConfiguration();
            Api = new ApiConfiguration();
            Motive = new MotiveConfiguration();
            TransportMode = new TransportationModeConfiguration();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterInstance(this);
            builder.RegisterInstance(MobileCenter);
            builder.RegisterInstance(Notifications);
            builder.RegisterInstance(Support);
            builder.RegisterInstance(SensorMeasurements);
            builder.RegisterInstance(UserInterface);
            builder.RegisterInstance(Logging);
            builder.RegisterInstance(Api);
            builder.RegisterInstance(Motive);
            builder.RegisterInstance(TransportMode);
        }
    }

    public class AppConfiguration
    {
        public bool Developer { get; set; }

        public string LicensesUrl { get; set; }
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

        public bool GzipRequests { get; set; }

        public List<string> CertificateKeys { get; set; } = new List<string>();

    }

    public class SupportConfiguration
    {
        public bool Available { get; set; }

        public string PhoneNumber { get; set; }

        public string DisplayPhoneNumber { get; set; }

        public string Email { get; set; }

        public string EmailSubject { get; set; }

        public string Url { get; set; }

        public string DisplayUrl { get; set; }
    }

    public class NotificationsConfiguration
    {
        public bool Enabled { get; set; }
    }

    public class MobileCenterConfiguration
    {
        public bool Enabled { get; set; }

        public bool Distribute { get; set; }

        public bool Crashes { get; set; }

        public bool Analytics { get; set; }

        public string ApiKey { get; set; }

        public bool DisableAnalyticsOption { get; set; } = true;

        public bool DisableCrashesOption { get; set; } = true;

        public bool ShouldAskConfirmation { get; set; } = true;
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
        /// <summary>
        /// Determines if motives are shown for stops
        /// </summary>
        /// <value><c>true</c> if stops should have motives; otherwise, <c>false</c>.</value>
        public bool Stops { get; set; }

        /// <summary>
        /// Determines if motives are shown for tracks
        /// </summary>
        /// <value><c>true</c> if tracks should have motives; otherwise, <c>false</c>.</value>
        public bool Tracks { get; set; }

        public int ShowAmount { get; set; }

        public List<MotiveOption> Options { get; set; }

        public List<MotiveOption> OtherOptions { get; set; }
    }

    public class MotiveOption
    {
        public string Id { get; set; }

        public string Text { get; set; }
    }

    public class TransportationModeConfiguration
    {
        public bool CustomTransportModes { get; set; }

        public List<TransportOption> Options { get; set; }
    }

    public class TransportOption
    {
        public string Id { get; set; }

        public string Text { get; set; }
    }
}

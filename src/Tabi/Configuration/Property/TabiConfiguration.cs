using System.Collections.Generic;
using Autofac;
using Tabi.Configuration;
using Xamarin.Forms;

namespace Tabi
{
    public class TabiConfiguration : SwitchableConfiguration, ITabiConfiguration
    {

        public TabiConfiguration() : base(new BaseConfiguration())
        {


            App = new AppConfiguration(this);
            MobileCenter = new MobileCenterConfiguration(this);
            Notifications = new NotificationsConfiguration(this);
            Support = new SupportConfiguration(this);
            SensorMeasurements = new SensorMeasurementsConfiguration(this);
            StopResolver = new StopResolverConfiguration(this);

            UserInterface = new UserInterfaceConfiguration(this);
            Logging = new LoggingConfiguration(this);
            Api = new ApiConfiguration(this);
            Motive = new MotiveConfiguration(this);
            TransportMode = new TransportationModeConfiguration(this);


        }

        public TabiConfiguration(LayeredConfiguration layeredConfiguration) : base(layeredConfiguration)
        {
            App = new AppConfiguration(this);
            MobileCenter = new MobileCenterConfiguration(this);
            Notifications = new NotificationsConfiguration(this);
            Support = new SupportConfiguration(this);
            SensorMeasurements = new SensorMeasurementsConfiguration(this);
            StopResolver = new StopResolverConfiguration(this);
            UserInterface = new UserInterfaceConfiguration(this);
            Logging = new LoggingConfiguration(this);
            Api = new ApiConfiguration(this);
            Motive = new MotiveConfiguration(this);
            TransportMode = new TransportationModeConfiguration(this);
        }

        public IAppConfiguration App
        {
            get => GetProperty<IAppConfiguration>();
            set => SetProperty<IAppConfiguration>(value);
        }

        public IMobileCenterConfiguration MobileCenter
        {
            get => GetProperty<IMobileCenterConfiguration>();
            set => SetProperty<IMobileCenterConfiguration>(value);
        }

        public INotificationsConfiguration Notifications
        {
            get => GetProperty<INotificationsConfiguration>();
            set => SetProperty<INotificationsConfiguration>(value);
        }

        public ISupportConfiguration Support
        {
            get => GetProperty<ISupportConfiguration>();
            set => SetProperty<ISupportConfiguration>(value);
        }

        public IStopResolverConfiguration StopResolver
        {
            get => GetProperty<IStopResolverConfiguration>();
            set => SetProperty<IStopResolverConfiguration>(value);
        }


        public ISensorMeasurementsConfiguration SensorMeasurements
        {
            get => GetProperty<ISensorMeasurementsConfiguration>();
            set => SetProperty<ISensorMeasurementsConfiguration>(value);
        }

        public IUserInterfaceConfiguration UserInterface
        {
            get => GetProperty<IUserInterfaceConfiguration>();
            set => SetProperty<IUserInterfaceConfiguration>(value);
        }

        public ILoggingConfiguration Logging
        {
            get => GetProperty<ILoggingConfiguration>();
            set => SetProperty<ILoggingConfiguration>(value);
        }

        public IApiConfiguration Api
        {
            get => GetProperty<IApiConfiguration>();
            set => SetProperty<IApiConfiguration>(value);
        }

        public IMotiveConfiguration Motive
        {
            get => GetProperty<IMotiveConfiguration>();
            set => SetProperty<IMotiveConfiguration>(value);
        }

        public ITransportationModeConfiguration TransportMode
        {
            get => GetProperty<ITransportationModeConfiguration>();
            set => SetProperty<ITransportationModeConfiguration>(value);
        }



        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterInstance(this);
            builder.RegisterInstance((ITabiConfiguration)this);
            builder.RegisterInstance(MobileCenter);
            builder.RegisterInstance(Notifications);
            builder.RegisterInstance(Support);
            builder.RegisterInstance(SensorMeasurements);
            builder.RegisterInstance(UserInterface);
            builder.RegisterInstance(StopResolver)
                   .As<Logic.IStopResolverConfiguration>();
            builder.RegisterInstance(Logging);
            builder.RegisterInstance(Api);
            builder.RegisterInstance(Motive);
            builder.RegisterInstance(TransportMode);
        }
    }

    public class AppConfiguration : SubBaseConfiguration, IAppConfiguration
    {
        public AppConfiguration(ITabiPropertyConfiguration baseConfiguration) : base(baseConfiguration)
        {
        }

        public string AppName
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        public bool Developer
        {
            get => GetProperty<bool>();
            set => SetProperty(value);
        }

        public string LicensesUrl
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        public string AgreementUrl
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }
    }

    public class ApiConfiguration : SubBaseConfiguration, IApiConfiguration
    {
        public ApiConfiguration(ITabiPropertyConfiguration baseConfiguration) : base(baseConfiguration)
        {
        }

        public string Url
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        /// <summary>
        /// Timeout in seconds
        /// </summary>
        /// <value>Timeout in seconds.</value>
        public int Timeout
        {
            get => GetProperty<int>();
            set => SetProperty(value);
        }

        /// <summary>
        /// Gets or sets the sync interval in minutes.
        /// </summary>
        /// <value>Sync interval in minutes.</value>
        public int SyncInterval
        {
            get => GetProperty<int>();
            set => SetProperty(value);
        }

        public string ClientIdentifier
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        public string ClientKey
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        public bool GzipRequests
        {
            get => GetProperty<bool>();
            set => SetProperty(value);
        }

        public List<string> CertificateKeys
        {
            get => GetProperty<List<string>>();
            set => SetProperty(value);
        }

    }

    public class SupportConfiguration : SubBaseConfiguration, ISupportConfiguration
    {
        public SupportConfiguration(ITabiPropertyConfiguration baseConfiguration) : base(baseConfiguration)
        {
        }

        public bool Available
        {
            get => GetProperty<bool>();
            set => SetProperty(value);
        }

        public string PhoneNumber
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        public string DisplayPhoneNumber
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        public string Email
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        public string EmailSubject
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        public string Url
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        public string DisplayUrl
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }
    }

    public class NotificationsConfiguration : SubBaseConfiguration, INotificationsConfiguration
    {
        public NotificationsConfiguration(ITabiPropertyConfiguration baseConfiguration) : base(baseConfiguration)
        {
        }

        public bool Enabled
        {
            get => GetProperty<bool>();
            set => SetProperty(value);
        }
    }

    public class MobileCenterConfiguration : SubBaseConfiguration, IMobileCenterConfiguration
    {
        public MobileCenterConfiguration(ITabiPropertyConfiguration baseConfiguration) : base(baseConfiguration)
        {
        }

        public bool Enabled
        {
            get => GetProperty<bool>();
            set => SetProperty(value);
        }

        public bool Distribute
        {
            get => GetProperty<bool>();
            set => SetProperty(value);
        }

        public bool Crashes
        {
            get => GetProperty<bool>();
            set => SetProperty(value);
        }

        public bool Analytics
        {
            get => GetProperty<bool>();
            set => SetProperty(value);
        }

        public string ApiKey
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        public bool DisableAnalyticsOption
        {
            get => GetProperty<bool>();
            set => SetProperty(value);
        }

        public bool DisableCrashesOption
        {
            get => GetProperty<bool>();
            set => SetProperty(value);
        }

        public bool ShouldAskConfirmation
        {
            get => GetProperty<bool>();
            set => SetProperty(value);
        }
    }

    public class SensorMeasurementsConfiguration : SubBaseConfiguration, ISensorMeasurementsConfiguration
    {
        public SensorMeasurementsConfiguration(ITabiPropertyConfiguration baseConfiguration) : base(baseConfiguration)
        {
        }

        public bool Enabled
        {
            get => GetProperty<bool>();
            set => SetProperty(value);
        }
        public bool UserAdjustable
        {
            get => GetProperty<bool>();
            set => SetProperty(value);
        }
    }

    public class StopResolverConfiguration : SubBaseConfiguration, Logic.IStopResolverConfiguration, IStopResolverConfiguration
    {
        public StopResolverConfiguration(ITabiPropertyConfiguration baseConfiguration) : base(baseConfiguration)
        {
        }

        public bool Enabled
        {
            get => GetProperty<bool>();
            set => SetProperty(value);
        }
        public bool UserAdjustable
        {
            get => GetProperty<bool>();
            set => SetProperty(value);
        }
        public int Time
        {
            get => GetProperty<int>();
            set => SetProperty(value);
        }
        public int GroupRadius
        {
            get => GetProperty<int>();
            set => SetProperty(value);
        }
        public int MinStopAccuracy
        {
            get => GetProperty<int>();
            set => SetProperty(value);
        }
        public int StopMergeRadius
        {
            get => GetProperty<int>();
            set => SetProperty(value);
        }
        public int StopMergeMaxTravelRadius
        {
            get => GetProperty<int>();
            set => SetProperty(value);
        }
    }

    public class UserInterfaceConfiguration : SubBaseConfiguration, IUserInterfaceConfiguration
    {
        public UserInterfaceConfiguration(ITabiPropertyConfiguration baseConfiguration) : base(baseConfiguration)
        {
        }

        public bool StopNameReplaceAllEnabled
        {
            get => GetProperty<bool>();
            set => SetProperty(value);
        }

        public bool SuggestPossibleNearbyStopsEnabled
        {
            get => GetProperty<bool>();
            set => SetProperty(value);
        }

        public double SuggestPossibleNearbyStopsDistance
        {
            get => GetProperty<double>();
            set => SetProperty(value);
        }

        public double SuggestPossibleNearbyStopsCount
        {
            get => GetProperty<double>();
            set => SetProperty(value);
        }

        public bool ShowNotificationOnAppTermination
        {
            get => GetProperty<bool>();
            set => SetProperty(value);
        }
    }

    public class LoggingConfiguration : SubBaseConfiguration, ILoggingConfiguration
    {
        public LoggingConfiguration(ITabiPropertyConfiguration baseConfiguration) : base(baseConfiguration)
        {
        }

        public string LogLevel
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }
    }

    public class MotiveConfiguration : SubBaseConfiguration, IMotiveConfiguration
    {
        public MotiveConfiguration(ITabiPropertyConfiguration baseConfiguration) : base(baseConfiguration)
        {
        }

        /// <summary>
        /// Determines if motives are shown for stops
        /// </summary>
        /// <value><c>true</c> if stops should have motives; otherwise, <c>false</c>.</value>
        public bool Stops
        {
            get => GetProperty<bool>();
            set => SetProperty(value);
        }

        /// <summary>
        /// Determines if motives are shown for tracks
        /// </summary>
        /// <value><c>true</c> if tracks should have motives; otherwise, <c>false</c>.</value>
        public bool Tracks
        {
            get => GetProperty<bool>();
            set => SetProperty(value);
        }

        public int ShowAmount
        {
            get => GetProperty<int>();
            set => SetProperty(value);
        }

        public List<MotiveOption> Options
        {
            get => GetProperty<List<MotiveOption>>();
            set => SetProperty(value);
        }

        public List<MotiveOption> OtherOptions
        {
            get => GetProperty<List<MotiveOption>>();
            set => SetProperty(value);
        }
    }

    public class MotiveOption
    {
        public string Id { get; set; }

        public string Text { get; set; }
    }

    public class TransportationModeConfiguration : SubBaseConfiguration, ITransportationModeConfiguration
    {
        public TransportationModeConfiguration(ITabiPropertyConfiguration baseConfiguration) : base(baseConfiguration)
        {
        }

        public bool CustomTransportModes
        {
            get => GetProperty<bool>();
            set => SetProperty(value);
        }

        public List<TransportOption> Options
        {
            get => GetProperty<List<TransportOption>>();
            set => SetProperty(value);
        }
    }

    public class TransportOption
    {
        public string Id { get; set; }

        public string Text { get; set; }
    }
}

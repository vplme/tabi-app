using Xamarin.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Microsoft.Azure.Mobile.Distribute;
using Microsoft.Azure.Mobile.Push;
using PCLStorage;
using Tabi.DataStorage;
using Tabi.Logging;
using Tabi.iOS.Helpers;
using Tabi.DataObjects.CollectionProfile;
using Tabi.Shared.Helpers;
using Tabi.Shared.Sensors;
using Autofac.Core;
using Autofac;
using Tabi.DataStorage.SqliteNet;
using SQLite;
using Tabi.ViewModels;
using Tabi.Core;
using TabiApiClient;
using Tabi.Shared.Pages.OnBoarding;
using Tabi.Shared.ViewModels;
using System.Net;
using Tabi.Shared.DataSync;
using Tabi.Shared;
using Tabi.Shared.Config;
using System.Globalization;
using Tabi.Shared.Resx;
using System;

namespace Tabi
{
    public partial class App : Application
    {
        private static IContainer _container;

        public static IContainer Container { get => _container; }

        public const string AppIdentifier = "tabi/tabi-app";

        public static bool DebugMode
        {
            get; private set;
        }

        public const string LogFilePath = "tabi.log";
        public static bool Developer;
        public static double ScreenHeight;
        public static double ScreenWidth;
        public static TabiConfiguration TabiConfig;
        public static bool LocationPermissionsGranted;

        public static CollectionProfile CollectionProfile { get; private set; }

        ILocationManager locationManager;
        ISensorManager sensorManager;

        static App()
        {
            IConfiguration configuration = RetrieveConfiguration();
            TabiConfig = ConvertTabiConfiguration(configuration);
        }

        private static IConfiguration RetrieveConfiguration()
        {
            var builder = new ConfigurationBuilder().AddXmlFile(new ResourceFileProvider(), "config.xml", false, false);
            return builder.Build();
        }

        private static TabiConfiguration ConvertTabiConfiguration(IConfiguration configuration)
        {
            TabiConfiguration config = configuration.Get<TabiConfiguration>();
            return config;
        }

        public App(IModule[] platformSpecificModules)
        {
            PrepareContainer(platformSpecificModules);
            // Resolve repo manager immediately for setup
            Container.Resolve<IRepoManager>();

            SetupLocalization();

            // Setup logging
            SetupLogging();

            CollectionProfile = CollectionProfile.GetDefaultProfile();

            InitializeComponent();

            SetupCertificatePinningCheck();
            SetupLocationManager();
            SetupSensorManager();

            Developer = TabiConfig.Developer;
            DebugMode = Developer;
#if DEBUG
            DebugMode = true;
#endif

            if (!string.IsNullOrEmpty(TabiConfig.MobileCenter.ApiKey))
            {
                if (TabiConfig.MobileCenter.Distribute)
                {
                    // Start with Distribute
                    MobileCenter.Start(TabiConfig.MobileCenter.ApiKey,
                                   typeof(Analytics), typeof(Crashes), typeof(Distribute), typeof(Push));
                }
                else
                {
                    MobileCenter.Start(TabiConfig.MobileCenter.ApiKey,
                                      typeof(Analytics), typeof(Crashes), typeof(Push));
                }

                Log.Debug("MobileCenter started with apikey");
                MobileCenter.SetEnabledAsync(TabiConfig.MobileCenter.Enabled);
                Log.Debug($"MobileCenter enabled: {TabiConfig.MobileCenter.Enabled}");
            }

            NavigationPage navigationPage = new NavigationPage();

            if (!Settings.Current.PermissionsGranted)
            {
                navigationPage.PushAsync(new WelcomePage());
            }
            else
            {
                navigationPage.PushAsync(new ActivityOverviewPage());
            }

            MainPage = navigationPage;
        }

        private void PrepareContainer(IModule[] platformSpecificModules)
        {
            var containerBuilder = new Autofac.ContainerBuilder();

            containerBuilder.RegisterInstance(GetSqliteConnection()).As<SQLiteConnection>();
            containerBuilder.RegisterType<SqliteNetRepoManager>().As<IRepoManager>().SingleInstance();

            containerBuilder.RegisterType<SyncService>();
            containerBuilder.RegisterType<ApiClient>().WithParameter("apiLocation", TabiConfig.Api.Url);

            containerBuilder.RegisterInstance(TabiConfig).As<TabiConfiguration>();

            containerBuilder.RegisterType<DateService>().SingleInstance();
            containerBuilder.RegisterType<DataResolver>();
            containerBuilder.RegisterType<DbLogWriter>();
            containerBuilder.RegisterType<BatteryHelper>().SingleInstance();

            containerBuilder.RegisterType<SettingsViewModel>();
            containerBuilder.RegisterType<ActivityOverviewViewModel>();
            containerBuilder.RegisterType<DaySelectorViewModel>();
            containerBuilder.RegisterType<StopDetailViewModel>();
            containerBuilder.RegisterType<TransportSelectionViewModel>();
            containerBuilder.RegisterType<WelcomeViewModel>();
            containerBuilder.RegisterType<LoginViewModel>();
            containerBuilder.RegisterType<LocationAccessViewModel>();
            containerBuilder.RegisterType<MotionAccessViewModel>();
            containerBuilder.RegisterType<ThanksViewModel>();
            containerBuilder.RegisterType<TrackDetailViewModel>();
            containerBuilder.RegisterType<StopDetailNameViewModel>();
            containerBuilder.RegisterType<StopDetailMotiveViewModel>();

            RegisterPlatformSpecificModules(platformSpecificModules, containerBuilder);

            _container = containerBuilder.Build();
        }

        public static void SetupCertificatePinningCheck()
        {
            TabiConfig.Api?.CertificateKeys?.ForEach(key => EndpointConfiguration.AddPublicKeyString(key));
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.DefaultConnectionLimit = 8;
            ServicePointManager.ServerCertificateValidationCallback = EndpointConfiguration.ValidateServerCertificate;
        }

        private void SetupLocalization()
        {
            // This lookup NOT required for Windows platforms - the Culture will be automatically set
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS || Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
            {
                // determine the correct, supported .NET culture
                var ci = App.Container.Resolve<ILocalize>().GetCurrentCultureInfo();
                Shared.Resx.AppResources.Culture = ci; // set the RESX for resource localization
                App.Container.Resolve<ILocalize>().SetLocale(ci); // set the Thread for locale-aware methods
            }
        }

        private void RegisterPlatformSpecificModules(IModule[] platformSpecificModules, ContainerBuilder containerBuilder)
        {
            foreach (var platformSpecificModule in platformSpecificModules)
            {
                containerBuilder.RegisterModule(platformSpecificModule);
            }
        }

        private void SetupLogging()
        {
            LogSeverity level = Log.SeverityFromString(TabiConfig.Logging.LogLevel);
            MultiLogger mLogger = new MultiLogger();
            mLogger.SetLogLevel(level);

            mLogger.AddLogger(new ConsoleLogWriter());
            mLogger.AddLogger(new FileLogWriter());

            DbLogWriter dbLogWriter = App.Container.Resolve<DbLogWriter>();
            mLogger.AddLogger(dbLogWriter);
            Log.SetLogger(mLogger);
            Log.Info("Logging Setup");
        }

        private SQLiteConnection GetSqliteConnection()
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            var dbPath = PortablePath.Combine(rootFolder.Path, "tabi.db");
            return new SQLiteConnection(
                dbPath,
                    SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex,
                    false);
        }



        private void SetupLocationManager()
        {
            locationManager = Container.Resolve<ILocationManager>();
            Settings.Current.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "Tracking")
                {
                    if (Settings.Current.Tracking && !locationManager.IsListening)
                    {
                        locationManager.StartLocationUpdates();
                    }
                    else if (!Settings.Current.Tracking && locationManager.IsListening)
                    {
                        locationManager.StopLocationUpdates();
                    }
                }
                if (e.PropertyName == "PermissionsGranted")
                {
                    if (Settings.Current.PermissionsGranted)
                    {
                        Settings.Current.Tracking = true;
                    }
                }
            };
            if (Settings.Current.Tracking)
            {
                locationManager.StartLocationUpdates();
            }
        }

        private void SetupSensorManager()
        {
            sensorManager = Container.Resolve<ISensorManager>();

            Settings.Current.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "Tracking" || e.PropertyName == "SensorMeasurements")
                {
                    bool enabled = TabiConfig.SensorMeasurements.Enabled && Settings.Current.Tracking;

                    if (TabiConfig.SensorMeasurements.UserAdjustable)
                    {
                        enabled = enabled && Settings.Current.SensorMeasurements;
                    }

                    if (enabled && !sensorManager.IsListening)
                    {
                        sensorManager.StartSensorUpdates();
                    }
                    else if (!enabled && sensorManager.IsListening)
                    {
                        sensorManager.StopSensorUpdates();
                    }
                }
            };

            bool enabledOnStart = TabiConfig.SensorMeasurements.Enabled && Settings.Current.Tracking;

            if (TabiConfig.SensorMeasurements.UserAdjustable)
            {
                enabledOnStart = enabledOnStart && Settings.Current.SensorMeasurements;
            }

            if (enabledOnStart)
            {
                sensorManager.StartSensorUpdates();
            }
        }

        protected override void OnStart()
        {
            Log.Info("App.OnStart");
        }

        protected override void OnSleep()
        {
            Log.Info("App.OnSleep");
        }

        protected override void OnResume()
        {
            Log.Info("App.OnResume");
        }
    }

}
using Xamarin.Forms;
using Microsoft.Extensions.Configuration;
using PCLStorage;
using Tabi.DataStorage;
using Tabi.Logging;
using Tabi.DataObjects.CollectionProfile;
using Autofac.Core;
using Autofac;
using Tabi.DataStorage.SqliteNet;
using SQLite;
using Tabi.ViewModels;
using TabiApiClient;
using System.Net;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Tabi.Logic;
using System;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using Microsoft.AppCenter.Push;
using System.Collections.Generic;
using Tabi.DataSync;
using Tabi.Files.ResourceFilesSupport;
using Tabi.Helpers;
using Tabi.Localization;
using Tabi.Pages;
using Tabi.Pages.OnBoarding;
using Tabi.Resx;
using Tabi.Sensors;
using XEFileSystem = Xamarin.Essentials.FileSystem;
using System.IO;

namespace Tabi
{
    public partial class App : Xamarin.Forms.Application
    {
        private static IContainer _container;

        private static LayeredConfiguration _layeredConfig;

        public static IContainer Container { get => _container; }

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
            TabiConfig = ConvertTabiConfiguration();
        }

        private static IConfiguration RetrieveConfiguration(string filename)
        {
            var builder = new ConfigurationBuilder().AddJsonFile(new ResourceFileProvider(), filename, false, false);
            return builder.Build();


        }

        private static void ReloadStoredRemoteConfig()
        {
            IConfiguration storedConfig = RemoteConfigService.LoadStoredConfiguration();
            if (storedConfig != null)
            {
                TabiConfiguration deviceConfig = storedConfig.Get<TabiConfiguration>();
                _layeredConfig.AddConfiguration(1, deviceConfig);
            }

        }

        private static TabiConfiguration ConvertTabiConfiguration()
        {
            TabiConfiguration config = RetrieveConfiguration("config.json").Get<TabiConfiguration>();
            TabiConfiguration def = RetrieveConfiguration("default.json").Get<TabiConfiguration>();

            _layeredConfig = new LayeredConfiguration();


            _layeredConfig.AddConfiguration(10, def);
            _layeredConfig.AddConfiguration(5, config);
            ReloadStoredRemoteConfig();

            TabiConfiguration combined = new TabiConfiguration(_layeredConfig);

            return combined;
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

            EndpointValidator.Init(TabiConfig.Api.CertificateKeys);

            SetupLocationManager();
            SetupSensorManager();

            Developer = TabiConfig.App.Developer;
            DebugMode = Developer;
#if DEBUG
            DebugMode = true;
#endif
            SetupAppCenter(TabiConfig.MobileCenter);


            Xamarin.Forms.NavigationPage navigationPage = new Xamarin.Forms.NavigationPage();
            navigationPage.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetPrefersLargeTitles(true);
            if (!Settings.Current.PermissionsGranted)
            {
                navigationPage.PushAsync(new WelcomePage());
            }
            else
            {
                navigationPage.PushAsync(new ActivityOverviewPage());
            }

            var timer = new System.Threading.Timer((o) =>
            {
                if (Settings.Current.PermissionsGranted && Settings.Current.AutoUpload)
                {
                    IDataUploadTask task = Container.Resolve<IDataUploadTask>();
                    task.Start();
                }

            }, null, 0, 2 * 60000);

            MainPage = navigationPage;

            TimeSpan timeAgo = DateTime.Now - new DateTime(Settings.Current.LastUpload);

            Analytics.TrackEvent("App loaded", new Dictionary<string, string>() {
                { "Device ID", Settings.Current.Device.ToString() },
                { "Last Upload Ago", Math.Round(timeAgo.TotalMinutes).ToString()}
            });
        }

        private void PrepareContainer(IModule[] platformSpecificModules)
        {
            var containerBuilder = new Autofac.ContainerBuilder();

            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
            {
                // Register if iOS, otherwise Android should have registered a specific Android one
                containerBuilder.RegisterInstance(GetSqliteConnection()).As<SQLiteConnection>();
            }
            containerBuilder.RegisterType<SqliteNetRepoManager>().As<IRepoManager>().SingleInstance();
            containerBuilder.RegisterType<SyncService>().SingleInstance();
            containerBuilder.RegisterType<ApiClient>()
                            .WithParameter("clientIdentifier", TabiConfig.Api.ClientIdentifier)
                            .WithParameter("clientKey", TabiConfig.Api.ClientKey)
                            .WithParameter("apiLocation", TabiConfig.Api.Url)
                            .WithParameter("gzip", TabiConfig.Api.GzipRequests);

            TabiConfig.ConfigureContainer(containerBuilder);
            Action updateAct = App.ReloadStoredRemoteConfig;
            containerBuilder.RegisterType<RemoteConfigService>()
                            .WithParameter("hasBeenUpdated", updateAct)
                            .WithParameter("deviceId", Settings.Current.Device);

            containerBuilder.RegisterType<DateService>().SingleInstance();
            containerBuilder.RegisterType<DataResolver>();
            containerBuilder.RegisterType<StopResolver>()
                            .As<IStopResolver>();

            containerBuilder.RegisterType<DbLogWriter>();
            containerBuilder.RegisterType<BatteryHelper>().SingleInstance();

            containerBuilder.RegisterType<WelcomeViewModel>();
            containerBuilder.RegisterType<LoginViewModel>();
            containerBuilder.RegisterType<LocationAccessViewModel>();
            containerBuilder.RegisterType<MotionAccessViewModel>();
            containerBuilder.RegisterType<ThanksViewModel>();

            containerBuilder.RegisterType<TourViewModel>();

            containerBuilder.RegisterType<ActivityOverviewViewModel>();
            containerBuilder.RegisterType<DayCommentViewModel>();
            containerBuilder.RegisterType<DaySelectorViewModel>();
            containerBuilder.RegisterType<SettingsViewModel>();

            containerBuilder.RegisterType<TrackDetailViewModel>();
            containerBuilder.RegisterType<TrackDetailMotiveViewModel>();
            containerBuilder.RegisterType<TransportSelectionViewModel>();
            containerBuilder.RegisterType<CustomTransportSelectionViewModel>();

            containerBuilder.RegisterType<StopDetailViewModel>();
            containerBuilder.RegisterType<StopDetailNameViewModel>();
            containerBuilder.RegisterType<StopDetailMotiveViewModel>();

            containerBuilder.RegisterType<SearchMotiveViewModel>();
            containerBuilder.RegisterType<MotiveSelectionViewModel>();

            RegisterPlatformSpecificModules(platformSpecificModules, containerBuilder);

            _container = containerBuilder.Build();
        }

        private void SetupLocalization()
        {
            // This lookup NOT required for Windows platforms - the Culture will be automatically set
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS || Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
            {
                // determine the correct, supported .NET culture
                var ci = App.Container.Resolve<ILocalize>().GetCurrentCultureInfo();
                AppResources.Culture = ci; // set the RESX for resource localization
                App.Container.Resolve<ILocalize>().SetLocale(ci); // set the Thread for locale-aware methods
            }
        }

        private void SetupAppCenter(IMobileCenterConfiguration mobileCenterConfiguration)
        {
            if (!string.IsNullOrEmpty(mobileCenterConfiguration.ApiKey))
            {
                if (mobileCenterConfiguration.Distribute)
                {
                    // Start with Distribute
                    AppCenter.Start(mobileCenterConfiguration.ApiKey,
                                   typeof(Analytics), typeof(Crashes), typeof(Distribute), typeof(Push));
                }
                else
                {
                    AppCenter.Start(mobileCenterConfiguration.ApiKey,
                                      typeof(Analytics), typeof(Crashes), typeof(Push));
                }

                Log.Debug("MobileCenter started with apikey");
                AppCenter.SetEnabledAsync(mobileCenterConfiguration.Enabled);

                Analytics.SetEnabledAsync(Settings.Current.AnalyticsGranted);
                Crashes.SetEnabledAsync(Settings.Current.CrashesGranted);

                // Configuration overrides positive user preference
                if (!mobileCenterConfiguration.Analytics)
                {
                    Analytics.SetEnabledAsync(false);
                }
                if (!mobileCenterConfiguration.Crashes)
                {
                    Crashes.SetEnabledAsync(false);
                }

                // Show a dialog if a crash occurs asking for user confirmation to send it
                if (mobileCenterConfiguration.ShouldAskConfirmation)
                {
                    Crashes.ShouldAwaitUserConfirmation = CrashConfirmationHandler;
                }

                Settings.Current.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == nameof(Settings.CrashesGranted))
                    {
                        Crashes.SetEnabledAsync(Settings.Current.CrashesGranted);
                    }
                    else if (e.PropertyName == nameof(Settings.AnalyticsGranted))
                    {
                        Analytics.SetEnabledAsync(Settings.Current.AnalyticsGranted);
                    }
                };

                Log.Debug($"MobileCenter enabled: {mobileCenterConfiguration.Enabled}");
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
            SQLiteConnection connection = new SQLiteConnection(
                dbPath,
                    SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex,
                    false);

            return connection;
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

        bool CrashConfirmationHandler()
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                Current.MainPage.DisplayActionSheet("Crash detected. Send anonymous crash report?", null, null, "Send", "Always Send", "Don't Send").ContinueWith((arg) =>
                {
                    var answer = arg.Result;
                    UserConfirmation userConfirmationSelection;
                    if (answer == "Send")
                    {
                        userConfirmationSelection = UserConfirmation.Send;
                    }
                    else if (answer == "Always Send")
                    {
                        userConfirmationSelection = UserConfirmation.AlwaysSend;
                    }
                    else
                    {
                        userConfirmationSelection = UserConfirmation.DontSend;
                    }
                    Log.Info("User selected confirmation option: \"" + answer + "\"");
                    Crashes.NotifyUserConfirmation(userConfirmationSelection);
                });
            });

            return true;
        }

        protected override void OnStart()
        {

            Log.Info("App.OnStart");

            var remoteconfig = Container.Resolve<RemoteConfigService>();
            remoteconfig.UpdateRemoteConfig(Settings.Current.Device);
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
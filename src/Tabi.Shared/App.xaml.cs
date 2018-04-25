using System;
using System.Reflection;
using Xamarin.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Microsoft.Azure.Mobile.Distribute;
using Microsoft.Azure.Mobile.Push;
using PCLStorage;
using Tabi.Shared.Extensions;
using Tabi.DataStorage;
using Tabi.Logging;
using Tabi.Pages;
using System.Threading.Tasks;
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

namespace Tabi
{
    public partial class App : Application
    {
        private static IContainer _container;

        public static IContainer Container { get => _container; }

        public const string LogFilePath = "tabi.log";
        public static bool Developer;
        public static double ScreenHeight;
        public static double ScreenWidth;
        public static readonly IConfigurationRoot Configuration;
        public static bool LocationPermissionsGranted;

        public static CollectionProfile CollectionProfile { get; private set; }

        ILocationManager locationManager;
        ISensorManager sensorManager;

        static App()
        {
            // Load configuration from embeddedresource file.
            Assembly assembly = typeof(App).GetTypeInfo().Assembly;
            var builder = new ConfigurationBuilder().AddEmbeddedXmlFile(assembly, "tabi.config");
            Configuration = builder.Build();

        }


        public App(IModule[] platformSpecificModules)
        {
            PrepareContainer(platformSpecificModules);


            // Setup logging
            SetupLogging();


            CollectionProfile = CollectionProfile.GetDefaultProfile();

            InitializeComponent();


            SetupLocationManager();
            SetupSensorManager();

            Developer = Convert.ToBoolean(Configuration["developer"]);

            string apiKey = Configuration["mobilecenter:apikey"];
            bool mobileCenterEnabled = Convert.ToBoolean(Configuration["mobilecenter:enabled"]);
           

            if (!apiKey.Equals(""))
            {
                MobileCenter.Start(apiKey,
                                   typeof(Analytics), typeof(Crashes), typeof(Distribute), typeof(Push));
                Log.Debug("MobileCenter started with apikey");
                MobileCenter.SetEnabledAsync(mobileCenterEnabled);
                Log.Debug($"MobileCenter enabled: {mobileCenterEnabled}");
            }

            MainPage = new NavigationPage(new ActivityOverviewPage());
            if (!Settings.Current.PermissionsGranted)
            {
                MainPage.Navigation.PushModalAsync(new IntroPage());
            }
        }

        private void PrepareContainer(IModule[] platformSpecificModules)
        {
            var containerBuilder = new Autofac.ContainerBuilder();
            RegisterPlatformSpecificModules(platformSpecificModules, containerBuilder);

            containerBuilder.RegisterInstance(GetSqliteConnection()).As<SQLiteConnection>();
            containerBuilder.RegisterType<SqliteNetRepoManager>().As<IRepoManager>().SingleInstance();

            containerBuilder.RegisterType<SyncService>();
            containerBuilder.RegisterType<ApiClient>().WithParameter("apiLocation", Configuration["api-url"]);

            containerBuilder.RegisterType<DateService>().SingleInstance();
            containerBuilder.RegisterType<DataResolver>();
            containerBuilder.RegisterType<DbLogWriter>();
            containerBuilder.RegisterType<BatteryHelper>().SingleInstance();

            containerBuilder.RegisterType<SettingsViewModel>();
            containerBuilder.RegisterType<ActivityOverviewViewModel>();
            containerBuilder.RegisterType<DaySelectorViewModel>();
            containerBuilder.RegisterType<StopDetailViewModel>();
            containerBuilder.RegisterType<TransportSelectionViewModel>();

            _container = containerBuilder.Build();
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
            LogSeverity level = Log.SeverityFromString(Configuration["logging:level"]);
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
                if (e.PropertyName == "Tracking")
                {
                    if (Settings.Current.Tracking && !sensorManager.IsListening)
                    {
                        sensorManager.StartSensorUpdates();
                    }
                    else if (!Settings.Current.Tracking && sensorManager.IsListening)
                    {
                        sensorManager.StopSensorUpdates();
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
                sensorManager.StartSensorUpdates();
            }

        }


        protected override void OnStart()
        {
            Log.Info("App.OnStart");
            CheckAuthorization(Settings.Current.Device);
        }

        async Task CheckAuthorization(int deviceId)
        {
            if (Settings.Current.PermissionsGranted)
            {
                TabiApiClient.ApiClient apiClient = Container.Resolve<ApiClient>();
                await apiClient.Authenticate(Settings.Current.Username, Settings.Current.Password);
                bool unauth = await apiClient.IsDeviceUnauthorized(deviceId);

                if (unauth)
                {
                    Log.Debug("Unauthorized!");
                    Settings.Current.PermissionsGranted = false;
                    await MainPage.Navigation.PushModalAsync(new IntroPage());
                }
            }

        }


        protected override void OnSleep()
        {
            Log.Info("App.OnSleep");
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            Log.Info("App.OnResume");
            // Handle when your app resumes
            CheckAuthorization(Settings.Current.Device);
        }

    }

}
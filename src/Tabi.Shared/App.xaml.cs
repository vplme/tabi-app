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
using Tabi.Shared.Collection;
using Tabi.Pages;
using System.Threading.Tasks;
using Tabi.iOS.Helpers;
using Tabi.DataObjects.CollectionProfile;
using Tabi.Shared.Helpers;
using Tabi.Shared.Sensors;

namespace Tabi
{
    public partial class App : Application
    {
        public const string LogFilePath = "tabi.log";
        public static bool Developer;
        public static readonly DateService DateService;
        public static double ScreenHeight;
        public static double ScreenWidth;
        public static readonly SyncService SyncService;
        public static readonly IConfigurationRoot Configuration;
        public static bool LocationPermissionsGranted;
        public static IRepoManager RepoManager;

        public static CollectionProfile CollectionProfile { get; private set; }

        ILocationManager locationManager;
        ISensorManager sensorManager;

        static App()
        {
            // Load configuration from embeddedresource file.
            Assembly assembly = typeof(App).GetTypeInfo().Assembly;
            var builder = new ConfigurationBuilder().AddEmbeddedXmlFile(assembly, "tabi.config");
            Configuration = builder.Build();

            DateService = new DateService();
            SyncService = new SyncService(Configuration["api-url"]);
        }


        public App()
        {
            // Setup logging
            SetupLogging();

            SetupSQLite();

            CollectionProfile = CollectionProfile.GetDefaultProfile();

            // Initialize Device Identifier on empty database
            if (RepoManager.DeviceRepository.Count() == 0)
            {
                Log.Info("Registering new device guid");
                DataObjects.Device device = new DataObjects.Device()
                {
                    Id = Guid.NewGuid(),
                    OperatingSystem = Xamarin.Forms.Device.RuntimePlatform,
                };
                RepoManager.DeviceRepository.Add(device);
                Settings.Current.Device = device.Id.ToString();
            }

            InitializeComponent();


            SetupLocationManager();

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

            MainPage = new TabiTabbedPage();
            if (!Settings.Current.PermissionsGranted)
            {
                MainPage.Navigation.PushModalAsync(new IntroPage());
            }
        }

        private void SetupLogging()
        {
            LogSeverity level = Log.SeverityFromString(Configuration["logging:level"]);
            MultiLogger mLogger = new MultiLogger();
            mLogger.SetLogLevel(level);

            mLogger.AddLogger(new ConsoleLogWriter());
            mLogger.AddLogger(new FileLogWriter());
            mLogger.AddLogger(new DbLogWriter());
            Log.SetLogger(mLogger);
            Log.Info("Logging Setup");
        }

        private void SetupSQLite()
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            var dbPath = PortablePath.Combine(rootFolder.Path, "tabi.db");
            RepoManager = new DataStorage.SqliteNet.SqliteNetRepoManager(dbPath);
        }

        private void SetupLocationManager()
        {
            locationManager = DependencyService.Get<ILocationManager>();
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
            sensorManager = new SensorManager();
            Settings.Current.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "Tracking")
                {
                    if (Settings.Current.Tracking && !locationManager.IsListening)
                    {
                        sensorManager.StartSensorUpdates();
                    }
                    else if (!Settings.Current.Tracking && locationManager.IsListening)
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
            SyncService.AutoUpload(TimeSpan.FromMinutes(10));
        }

        async Task CheckAuthorization(string deviceId)
        {
            if (Settings.Current.PermissionsGranted)
            {
                TabiApiClient.ApiClient apiClient = new TabiApiClient.ApiClient();
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
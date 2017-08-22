using System;
using System.Reflection;
using Xamarin.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Microsoft.Azure.Mobile.Distribute;
using PCLStorage;
using Tabi.Shared.Extensions;
using Tabi.DataStorage;
using Tabi.Logging;
using Tabi.Shared.Collection;

namespace Tabi
{
    public partial class App : Application
    {
        public const string LogFilePath = "tabi.log";
        public static double ScreenHeight;
        public static double ScreenWidth;
        public static readonly IConfigurationRoot Configuration;
        public static bool LocationPermissionsGranted;
        public static IRepoManager RepoManager;

        public static CollectionProfile CollectionProfile { get; private set; }

        ILocationManager manager;

        static App()
        {
            // Load configuration from embeddedresource file.
            Assembly assembly = typeof(App).GetTypeInfo().Assembly;
            var builder = new ConfigurationBuilder().AddEmbeddedXmlFile(assembly, "tabi.config");
            Configuration = builder.Build();
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


            string apiKey = Configuration["mobilecenter:apikey"];
            bool mobileCenterEnabled = Convert.ToBoolean(Configuration["mobilecenter:enabled"]);

            if (!apiKey.Equals(""))
            {
                MobileCenter.Start(apiKey,
                    typeof(Analytics), typeof(Crashes), typeof(Distribute));
                Log.Debug("MobileCenter started with apikey");
                MobileCenter.SetEnabledAsync(mobileCenterEnabled);
                Log.Debug($"MobileCenter enabled: {mobileCenterEnabled}");
            }


            MainPage = new TabiTabbedPage();
            if (!Settings.Current.PermissionsGranted)
            {
                MainPage.Navigation.PushModalAsync(new PermissionsPage());
            }
        }

        private void SetupLogging()
        {
            MultiLogger mLogger = new MultiLogger();
            mLogger.AddLogger(new ConsoleLogWriter());
            mLogger.AddLogger(new FileLogWriter());
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
            manager = DependencyService.Get<ILocationManager>();
            Settings.Current.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "Tracking")
                {
                    if (Settings.Current.Tracking && !manager.IsListening)
                    {
                        manager.StartLocationUpdates();
                    }
                    else if (!Settings.Current.Tracking && manager.IsListening)
                    {
                        manager.StopLocationUpdates();
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
                manager.StartLocationUpdates();
            }
        }

        protected override void OnStart()
        {
            Log.Info("App.OnStart");
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
        }
    }
}
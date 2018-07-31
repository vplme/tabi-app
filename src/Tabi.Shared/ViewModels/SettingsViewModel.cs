using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using CsvHelper;
using PCLStorage;
using Tabi.Core;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.Shared.Csv;
using Xamarin.Forms;
using Plugin.Connectivity;

using FileAccess = PCLStorage.FileAccess;
using Tabi.Shared.Resx;
using Acr.UserDialogs;
using Tabi.iOS.Helpers;
using Tabi.Shared;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Tabi.Helpers;
using Xamarin.Essentials;
using System.Text;
using Tabi.Shared.Pages;

namespace Tabi
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly IRepoManager _repoManager;
        private readonly INavigation _navigation;
        private readonly DataResolver _dataResolver;
        private readonly SyncService _syncService;
        private readonly TabiConfiguration _config;

        public SettingsViewModel(TabiConfiguration config, INavigation navigation, IRepoManager repoManager, SyncService syncService, DataResolver dataResolver)
        {
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
            _dataResolver = dataResolver ?? throw new ArgumentNullException(nameof(dataResolver));
            _syncService = syncService ?? throw new ArgumentNullException(nameof(syncService));


            ExportDatabaseCommand = new Command(async key =>
                {
                    Log.Info("Command: Exporting database");

                    IFolder rootFolder = PCLStorage.FileSystem.Current.LocalStorage;
                    var t = await rootFolder.GetFileAsync("tabi.db");
                    DependencyService.Get<IShareFile>().ShareFile(t.Path);
                });

            DropDatabaseCommand = new Command(async () =>
            {
                Log.Info("Command: Dropping database");

                var answer =
                    await Application.Current.MainPage.DisplayAlert("Confirm", "Drop database?", "Yes", "Cancel");
                if (answer)
                {
                    //SQLiteHelper.Instance.ClearPositions();
                }
            });

            InfoCommand = new Command((obj) =>
            {
                InfoCount++;
                if (InfoCount == 10 && App.Developer)
                {
                    Settings.Developer = true;
                    InfoCount = 0;
                }
            });

            ExportKMLCommand = new Command(async key =>

            {
                Log.Info("Command: Export KML");
                //IFolder rootFolder = FileSystem.Current.LocalStorage;
                //string fileName = "Tabi-Export.kml";
                //IFile file = await rootFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                ////Repository<Position> posRepo = new Repository<Position>(SQLiteHelper.Instance.Connection);


                //var result = await posRepo.Get(p => p.Accuracy < 100, x => x.Timestamp);
                //string kml = GeoUtil.GeoSerialize(result);
                //await file.WriteAllTextAsync(kml);
                //DependencyService.Get<IShareFile>().ShareFile(file.Path);
            });

            ExportCSVCommand = new Command(async key =>

            {
                Log.Info("Command: Export CSV");

                IFolder rootFolder = PCLStorage.FileSystem.Current.LocalStorage;

                string fileName = "Tabi-Export.txt";
                string path = PortablePath.Combine(rootFolder.Path, fileName);
                IFile file = await rootFolder.CreateFileAsync(path, CreationCollisionOption.ReplaceExisting);

                IPositionEntryRepository positionEntryRepository = _repoManager.PositionEntryRepository;
                var result = positionEntryRepository.FilterAccuracy(100).ToList();

                Stream stream = await file.OpenAsync(FileAccess.ReadAndWrite);
                GeoUtil.PositionsToCsv(result, stream);

                DependencyService.Get<IShareFile>().ShareFile(path, "text/csv");
            });

            ExportBatteryCSVCommand = new Command(async key =>

            {
                Log.Info("Command: Export Battery CSV");

                IFolder rootFolder = PCLStorage.FileSystem.Current.LocalStorage;

                string fileName = "Tabi-Export-Battery.txt";
                string path = PortablePath.Combine(rootFolder.Path, fileName);
                IFile file = await rootFolder.CreateFileAsync(path, CreationCollisionOption.ReplaceExisting);

                var batteryEntryRepo = _repoManager.BatteryEntryRepository;
                var result = batteryEntryRepo.GetAll().ToList();

                Stream stream = await file.OpenAsync(FileAccess.ReadAndWrite);

                using (TextWriter tw = new StreamWriter(stream))
                {
                    var csv = new CsvWriter(tw);
                    csv.Configuration.RegisterClassMap<BatteryEntryMap>();
                    csv.WriteRecords(result);
                }

                DependencyService.Get<IShareFile>().ShareFile(path, "text/csv");
            });



            ClearStopsCommand = new Command((obj) =>
            {
                _repoManager.StopVisitRepository.ClearAll();
                _repoManager.TrackEntryRepository.ClearAll();
            });

            OpenLogsCommand = new Command((obj) =>
            {
                LogsPage page = new LogsPage();
                _navigation.PushAsync(page);
            });

            LoadSampleCommand = new Command(async () =>
            {
                DummyDbLoader dummy = new DummyDbLoader();
                await dummy.LoadAsync();
            });

            ShowMockupCommand = new Command(() =>
            {
                var assembly = typeof(SettingsViewModel).GetTypeInfo().Assembly;

                using (Stream stream = assembly.GetManifestResourceStream("Tabi.DemoCsv"))
                {
                    List<PositionEntry> entries = GeoUtil.CsvToPositions(stream).ToList();
                    _dataResolver.ResolveData(DateTimeOffset.MinValue, DateTimeOffset.Now);


                    //var x = sv.GroupPositions(entries, 100);
                    //var z = sv.DetermineStopVisits(x, null);

                    //Log.Debug(z.ToString());
                }
                //                ActivityOverviewMockupPage sPage = new ActivityOverviewMockupPage();
                //                navigationPage.PushAsync(sPage);
            });
            ShowPageCommand = new Command(() =>
            {
                TourVideoPage sPage = new TourVideoPage();
                _navigation.PushModalAsync(sPage);
            });

            ShowTourCommand = new Command(async () =>
            {
                Page tPage = new TourVideoPage();
                Analytics.TrackEvent("ShowTour clicked");

                await _navigation.PushModalAsync(tPage);
            });

            PrivacyDataCommand = new Command(async () =>
            {


                Analytics.TrackEvent("Privacy & data settings clicked");

                await _navigation.PushAsync(new SettingsPrivacyPage(this));
            });

            AppAboutCommand = new Command(async () =>
            {
                Analytics.TrackEvent("Privacy & data settings clicked");

                await _navigation.PushAsync(new AboutSettings(this));
            });

            SendSupportCallCommand = new Command(async () =>
            {
                try
                {
                    PhoneDialer.Open(_config.Support.PhoneNumber);
                }
                catch (ArgumentNullException ex)
                {
                    await UserDialogs.Instance.AlertAsync(AppResources.ErrorOccurredTitle, okText: AppResources.OkText);
                    Log.Error(ex);
                }
                catch (FeatureNotSupportedException ex)
                {
                    await UserDialogs.Instance.AlertAsync(AppResources.DeviceUnsupportedText, AppResources.DeviceUnsupportedTitle, AppResources.OkText);
                    Log.Error(ex);
                }
                catch (Exception ex)
                {
                    await UserDialogs.Instance.AlertAsync(AppResources.ErrorOccurredTitle, AppResources.OkText);
                    Log.Error(ex);
                }
            });

            SendSupportEmailCommand = new Command(async () =>
            {
                string subject = _config.Support.EmailSubject ?? "Tabi app";

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(" ");
                stringBuilder.AppendLine("==========================");
                stringBuilder.AppendLine("Tabi Verplaatsingen app");
                stringBuilder.AppendLine($"Version: {VersionTracking.CurrentVersion} ({VersionTracking.CurrentBuild})");
                stringBuilder.AppendLine($"Device ID: {Settings.Device}");

                List<string> to = new List<string>() { _config.Support.Email };

                try
                {
                    var message = new EmailMessage
                    {
                        Subject = subject,
                        Body = stringBuilder.ToString(),
                        To = to,
                    };

                    await Email.ComposeAsync(message);
                }
                catch (FeatureNotSupportedException ex)
                {
                    await UserDialogs.Instance.AlertAsync(AppResources.DeviceUnsupportedText, AppResources.DeviceUnsupportedTitle, AppResources.OkText);
                    Log.Error(ex);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            });

            OpenSupportWebsiteCommand = new Command(async () =>
            {
                await Browser.OpenAsync(_config.Support.Url, BrowserLaunchType.SystemPreferred);
            });

            LicensesCommand = new Command(async () =>
            {
                await Browser.OpenAsync(_config.LicensesUrl, BrowserLaunchType.SystemPreferred);
            });

            UploadCommand = new Command(async () =>
            {
                Analytics.TrackEvent("Upload clicked");

                // Check if there is an active internet connection
                if (CrossConnectivity.Current.IsConnected)
                {
                    bool wifiAvailable = CrossConnectivity.Current.ConnectionTypes.Contains(Plugin.Connectivity.Abstractions.ConnectionType.WiFi);
                    bool upload = !Settings.WifiOnly || wifiAvailable;

                    // Check if connected to WiFI
                    if (!upload)
                    {
                        upload = await UserDialogs.Instance.ConfirmAsync(AppResources.MobileDataUsageText, AppResources.MobileDataUsageTitle, AppResources.ContinueButton, AppResources.CancelText);
                    }

                    if (upload)
                    {
                        UserDialogs.Instance.ShowLoading(AppResources.UploadDataInProgress, MaskType.Black);

                        // Check if the API is available (within 5 seconds).
                        // Display message to user if api is unavailable. 
                        bool available = await _syncService.Ping(5);

                        if (!available)
                        {
                            UserDialogs.Instance.HideLoading();

                            await UserDialogs.Instance.AlertAsync(AppResources.APIUnavailableText, AppResources.APIUnavailableTitle, AppResources.OkText);
                            return;
                        }

                        try
                        {
                            await _syncService.UploadAll(false);

                            UserDialogs.Instance.HideLoading();

                            Settings.LastUpload = DateTime.Now.Ticks;

                            UserDialogs.Instance.Toast(AppResources.DataUploadSuccesful);
                        }
                        catch (Exception e)
                        {
                            UserDialogs.Instance.HideLoading();
                            await UserDialogs.Instance.AlertAsync(AppResources.UploadDataErrorText, AppResources.ErrorOccurredTitle, AppResources.OkText);
                            Log.Error($"UploadAll exception {e.Message}: {e.StackTrace}");
                        }
                    }
                }
                else
                {
                    UserDialogs.Instance.HideLoading();
                    // Show user a message that there is no internet connection
                    await UserDialogs.Instance.AlertAsync(AppResources.NoInternetConnectionText, AppResources.NoInternetConnectionTitle, AppResources.OkText);
                }
            });

            Settings.PropertyChanged += Settings_PropertyChanged;
        }

        public ICommand ExportDatabaseCommand { protected set; get; }

        public ICommand DropDatabaseCommand { protected set; get; }

        public ICommand ExportKMLCommand { protected set; get; }

        public ICommand ExportCSVCommand { protected set; get; }

        public ICommand ExportBatteryCSVCommand { protected set; get; }

        public ICommand GenerateStopsCommand { protected set; get; }

        public ICommand InfoCommand { protected set; get; }

        public ICommand ClearStopsCommand { protected set; get; }

        public ICommand OpenLogsCommand { protected set; get; }

        public ICommand LoadSampleCommand { protected set; get; }

        public ICommand ShowMockupCommand { protected set; get; }

        public ICommand ShowPageCommand { protected set; get; }

        public ICommand UploadCommand { protected set; get; }

        public ICommand ShowTourCommand { protected set; get; }

        public ICommand PrivacyDataCommand { protected set; get; }

        public ICommand AppAboutCommand { protected set; get; }

        public ICommand SendSupportEmailCommand { protected set; get; }

        public ICommand SendSupportCallCommand { protected set; get; }

        public ICommand OpenSupportWebsiteCommand { protected set; get; }

        public ICommand LicensesCommand { protected set; get; }

        public TabiConfiguration Configuration { get => _config; }

        public bool ShowSensorMeasurements { get => _config.SensorMeasurements.UserAdjustable; }

        public bool ShowAnalyticsOption { get => _config.MobileCenter.DisableAnalyticsOption; }

        public bool ShowCrashesOption { get => _config.MobileCenter.DisableCrashesOption; }

        public int InfoCount { get; set; }

        public string ApiUrl { get => _config.Api.Url; }

        public string LastSynced { get => TimeAgo(new DateTime(Settings.LastUpload)); }

        public string VersionText { get => DependencyService.Get<IVersion>().GetVersion(); }

        void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Console.WriteLine("Call propert!");
            if (e.PropertyName == "LastUpload")
            {
                OnPropertyChanged(nameof(LastSynced));
            }
            else if (e.PropertyName == "Tracking")
            {
                Analytics.TrackEvent("Tracking toggled", new Dictionary<string, string> { { "Value", Settings.Tracking.ToString() } });
            }
        }

        public bool WifiOnly
        {
            get => Settings.WifiOnly;
            set
            {
                if (!value)
                {
                    UserDialogs.Instance.ConfirmAsync(AppResources.MobileDataUsageText, AppResources.MobileDataUsageTitle, AppResources.ContinueButton, AppResources.CancelText).ContinueWith((arg) =>
                    {
                        if (arg.Result)
                        {
                            Settings.WifiOnly = false;
                        }
                        else
                        {
                            // Refresh Binding
                            OnPropertyChanged();
                        }
                    });
                }
                else
                {
                    Settings.WifiOnly = true;
                }
            }
        }

        private string TimeAgo(DateTime date)
        {
            DateTime now = DateTime.Now;

            TimeSpan timeAgo = now.Subtract(date);

            string result = "Unknown";
            if (timeAgo.TotalMinutes <= 1)
            {
                result = AppResources.TimeAgoLessThanAMinute;
            }
            else if (timeAgo.TotalMinutes <= 59)
            {
                result = $"{(int)timeAgo.TotalMinutes} {AppResources.TimeAgoMinutes}";
            }
            else if (timeAgo.TotalHours <= 8)
            {
                return $"{(int)timeAgo.TotalHours} {AppResources.TimeAgoHours}";
            }
            else if (date == DateTime.MinValue)
            {
                result = AppResources.TimeAgoNever;
            }
            else
            {
                result = date.ToString("dd-MM-yy hh:mm:ss");
            }

            return result;
        }


    }
}
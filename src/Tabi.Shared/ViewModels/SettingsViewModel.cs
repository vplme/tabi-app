using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Input;
using CsvHelper;
using PCLStorage;
using Tabi.Core;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.iOS.Helpers;
using Tabi.Model;
using Tabi.Shared.Csv;
using Xamarin.Forms;
using FileAccess = PCLStorage.FileAccess;

namespace Tabi
{
    public class SettingsViewModel : BaseViewModel
    {
        private INavigation navigationPage;

        public SettingsViewModel(INavigation navigationPage) : this()
        {
            this.navigationPage = navigationPage;
        }

        public SettingsViewModel()
        {
            ExportDatabaseCommand = new Command(async key =>
            {
                Log.Info("Command: Exporting database");

                IFolder rootFolder = FileSystem.Current.LocalStorage;
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
                if (InfoCount == 10)
                {
                    Settings.Developer = true;
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

                IFolder rootFolder = FileSystem.Current.LocalStorage;

                string fileName = "Tabi-Export.txt";
                string path = PortablePath.Combine(rootFolder.Path, fileName);
                IFile file = await rootFolder.CreateFileAsync(path, CreationCollisionOption.ReplaceExisting);

                IPositionEntryRepository positionEntryRepository = App.RepoManager.PositionEntryRepository;
                var result = positionEntryRepository.FilterAccuracy(100).ToList();

                Stream stream = await file.OpenAsync(FileAccess.ReadAndWrite);
                GeoUtil.PositionsToCsv(result, stream);

                DependencyService.Get<IShareFile>().ShareFile(path, "text/csv");
            });

            ExportBatteryCSVCommand = new Command(async key =>

            {
                Log.Info("Command: Export Battery CSV");

                IFolder rootFolder = FileSystem.Current.LocalStorage;

                string fileName = "Tabi-Export-Battery.txt";
                string path = PortablePath.Combine(rootFolder.Path, fileName);
                IFile file = await rootFolder.CreateFileAsync(path, CreationCollisionOption.ReplaceExisting);

                var batteryEntryRepo = App.RepoManager.BatteryEntryRepository;
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
                App.RepoManager.StopVisitRepository.ClearAll();
                App.RepoManager.TrackEntryRepository.ClearAll();
            });

            OpenLogsCommand = new Command((obj) =>
            {
                LogsPage page = new LogsPage();
                navigationPage.PushAsync(page);
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
                    DataResolver sv = new DataResolver();
                    sv.ResolveData(DateTimeOffset.MinValue, DateTimeOffset.Now);


                    //var x = sv.GroupPositions(entries, 100);
                    //var z = sv.DetermineStopVisits(x, null);

                    //Log.Debug(z.ToString());
                }
//                ActivityOverviewMockupPage sPage = new ActivityOverviewMockupPage();
//                navigationPage.PushAsync(sPage);
            });
            ShowPageCommand = new Command(() =>
            {
                PermissionsPage sPage = new PermissionsPage();
                navigationPage.PushModalAsync(sPage);
            });

            UploadCommand = new Command(async () =>
            {
                SyncService sc = new SyncService();
                await sc.UploadAll(false);
            });
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


        public int InfoCount { get; set; }

        public bool Tracking
        {
            get { return Settings.Current.Tracking; }
            set { Settings.Current.Tracking = value; }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Threading.Tasks;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.Pages;
using Xamarin.Forms;
using System.Threading;
using Microsoft.AppCenter.Analytics;
using Tabi.Helpers;
using Tabi.Logging;
using Tabi.Model;
using Tabi.Resx;
using Stop = Tabi.DataObjects.Stop;
using StopVisit = Tabi.DataObjects.StopVisit;

namespace Tabi.ViewModels
{
    public class ActivityOverviewViewModel : BaseViewModel
    {
        public ObservableCollection<ActivityEntry> ActivityEntries { get; } = new ObservableCollection<ActivityEntry>();

        private readonly MotiveConfiguration _motiveConfiguration;
        private readonly INavigation _navigation;
        private readonly DataResolver _dataResolver;
        private readonly DateService _dateService;
        private readonly IRepoManager _repoManager;

        private readonly static SemaphoreSlim semaphore;

        static ActivityOverviewViewModel()
        {
            semaphore = new SemaphoreSlim(1);
        }

        private bool listIsRefreshing;
        public bool ListIsRefreshing
        {
            get
            {
                return listIsRefreshing;
            }
            set
            {
                SetProperty(ref listIsRefreshing, value);
            }
        }

        private bool noDataInOverviewVisible;

        public bool NoDataInOverviewVisible
        {
            get
            {
                return noDataInOverviewVisible;
            }
            set
            {
                SetProperty(ref noDataInOverviewVisible, value);
            }
        }

        public ICommand SettingsCommand { protected set; get; }

        public ICommand DaySelectorCommand { protected set; get; }

        public ICommand RefreshCommand { protected set; get; }

        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;

            set => SetProperty(ref isBusy, value);
        }

        private bool refreshEnabled = true;

        public bool RefreshEnabled
        {
            get => refreshEnabled;

            set => SetProperty(ref refreshEnabled, value);
        }

        private string title;
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                SetProperty(ref title, value);
            }
        }

        public ActivityOverviewViewModel(MotiveConfiguration motiveConfiguration, INavigation navigation, DateService dateService, IRepoManager repoManager, DataResolver dataResolver)
        {
            _motiveConfiguration = motiveConfiguration ?? throw new ArgumentNullException(nameof(motiveConfiguration));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            _dataResolver = dataResolver ?? throw new ArgumentNullException(nameof(dataResolver));
            _dateService = dateService ?? throw new ArgumentNullException(nameof(dateService));
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));

            SettingsCommand = new Command(async () =>
            {
                await _navigation.PushAsync(new SettingsPage());
                Analytics.TrackEvent("Settings clicked");
            });

            DaySelectorCommand = new Command(async () =>
            {
                await _navigation.PushModalAsync(new NavigationPage(new DaySelectorPage()));
                Analytics.TrackEvent("DaySelector clicked");

            });

            RefreshCommand = new Command(async () =>
            {
                ListIsRefreshing = false;
                IsBusy = true;
                refreshEnabled = false;
                Task uiTask = Task.Delay(1200);
                await UpdateStopVisitsAsync();
                // Show the UI for at least a second..
                await uiTask;

                IsBusy = false;
                refreshEnabled = true;

                Analytics.TrackEvent("Refresh pulled");
            });

            SetDataFromDateService();

            _dateService.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "SelectedDay")
                {
                    SetDataFromDateService();
                }
            };
        }

        private void SetDataFromDateService()
        {
            Title = _dateService.SelectedDay.CurrentDateShort;
        }

        public DateTime SelectedDate
        {
            get
            {
                return _dateService.SelectedDay.Time;
            }
        }


        public async Task UpdateStopVisitsAsync()
        {
            // Don't run this more than once at the same time.
            // Could result in duplicate stops being saved.
            await semaphore.WaitAsync();

            await Task.Run(() => _dataResolver.ResolveData(DateTimeOffset.MinValue, DateTimeOffset.Now));

            //semaphore.Release();

            List<ActivityEntry> newActivityEntries = new List<ActivityEntry>();

            DateTimeOffset startDate = _dateService.SelectedDay.Time.Date;
            DateTimeOffset endDate = _dateService.SelectedDay.Time.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            var stopVisits = _repoManager.StopVisitRepository.BetweenDates(startDate, endDate);
            Dictionary<int, Stop> stopDictionary = new Dictionary<int, Stop>();
            foreach (StopVisit sv in stopVisits)
            {
                ActivityEntry ae = new ActivityEntry();

                if (stopDictionary.ContainsKey(sv.StopId))
                {
                    sv.Stop = stopDictionary[sv.StopId];
                }
                else
                {
                    sv.Stop = _repoManager.StopRepository.Get(sv.StopId);
                    stopDictionary.Add(sv.StopId, sv.Stop);
                }

                ae.Completed = sv.Stop.Name != null && _repoManager.MotiveRepository.GetByStopVisitId(sv.Id) != null;

                DateTimeOffset beginTimestampLocal = sv.BeginTimestamp.ToLocalTime();
                DateTimeOffset endTimestampLocal = sv.EndTimestamp.ToLocalTime();
                DateTimeOffset startDateLocal = startDate.ToLocalTime();

                bool stopEndsNextDay = startDateLocal.Day < endTimestampLocal.Day;
                bool stopBeginsPreviousDay = startDateLocal.Day > beginTimestampLocal.Day;

                if (stopEndsNextDay)
                {
                    ae.Time = $"{beginTimestampLocal:HH:mm} - {endTimestampLocal:HH:mm} ({AppResources.NextDay})";
                }
                else if (stopBeginsPreviousDay)
                {

                    ae.Time = $"{beginTimestampLocal:HH:mm} ({AppResources.PreviousDay}) - {endTimestampLocal:HH:mm}";
                }
                else
                {
                    ae.Time = $"{beginTimestampLocal:HH:mm} - {endTimestampLocal:HH:mm}";

                }

                ae.StopVisit = sv;
                newActivityEntries.Add(ae);

                if (sv.NextTrackId != 0 && !stopEndsNextDay)
                {
                    TrackEntry te = _repoManager.TrackEntryRepository.Get(sv.NextTrackId);
                    try
                    {
                        double minutes = te.TimeTravelled.TotalMinutes < 200 ? te.TimeTravelled.TotalMinutes : 200;

                        ActivityEntry tAe = new ActivityEntry()
                        {
                            Track = new Track()
                            {
                                TrackEntry = te,
                                Height = minutes,
                                Color = (Color)Application.Current.Resources["TintColor"],
                                Text = $"{Math.Round(te.DistanceTravelled / 1000, 1)} km",
                            },
                        };

                        bool transportsSelected = _repoManager.TransportationModeRepository.GetLastWithTrackEntry(te.Id) != null;
                        bool motiveFilledIn = !_motiveConfiguration.Tracks || _repoManager.MotiveRepository.GetByTrackId(te.Id) != null;
                        tAe.Completed = transportsSelected && motiveFilledIn;

                        newActivityEntries.Add(tAe);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }

            ActivityEntries.Clear();

            foreach (ActivityEntry e in newActivityEntries)
            {
                ActivityEntries.Add(e);
            }

            NoDataInOverviewVisible = (ActivityEntries.Count == 0);

            semaphore.Release();
        }

        public async Task OnAppearing()
        {
            if (Settings.ShowTour)
            {
                Settings.ShowTour = false;
                Page tPage = new TourVideoPage();
                await _navigation.PushModalAsync(tPage);
            }

            SetDataFromDateService();
        }
    }
}

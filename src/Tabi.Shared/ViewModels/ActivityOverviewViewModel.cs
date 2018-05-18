using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Threading.Tasks;
using MvvmHelpers;
using Tabi.Core;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.Pages;
using Tabi.Shared.Helpers;
using Xamarin.Forms;

namespace Tabi.ViewModels
{
    public class ActivityOverviewViewModel : ObservableObject
    {
        public ObservableCollection<ActivityEntry> ActivityEntries { get; } = new ObservableCollection<ActivityEntry>();

        private readonly DataResolver _dataResolver;
        private readonly DateService _dateService;
        private readonly IRepoManager _repoManager;

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

        public INavigation Navigation { get; set; }

        public ActivityOverviewViewModel(DateService dateService, IRepoManager repoManager, DataResolver dataResolver)
        {
            _dataResolver = dataResolver ?? throw new ArgumentNullException(nameof(dataResolver));
            _dateService = dateService ?? throw new ArgumentNullException(nameof(dateService));
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));

            SettingsCommand = new Command(async () =>
            {
                await Navigation.PushAsync(new SettingsPage());
            });

            DaySelectorCommand = new Command(async () =>
            {
                await Navigation.PushAsync(new DaySelectorPage());
            });

            RefreshCommand = new Command(async () =>
            {
                ListIsRefreshing = false;
                IsBusy = true;
                refreshEnabled = false;
                Task uiTask = Task.Delay(2000);
                await UpdateStopVisitsAsync();
                // Show the UI for at least a second..
                await uiTask;

                IsBusy = false;
                refreshEnabled = true;
            });

        }

        public DateTime SelectedDate
        {
            get
            {
                return _dateService.SelectedDate.Date;
            }
            set
            {
                _dateService.SelectedDate = value;
            }
        }


        public async System.Threading.Tasks.Task UpdateStopVisitsAsync()
        {
            await _dataResolver.ResolveDataAsync(DateTimeOffset.MinValue, DateTimeOffset.Now);

            List<ActivityEntry> newActivityEntries = new List<ActivityEntry>();

            DateTimeOffset startDate = _dateService.SelectedDate.Date;
            DateTimeOffset endDate = _dateService.SelectedDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);


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
                sv.Stop.Name = string.IsNullOrEmpty(sv.Stop.Name) ? "Stop" : sv.Stop.Name;
                ae.Time = $"{sv.BeginTimestamp.ToLocalTime():HH:mm} - {sv.EndTimestamp.ToLocalTime():HH:mm}";
                ae.StopVisit = sv;
                newActivityEntries.Add(ae);

                if (sv.NextTrackId != 0)
                {
                    TrackEntry te = _repoManager.TrackEntryRepository.Get(sv.NextTrackId);

                    double minutes = te.TimeTravelled.TotalMinutes < 200 ? te.TimeTravelled.TotalMinutes : 200;

                    ActivityEntry tAe = new ActivityEntry()
                    {
                        Track = new Track()
                        {
                            TrackEntry = te,
                            Height = minutes,
                            Color = Color.Blue,
                            //Text = $"{te.StartTime} {te.EndTime} {te.DistanceTravelled}",
                        },
                    };
                    newActivityEntries.Add(tAe);
                }
            }

            ActivityEntries.Clear();

            foreach (ActivityEntry e in newActivityEntries)
            {
                ActivityEntries.Add(e);
            }

            NoDataInOverviewVisible = (ActivityEntries.Count == 0);

        }

    }
}

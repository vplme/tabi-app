using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MvvmHelpers;
using Tabi.Core;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Xamarin.Forms;

namespace Tabi.ViewModels
{
    public class ActivityOverviewViewModel : ObservableObject
    {
        public ObservableCollection<ActivityEntry> ActivityEntries { get; } = new ObservableCollection<ActivityEntry>();

        readonly INavigation navigationPage;

        IStopVisitRepository stopVisitRepository = App.RepoManager.StopVisitRepository;
        IStopRepository stopRepository = App.RepoManager.StopRepository;
        ITrackEntryRepository trackEntryRepository = App.RepoManager.TrackEntryRepository;


        public ICommand SettingsCommand { protected set; get; }

        public ICommand DaySelectorCommand { protected set; get; }


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

        public ActivityOverviewViewModel(INavigation navigationPage)
        {
            this.navigationPage = navigationPage;
            App.DateService.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "SelectedDate")
                {
                    UpdateStopVisits();
                }
            };
        }

        private DateTime selectedDate = App.DateService.SelectedDate.Date;

        public DateTime SelectedDate
        {
            get
            {
                return selectedDate;
            }
            set
            {
                selectedDate = value;
                App.DateService.SelectedDate = selectedDate;
            }
        }

        public void UpdateStopVisits()
        {
            DataResolver dateResolver = new DataResolver();
            dateResolver.ResolveData(DateTimeOffset.MinValue, DateTimeOffset.Now);

            List<ActivityEntry> newActivityEntries = new List<ActivityEntry>();

            DateTimeOffset startDate = App.DateService.SelectedDate.Date;
            DateTimeOffset endDate = App.DateService.SelectedDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);


            var stopVisits = stopVisitRepository.BetweenDates(startDate, endDate);
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
                    sv.Stop = stopRepository.Get(sv.StopId);
                    stopDictionary.Add(sv.StopId, sv.Stop);
                }
                sv.Stop.Name = string.IsNullOrEmpty(sv.Stop.Name) ? "Stop" : sv.Stop.Name;
                ae.Time = $"{sv.BeginTimestamp.ToLocalTime():HH:mm} - {sv.EndTimestamp.ToLocalTime():HH:mm}";
                ae.StopVisit = sv;
                newActivityEntries.Add(ae);

                if (sv.NextTrackId != 0)
                {
                    TrackEntry te = trackEntryRepository.Get(sv.NextTrackId);

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
        }

    }
}

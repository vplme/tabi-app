using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        INavigation navigationPage;

        IStopVisitRepository stopVisitRepository = App.RepoManager.StopVisitRepository;
        IStopRepository stopRepository = App.RepoManager.StopRepository;
        ITrackEntryRepository trackEntryRepository = App.RepoManager.TrackEntryRepository;


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
        }


        public void UpdateStopVisits()
        {
            DataResolver dateResolver = new DataResolver();
            dateResolver.ResolveData(DateTimeOffset.MinValue, DateTimeOffset.Now);

            List<ActivityEntry> newActivityEntries = new List<ActivityEntry>();

            var stopVisits = stopVisitRepository.AllSortedByTime();
            foreach (StopVisit sv in stopVisits)
            {
                ActivityEntry ae = new ActivityEntry();
                ae.StopCommand = new Command(() =>
                {
                    StopDetailPage page = new StopDetailPage(sv);
                    navigationPage.PushAsync(page);
                    page.Disappearing += (s, e) => {
                        ae.StopVisit = null; // Make ObservableObject refresh the collection
                        ae.StopVisit = sv;
                    };

                });

                sv.Stop = stopRepository.Get(sv.StopId);
                sv.Stop.Name = string.IsNullOrEmpty(sv.Stop.Name) ? "Stop" : sv.Stop.Name;
                ae.Time = $"{sv.Id} {sv.BeginTimestamp.ToLocalTime():HH:mm} - {sv.EndTimestamp.ToLocalTime():HH:mm}";
                ae.StopVisit = sv;
                newActivityEntries.Add(ae);

                if (sv.NextTrackId != 0)
                {
                    TrackEntry te = trackEntryRepository.Get(sv.NextTrackId);
                    ActivityEntry tAe = new ActivityEntry()
                    {
                        Track = new Track()
                        {
                            TrackEntry = te,
                            Height = (te.TimeTravelled.TotalMinutes + 1) * 5,
                            Color = Color.Blue,
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

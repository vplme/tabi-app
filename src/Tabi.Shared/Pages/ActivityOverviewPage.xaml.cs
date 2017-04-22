using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using MvvmHelpers;
using Tabi.DataObjects;
using Tabi.Logging;
using Tabi.Shared.Resx;
using Tabi.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tabi
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityOverviewPage : ContentPage
    {
        ActivityOverviewViewModel ViewModel => vm ?? (vm = BindingContext as ActivityOverviewViewModel);
        ActivityOverviewViewModel vm;

        RouteTracker routeTracker = new RouteTracker();
        StopResolver stopResolver = new StopResolver();

        bool first = true;

        public ActivityOverviewPage()
        {
            InitializeComponent();

            BindingContext = new ActivityOverviewViewModel();
            ViewModel.Title = AppResources.ActivityOverviewPageTitle;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (first)
            {
                UpdateStops();
                LoadData();
                first = false;
            }
        }

        // TODO Async fetch data

        void LoadData()
        {
            DateTime now = DateTime.Now;
            DateTimeOffset begin = new DateTime(now.Year, now.Month, now.Day, 00, 00, 00);
            DateTimeOffset end = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            //List<(StopVisit stop, List<PositionEntry> positions)> visits = routeTracker.StopsAndPositionsBetween(begin, end);

            List<Tuple<StopVisit, List<PositionEntry>>> visits = routeTracker.StopsAndPositionsBetween(begin, end);

            ViewModel.ActivityEntries.Clear();


            List<ActivityEntry> entries = new List<ActivityEntry>();
            foreach (var tuple in visits)
            {

                string name = "Stop";

                StopVisit visit = tuple.Item1;
                List<PositionEntry> positions = tuple.Item2;

                if (visit != null)
                {
                    ActivityEntry stopActivityEntry = new ActivityEntry();

                    if (visit.Stop.Name != null && !visit.Stop.Name.Equals(""))
                    {
                        name = visit.Stop.Name;
                    }

                    string timeString = string.Format("{0:HH:mm} - {1:HH:mm}", visit.BeginTimestamp.ToLocalTime(), visit.EndTimestamp.ToLocalTime());

                    stopActivityEntry.Name = name;
                    stopActivityEntry.Time = timeString;
                    stopActivityEntry.StopCommand = new Command(() =>
                    {
                        StopDetailPage page = new StopDetailPage(visit.Stop);
                        page.Disappearing += (sender, e) =>
                        {
                            if (page.StopName != null && !page.StopName.Equals(""))
                            {
                                stopActivityEntry.Name = page.StopName;
                            }
                        };
                        Navigation.PushAsync(page);

                    });

                    entries.Add(stopActivityEntry);

                    ActivityEntry trackActivityEntry = new ActivityEntry();

                    TimeSpan timespent = routeTracker.TimeInPositionsList(positions);
                    Track track = new Track() { Height = timespent.TotalMinutes * 0.5 };
                    Debug.WriteLine("TrackHeight: " + track.Height);
                    if (track.Height < 1)
                    {
                        track.Height = 2;
                    }
                    Debug.WriteLine("TrackHeightAfter: " + track.Height);
                    track.Color = Color.Red;

                    trackActivityEntry.Track = track;
                    entries.Add(trackActivityEntry);
                }
            }
            foreach (ActivityEntry entry in entries)
            {
                ViewModel.ActivityEntries.Add(entry);
            }
        }

        void UpdateStops()
        {
            stopResolver.GetStopsBetweenAsync(DateTimeOffset.MinValue, DateTimeOffset.Now);
        }

        void RefreshClicked(object sender, EventArgs arg)
        {
            UpdateStops();
            LoadData();
        }

    }
}

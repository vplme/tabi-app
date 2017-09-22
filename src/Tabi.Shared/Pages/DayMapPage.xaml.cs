using System;
using System.Collections.Generic;
using System.Linq;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.Logging;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Tabi
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DayMapPage : ContentPage
    {
        IStopRepository stopRepository = App.RepoManager.StopRepository;
        IStopVisitRepository stopVisitRepository = App.RepoManager.StopVisitRepository;
        IPositionEntryRepository positionEntryRepo = App.RepoManager.PositionEntryRepository;
        ITrackEntryRepository trackEntryRepository = App.RepoManager.TrackEntryRepository;

        public DayMapPage()
        {
            InitializeComponent();


        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ShowPositions();
            ShowMap();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            routeMap.ClearMap();

        }

        void ShowPositions()
        {

            IEnumerable<TrackEntry> trackEntries = trackEntryRepository.GetAll();

            List<PositionEntry> posits = new List<PositionEntry>();

            List<Line> wer = new List<Line>();

            List<StopVisit> visits = stopVisitRepository.BetweenDates(DateTimeOffset.MinValue, DateTimeOffset.Now).ToList();
            foreach (StopVisit sv in visits)
            {
                TrackEntry tEntry = trackEntryRepository.Get(sv.NextTrackId);
                if (tEntry != null)
                {
                    IEnumerable<PositionEntry> entries = positionEntryRepo.FilterPeriodAccuracy(tEntry.StartTime, tEntry.EndTime, 100);
                    Line line = new Line();
                    posits.AddRange(entries);
                    foreach(var entry in entries)
                    {
                        line.Positions.Add(new Position(entry.Latitude, entry.Longitude));
                    }
                    line.Color = Color.Blue;
                    wer.Add(line);
                 
                }
            }

            routeMap.Lines.Clear();
            routeMap.Lines = wer;

            PositionEntry avg = Util.AveragePosition(posits);
            routeMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(avg.Latitude, avg.Longitude), Distance.FromMiles(20.0)));

            routeMap.DrawRoute();
        }

        void DaySwitcher(object sender, EventArgs e)
        {

        }

        async void ShowMap()
        {
            routeMap.Pins.Clear();

            StopResolver resolver = new StopResolver();

            List<StopVisit> visits = stopVisitRepository.BetweenDates(DateTimeOffset.MinValue, DateTimeOffset.Now).ToList();

            foreach (StopVisit p in visits)
            {
                Stop st = stopRepository.Get(p.StopId);
                string labelPin = "Stop";

                if (st.Name != null)
                {
                    labelPin = st.Name;
                }

                var xP = new Position(st.Latitude, st.Longitude);
                Pin pin = new Pin() { Label = labelPin, Position = xP };
                pin.Clicked += (sender, e) =>
                {
                    StopDetailPage page = new StopDetailPage(p);
                    Navigation.PushAsync(page);
                };
                routeMap.Pins.Add(pin);
            }
        }
    }
}

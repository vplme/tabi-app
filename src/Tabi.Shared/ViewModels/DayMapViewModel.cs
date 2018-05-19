using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Tabi.Shared.ViewModels
{
    public class DayMapViewModel : BaseViewModel
    {
        private readonly IRepoManager _repoManager;
        private List<PositionEntry> positions = new List<PositionEntry>();

        public DayMapViewModel(IRepoManager repoManager)
        {
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
        }

        public INavigation Navigation { get; set; }

        public List<Line> GetLines()
        {
            ITrackEntryRepository trackEntriesRepo = _repoManager.TrackEntryRepository;
            IEnumerable<TrackEntry> trackEntries = trackEntriesRepo.GetAll();

            List<Line> resultingLines = new List<Line>();

            List<StopVisit> visits = _repoManager.StopVisitRepository.BetweenDates(DateTimeOffset.MinValue, DateTimeOffset.Now).ToList();
            foreach (StopVisit sv in visits)
            {
                TrackEntry tEntry = trackEntriesRepo.Get(sv.NextTrackId);
                if (tEntry != null)
                {
                    IEnumerable<PositionEntry> entries = _repoManager.PositionEntryRepository.FilterPeriodAccuracy(tEntry.StartTime, tEntry.EndTime, 100);
                    Line line = new Line();
                    foreach (var entry in entries)
                    {
                        line.Positions.Add(new Position(entry.Latitude, entry.Longitude));
                    }
                    line.Color = Color.Blue;
                    resultingLines.Add(line);
                }
            }
            return resultingLines;
        }

        public async Task<List<Line>> GetLinesAsync()
        {
            return await Task.FromResult<List<Line>>(GetLines());
        }

        public MapSpan GetMapSpan()
        {
            MapSpan mapSpan = MapSpan.FromCenterAndRadius(new Position(52.092876, 5.104480), Distance.FromKilometers(30));
            if (positions.Any())
            {
                PositionEntry avg = Util.AveragePosition(positions);
                System.Diagnostics.Debug.WriteLine($"AVG {avg.Latitude} {avg.Longitude}");
                MapSpan.FromCenterAndRadius(new Position(avg.Latitude, avg.Longitude), Distance.FromMiles(20.0));
            }

            return mapSpan;
        }

        public async Task<MapSpan> GetMapSpanAsync()
        {
            return await Task.FromResult<MapSpan>(GetMapSpan());
        }

        public List<Pin> GetPins()
        {
            List<Pin> resultingsPins = new List<Pin>();

            List<StopVisit> visits = _repoManager.StopVisitRepository.BetweenDates(DateTimeOffset.MinValue, DateTimeOffset.Now).ToList();

            foreach (StopVisit p in visits)
            {
                Stop st = _repoManager.StopRepository.Get(p.StopId);
                string labelPin = "Stop";

                if (st.Name != null)
                {
                    labelPin = st.Name;
                }

                var xP = new Position(st.Latitude, st.Longitude);
                Pin pin = new Pin() { Label = labelPin, Position = xP };
                pin.Clicked += async (object sender, EventArgs e) =>
                {
                    StopDetailPage page = new StopDetailPage(p);
                    await Navigation.PushAsync(page);
                };
            }
            return resultingsPins;
        }
    }
}

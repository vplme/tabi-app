using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MvvmHelpers;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.Pages;
using Tabi.Shared.Model;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Tabi.ViewModels
{
    public class TrackDetailViewModel : ObservableObject
    {
        private readonly IRepoManager _repoManager;

        public TrackDetailViewModel(IRepoManager repoManager)
        {
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
            TransportModeSelectionCommand = new Command(async () =>
            {
                await Navigation.PushAsync(new TransportSelectionPage(TrackEntry));
            });
        }

        public ICommand TransportModeSelectionCommand { get; protected set; }

        public INavigation Navigation { get; set; }

        public TrackEntry TrackEntry { get; set; }

        public MapSpan AveragePosition()
        {
            MapSpan result = MapSpan.FromCenterAndRadius(new Position(52.092876, 5.104480), Distance.FromKilometers(30));
            List<PositionEntry> positions = _repoManager.PositionEntryRepository.FilterPeriodAccuracy(TrackEntry.StartTime, TrackEntry.EndTime, 100);

            if (positions.Count() > 0)
            {
                PositionEntry avg = Util.AveragePosition(positions);
                System.Diagnostics.Debug.WriteLine($"AVG {avg.Latitude} {avg.Longitude}");
                result = MapSpan.FromCenterAndRadius(new Position(avg.Latitude, avg.Longitude), Distance.FromMeters(TrackEntry.DistanceTravelled));
            }

            return result;
        }

        public Line GetMapLine()
        {
            Line line = new Line();

            List<PositionEntry> positions = _repoManager.PositionEntryRepository.FilterPeriodAccuracy(TrackEntry.StartTime, TrackEntry.EndTime, 100);
            foreach (PositionEntry pe in positions)
            {
                line.Positions.Add(new Position(pe.Latitude, pe.Longitude));
            }

            return line;
        }
    }
}

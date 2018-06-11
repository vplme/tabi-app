using System;
using System.Collections.Generic;
using System.Linq;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.Logic;
using Tabi.Shared.DataSync;

namespace Tabi.Core
{
    public class DataResolver
    {
        IPositionEntryRepository _positionEntryRepository;
        ITrackEntryRepository _trackEntryRepository;

        IStopVisitRepository _stopVisitRepository;
        IStopRepository _stopRepository;
        private readonly IRepoManager _repoManager;
        private readonly IStopResolver _stopResolver;


        private double _accuracy;

        public DataResolver(IRepoManager repoManager, IStopResolver stopResolver)
        {
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
            _stopResolver = stopResolver ?? throw new ArgumentNullException(nameof(stopResolver));

            _positionEntryRepository = _repoManager.PositionEntryRepository;
            _trackEntryRepository = _repoManager.TrackEntryRepository;
            _stopVisitRepository = _repoManager.StopVisitRepository;
            _stopRepository = _repoManager.StopRepository;

            _accuracy = 10000;
        }

        public void ReplaceStopsWithExisting(StopVisit stopVisit, Action<StopVisit> action)
        {
            if (stopVisit != null)
            {
                if (stopVisit.NextTrack != null && stopVisit.NextTrack.NextStop != null)
                {
                    ReplaceStopsWithExisting(stopVisit.NextTrack.NextStop, action);
                }
                action.Invoke(stopVisit);
            }
        }

        // Resolve data for period including
        public void ResolveData(DateTimeOffset begin, DateTimeOffset end)
        {
            // Get latest StopVisit/Track from db
            StopVisit lastStopVisit = _stopVisitRepository.LastStopVisit();
            TrackEntry lastTrackEntry = _trackEntryRepository.LastTrackEntry();

            DateTimeOffset newBeginTime = lastStopVisit != null ? lastStopVisit.BeginTimestamp : begin;

            // Fetch Positions starting from beginning
            List<PositionEntry> fetchedPositions = _positionEntryRepository.FilterPeriodAccuracy(newBeginTime, end, _accuracy);

            IList<ResolvedStop> resolvedStops = _stopResolver.ResolveStops(fetchedPositions);

            var rStop = resolvedStops.FirstOrDefault();
            StopVisit first = null;
            if (rStop != null)
            {
                // Convert ResolvedStop to StopVisit (recursively)
                first = rStop.ToStopVisitAndStop();
            }

            // Merge last StopVisit/Track from db and new stopvisits/tracks
            if (first != null && first.BeginTimestamp == lastStopVisit?.BeginTimestamp)
            {
                lastStopVisit.EndTimestamp = first.EndTimestamp;
                lastStopVisit.StopAccuracy = first.StopAccuracy;
                lastStopVisit.Latitude = first.Latitude;
                lastStopVisit.Longitude = first.Longitude;

                if (first.NextTrack != null && first.NextTrack.TimeTravelled != TimeSpan.Zero)
                {
                    lastStopVisit.NextTrack = first.NextTrack;
                    _trackEntryRepository.Add(lastStopVisit.NextTrack);
                    lastStopVisit.NextTrackId = lastStopVisit.NextTrack.Id;
                }

                _stopVisitRepository.Update(lastStopVisit);
                first = first.NextTrack?.NextStop;
            }

            // Replace StopVisits name if there is ONE stop nearby.
            ReplaceStopsWithExisting(first, (s =>
            {
                if (s.Stop != null)
                {
                    IList<Stop> nearestStops = _stopRepository.NearestStops(s.Latitude, s.Longitude, 30).ToList();
                    if (nearestStops.Count == 1)
                    {
                        s.Stop = nearestStops.First();
                    }
                }
            }));


            // Store new StopVisits, Tracks
            if (first != null)
            {
                SaveStopVisitChain(first);
            }
        }

        public StopVisit SaveStopVisitChain(StopVisit sv)
        {
            if (sv.NextTrack != null && sv.NextTrack.StartTime != default(DateTimeOffset))
            {
                sv.NextTrack = SaveTrackEntryChain(sv.NextTrack);
                sv.NextTrackId = sv.NextTrack.Id;
            }

            if (sv.Stop.Id == 0)
            {
                _stopRepository.Add(sv.Stop);
            }
            sv.StopId = sv.Stop.Id;

            _stopVisitRepository.Add(sv);

            return sv;
        }

        public TrackEntry SaveTrackEntryChain(TrackEntry te)
        {
            if (te.NextStop != null)
            {
                te.NextStop = SaveStopVisitChain(te.NextStop);
                te.NextStopId = te.NextStop.Id;
            }

            _trackEntryRepository.Add(te);
            return te;
        }



        /// <summary>
        /// Save StopVisits to Repository
        /// </summary>
        /// <param name="stopVisits"></param>
        public void SaveStopVisits(IList<StopVisit> stopVisits)
        {
            foreach (StopVisit sv in stopVisits)
            {
                if (sv.StopId == 0)
                {
                    _stopRepository.Add(sv.Stop);
                    sv.StopId = sv.Stop.Id;
                }
                if (sv.NextTrackId == 0)
                {
                    _trackEntryRepository.Add(sv.NextTrack);
                    sv.NextTrackId = sv.NextTrack.Id;
                }

                _stopVisitRepository.Add(sv);
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Tabi.DataObjects;
using Tabi.DataStorage;

namespace Tabi
{
    public class RouteTracker
    {
        IPositionEntryRepository positionEntryRepository = App.RepoManager.PositionEntryRepository;
        IStopVisitRepository stopVisitRepository = App.RepoManager.StopVisitRepository;
        IStopRepository stopRepository = App.RepoManager.StopRepository;

        public RouteTracker()
        {
        }

        public List<StopVisit> StopsBetween(DateTimeOffset begin, DateTimeOffset end)
        {
            var visits = stopVisitRepository.BetweenDates(begin, end).ToList();
            foreach (StopVisit v in visits)
            {
                v.Stop = stopRepository.Get(v.StopId);
            }

            return visits;
        }

        

        public List<Tuple<StopVisit, List<PositionEntry>>> StopsAndPositionsBetween(DateTimeOffset begin, DateTimeOffset end)
        {
            List<StopVisit> visits = StopsBetween(begin, end);
            List<PositionEntry> allPositionEntries = positionEntryRepository.FilterPeriodAccuracy(begin, end, 100);
            List<Tuple<StopVisit, List<PositionEntry>>> visitsPositions = new List<Tuple<StopVisit, List<PositionEntry>>>();

            Tuple<StopVisit, List<PositionEntry>> currentTuple = new Tuple<StopVisit, List<PositionEntry>>(null, new List<PositionEntry>());
            bool currentTupleAdded = true;

            foreach (PositionEntry p in allPositionEntries)
            {
                StopVisit lastVisit = currentTuple.Item1;

                StopVisit currentVisit = PositionInStop(p, visits);

                if (currentVisit != null)
                {
                    Debug.WriteLine("currentvisit not null");
                    Debug.WriteLine("CurrentVisit: {0} lastvisit: {1}", currentVisit, lastVisit);

                    // New stopvisit
                    if (!currentVisit.Equals(lastVisit))
                    {
                        Debug.WriteLine("currentvisit not equal lastvisit");

                        Debug.WriteLine("Different visit items " + visits.Count);


                        // Store old visit and positions in list and create new tuple to store data in
                        if(currentTuple.Item2.Count > 0)
                        {
                            visitsPositions.Add(currentTuple);
                        }
                        currentTuple = new Tuple<StopVisit, List<PositionEntry>>(currentVisit, new List<PositionEntry>());
                        currentTupleAdded = false;
                    }
                    currentTuple.Item2.Add(p);
                    Debug.WriteLine("Added items" + currentTuple.Item2.Count);

                }
            }

            if(!currentTupleAdded)
            {
                visitsPositions.Add(currentTuple);
            }


            Debug.WriteLine("visitspositions " + visitsPositions.Count);

            return visitsPositions;
        }


        private StopVisit PositionInStop(PositionEntry pos, List<StopVisit> visits)
        {
            return visits.FirstOrDefault(sv => pos.Timestamp >= sv.BeginTimestamp && pos.Timestamp <= sv.EndTimestamp);
        }


        public TimeSpan TimeInPositionsList(List<PositionEntry> positions)
        {
            if(positions == null || positions.Count < 2)
            {
                return new TimeSpan();
            }
            List<PositionEntry> orderedPositions = positions.OrderBy(p => p.Timestamp).ToList();
            return TimeBetweenPositions(orderedPositions.First(), orderedPositions.Last());
        }

        public TimeSpan TimeBetweenPositions(PositionEntry begin, PositionEntry end)
        {
            return end.Timestamp - begin.Timestamp;
        }


    }
}

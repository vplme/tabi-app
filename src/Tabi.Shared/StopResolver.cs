using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.Shared;
using Xamarin.Forms;

namespace Tabi
{
    public class StopResolver
    {

        List<InternalStop> stopPosSets = new List<InternalStop>();
        List<Stop> visitedStops;

        PositionEntry lastPosition;

        IPositionEntryRepository positionEntryRepository = App.RepoManager.PositionEntryRepository;
        ITrackEntryRepository trackEntryRepository = App.RepoManager.TrackEntryRepository;

        IStopVisitRepository stopVisitRepository = App.RepoManager.StopVisitRepository;
        IStopRepository stopRepository = App.RepoManager.StopRepository;

        private DateTimeOffset LatestProcessedDate
        {
            get
            {
                if (Application.Current.Properties.ContainsKey("latestProcessedDate"))
                {
                    long ticks = (long)Application.Current.Properties["latestProcessedDate"];
                    return new DateTimeOffset(ticks, new TimeSpan(0));
                }
                return DateTimeOffset.MinValue;
            }
            set { Application.Current.Properties["latestProcessedDate"] = value.UtcTicks; }
        }

        public StopResolver()
        {
            Log.Debug("Latestproceseddate+ " + LatestProcessedDate);

            Log.Debug("Before visited" + DateTime.Now);
            visitedStops = stopRepository.GetAll().ToList();
            Log.Debug("after visited" + DateTime.Now + " " + visitedStops.Count);
        }

        public Task<List<StopVisit>> GetStopsBetweenAsync(DateTimeOffset begin, DateTimeOffset end)
        {
            return Task.Factory.StartNew(() => GetStopsBetween(begin, end));
        }

        public List<StopVisit> GetStopsBetween(DateTimeOffset begin, DateTimeOffset end)
        {
            // Check if all stops are in db, if not, request them;
            if (end > LatestProcessedDate)
            {
                LoadPositionsBetweenAsync(LatestProcessedDate, DateTimeOffset.Now);
                //LatestProcessedDate = DateTimeOffset.Now;
            }

            // Retrieve stops
            return stopVisitRepository.BetweenDates(begin, end).ToList();
        }

        public void LoadPositionsBetweenAsync(DateTimeOffset begin, DateTimeOffset end)
        {
            List<PositionEntry> positions = positionEntryRepository.FilterPeriodAccuracy(begin, end, 100);
            GenerateStops(positions);
            SaveStops();
        }


        public void GenerateStops(List<PositionEntry> positions)
        {
            Log.Debug("Positions in db:" + positions.Count);
            foreach (PositionEntry p in positions)
            {
                //Log.Debug("Process loc");
                lastPosition = p;
                bool addedToExistingStop = CheckPositionInExistingStops(p);
                if (!addedToExistingStop)
                {
                    Debug.WriteLine("NEW INTSTOP");
                    InternalStop s = new InternalStop();
                    s.AddPosition(p);

                    stopPosSets.Add(s);
                    ClearStopCandidates(false);
                }
            }
            ClearStopCandidates(true);
            stopPosSets = MergeStops(stopPosSets);
        }



        public void SaveStops()
        {
            Log.Debug("Save stops");

            StopVisit lastStopVisit = null;
            int count = 0;
            List<Stop> stops = new List<Stop>();
            foreach (InternalStop entry in stopPosSets)
            {
                InternalStop intStop = entry;
                count++;

                Debug.WriteLine("InternalStop Entry:" + count);

                bool stopAlreadyExists = false;

                Stop stop = new Stop();
                // Check if an (internal, temporary) stop matches a stop nearby.
                if (visitedStops != null)
                {
                    foreach (Stop v in visitedStops)
                    {
                        double distance = Util.DistanceBetween(v.Latitude, v.Longitude,
                            intStop.AveragePosition.Latitude, intStop.AveragePosition.Longitude);
                        if (distance < 100)
                        {
                            // replace stop with stop from db.
                            stop = v;
                            stopAlreadyExists = true;
                            break;
                        }
                    }
                }


                int existingStopVisitId = 0;
                int stopId;

                // Check if the new Visit has the same Stop as the last Visit in the database.
                if (stopAlreadyExists)
                {
                    StopVisit last = stopVisitRepository.LastStopVisit();

                    if (last?.StopId == stop.Id)
                    {
                        existingStopVisitId = last.Id;
                    }
                }
                else
                {
                    stop.Latitude = intStop.AveragePosition.Latitude;
                    stop.Longitude = intStop.AveragePosition.Longitude;

                    stopRepository.Add(stop);
                    stopId = stop.Id;

                    // Add stop to cache
                    visitedStops.Add(stop);
                }


                if (existingStopVisitId != 0)
                {
                    Debug.WriteLine("Extend stopvisit");

                    StopVisit v = stopVisitRepository.Get(existingStopVisitId);
                    v.EndTimestamp = intStop.End;

                    lastStopVisit = v;

                    // Update the StopVisit in database with most recent time.
                    stopVisitRepository.Update(v);
                }
                else
                {
                    Debug.WriteLine("new StopVisit");
                    lastStopVisit = new StopVisit()
                    {
                        BeginTimestamp = intStop.Start,
                        EndTimestamp = intStop.End,
                        StopId = stop.Id,
                    };

                    stopVisitRepository.Add(lastStopVisit);
                }
            }
            if (lastStopVisit != null)
            {
                LatestProcessedDate = lastStopVisit.EndTimestamp;
            }
        }


        private bool CheckPositionInExistingStops(PositionEntry p)
        {
            //Log.Debug("Existing stops: " + stopPosSets.Count);
            bool exists = false;

            foreach (InternalStop iStop in stopPosSets)
            {
                InternalStop aStop = iStop;

                // Add to internalstop if distance and position is not 
                bool withinTimeSpan = (p.Timestamp - iStop.End) < TimeSpan.FromMinutes(5);
                bool withinDistance = aStop.AveragePosition.DistanceTo(p) < 80;
                //Debug.WriteLine("Between CurrentTime and LastPosition timestamp {0} - Distance: {1}", (p.Timestamp - iStop.End), aStop.AveragePosition.DistanceTo(p));

                if (withinDistance && withinTimeSpan)
                {
                    //Debug.WriteLine("ADD TO EXISTING INTSTOP");
                    iStop.AddPosition(p);
                    exists = true;
                    break;
                }
            }
            return exists;
        }

        private void ClearStopCandidates(bool includingRecent = true)
        {
            List<InternalStop> toRemove = new List<InternalStop>();
            foreach (InternalStop iStop in stopPosSets)
            {
                TimeSpan between = iStop.End - iStop.Start;

                bool deleteRecent = includingRecent || lastPosition.Timestamp - iStop.End > TimeSpan.FromMinutes(10);
                bool notEnoughPositionsInTime = TimeSpan.FromMinutes(2) > between;
                //Debug.WriteLine("timespan  " + between);
                if (notEnoughPositionsInTime && deleteRecent)
                {
                    toRemove.Add(iStop);
                }
            }

            // Actually remove the items from the dictionary since we can't modify while iterating.
            foreach (InternalStop s in toRemove)
            {
                stopPosSets.Remove(s);
            }
        }

        public StopVisit PositionsToStopsAndTracks(IList<PositionEntry> positionEntries)
        {
            IList<IList<PositionEntry>> groupedPositions;


            return null;
        }

        private List<InternalStop> MergeStops(List<InternalStop> stops)
        {
            List<InternalStop> newStops = new List<InternalStop>();

            foreach (InternalStop stop in stops)
            {
                bool inExisting = false;

                foreach (InternalStop iSt in newStops)
                {
                    if (stop.AveragePosition.DistanceTo(iSt.AveragePosition) <= 150 &&
                        stop.Start - iSt.End < TimeSpan.FromMinutes(10))
                    {
                        List<PositionEntry> avgs = new List<PositionEntry>()
                        {
                            iSt.AveragePosition,
                            stop.AveragePosition
                        };
                        iSt.AveragePosition = Util.AveragePosition(avgs);
                        stop.AddPositionsRange(stop.Positions);
                        inExisting = true;
                        break;
                    }
                }
                if (!inExisting)
                {
                    newStops.Add(stop);
                }
            }

            return newStops;
        }


        class InternalStop
        {
            public int PositionsCount { get; set; }
            public PositionEntry AveragePosition { get; set; }

            public void SetNewAveragePosition(List<PositionEntry> pos)
            {
                int difference = pos.Count - PositionsCount;
                //Log.Debug("Difference" + difference);
                if (PositionsCount < 10 && difference != 0 || difference > 4)
                {
                    AveragePosition = Util.AveragePosition(pos);
                    PositionsCount = pos.Count;
                }
            }

            public List<PositionEntry> Positions { get; private set; } = new List<PositionEntry>();

            public void AddPosition(PositionEntry p)
            {
                Positions.Add(p);
                SetNewAveragePosition(Positions);
                Sort();
            }

            public void AddPositionsRange(List<PositionEntry> ps)
            {
                Positions.AddRange(ps);
                SetNewAveragePosition(Positions);
                Sort();
            }

            public void Sort()
            {
                Positions.Sort((x, y) => x.Timestamp.CompareTo(y.Timestamp));
            }

            public DateTimeOffset Start
            {
                get { return Positions.First().Timestamp; }
            }

            public DateTimeOffset End
            {
                get { return Positions.Last().Timestamp; }
            }

            public override string ToString()
            {
                return string.Format("[InternalStop: Positions={0}, AveragePosition={1}]", PositionsCount,
                    AveragePosition);
            }
        }
    }
}
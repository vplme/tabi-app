using System;
using System.Collections.Generic;
using System.Linq;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.Shared;

namespace Tabi.Core
{
    public class DataResolver
    {
        IPositionEntryRepository positionEntryRepository = App.RepoManager.PositionEntryRepository;
        ITrackEntryRepository trackEntryRepository = App.RepoManager.TrackEntryRepository;

        IStopVisitRepository stopVisitRepository = App.RepoManager.StopVisitRepository;
        IStopRepository stopRepository = App.RepoManager.StopRepository;

        private double _accuracy;
        private double _groupDistance;
        public DataResolver()
        {
            _accuracy = 100;
            _groupDistance = 50;
        }

        // Resolve data for period including
        public void ResolveData(DateTimeOffset begin, DateTimeOffset end)
        {
            // Get latest StopVisit/Track from db
            StopVisit lastStopVisit = stopVisitRepository.LastStopVisit();
            TrackEntry lastTrackEntry = trackEntryRepository.LastTrackEntry();

            DateTimeOffset newBeginTime = lastStopVisit != null ? lastStopVisit.BeginTimestamp : begin;

            // Fetch Positions starting from beginning
            List<PositionEntry> fetchedPositions = positionEntryRepository.FilterPeriodAccuracy(newBeginTime, end, _accuracy);
            // Group Positions
            IList<PositionGroup> positionGroups = GroupPositions(fetchedPositions, _groupDistance);

            // fetch existing stops
            IList<Stop> existingStops = stopRepository.GetAll().ToList();
            Log.Debug($"Existingstops size {existingStops.Count()}");

            // CreateStopVisits, Tracks
            (TrackEntry firstTrack, IList<StopVisit> stopVisits) = DetermineStopVisits(positionGroups, existingStops);

            // tracks will not be added unless there is a stopvisit, need fix....


            Log.Debug($"after Existingstops size {existingStops.Count()}");

            // Merge last StopVisit/Track from db and new stopvisits/tracks
            StopVisit first = stopVisits.FirstOrDefault();
            if (first != null && first.BeginTimestamp == lastStopVisit?.BeginTimestamp)
            {
                lastStopVisit.EndTimestamp = first.EndTimestamp;
                if (first.NextTrack.TimeTravelled != TimeSpan.Zero){
                    lastStopVisit.NextTrack = first.NextTrack;
                    trackEntryRepository.Add(lastStopVisit.NextTrack);
                    lastStopVisit.NextTrackId = lastStopVisit.NextTrack.Id;
                }

                stopVisitRepository.Update(lastStopVisit);
                stopVisits.Remove(first);
            }


            // Store new StopVisits, Tracks
            if (stopVisits.Count() > 0)
            {
                SaveStopVisitChain(stopVisits.First());

            }
        }



        /// <summary>
        /// Divide positions up into groups based on the distance between the positions
        /// </summary>
        /// <param name="positions">positions that will be grouped</param>
        /// <param name="groupDistance">the maximum distance of a position between another position in a group</param>
        /// <returns>List of grouped positions</returns>
        public IList<PositionGroup> GroupPositions(IList<PositionEntry> positions, double groupDistance)
        {
            IList<PositionGroup> groupedPositions = new List<PositionGroup>();
            PositionGroup lastGroup = null;

            foreach (PositionEntry pe in positions)
            {
                bool positionBelongsInGroup = false;
                if (lastGroup != null)
                {
                    positionBelongsInGroup = true;

                    // Check if the current positions is within the set distance of all positions in the group
                    foreach (var positionInGroup in lastGroup)
                    {
                        if (positionInGroup.DistanceTo(pe) > groupDistance)
                        {
                            positionBelongsInGroup = false;
                            break;
                        }
                    }
                }

                if (positionBelongsInGroup)
                {
                    // Add position to the last group
                    lastGroup.Add(pe);
                }
                else
                {
                    // Create a new group with the current position
                    PositionGroup newGroup = new PositionGroup();
                    newGroup.Add(pe);
                    groupedPositions.Add(newGroup);
                    lastGroup = newGroup;
                }
            }

            return groupedPositions;
        }


        /// <summary>
        /// Create StopVisits based on a list of grouped positions.
        /// </summary>
        /// <param name="groupedPositionEntries"></param>
        /// <returns></returns>
        public (TrackEntry firstTrack, IList<StopVisit> stopVisits) DetermineStopVisits(IList<PositionGroup> groupedPositionEntries,
            IList<Stop> existingStops)
        {
            IList<StopVisit> stopVisits = new List<StopVisit>();

            int stopVisitPositionGroup = -1;
            StopVisit previousStopVisit = null;

            TrackEntry previousTrack = new TrackEntry();
            TrackEntry firstLooseTrack = previousTrack;

            for (int i = 0; i < groupedPositionEntries.Count; i++)
            {
                PositionGroup grp = groupedPositionEntries[i];

                if (grp.TimeSpent > TimeSpan.FromMinutes(5))
                {

                    StopVisit sv = DetermineStopVisitFromGroup(grp, existingStops, 80);
                    // If new stopvisit was near the previous stopvisit AND distance travelled < d then
                    // update previous stopvisit.
                    if (previousStopVisit?.Stop != null &&
                        sv.Stop.Equals(previousStopVisit.Stop) &&
                        previousTrack?.DistanceTravelled < 200)

                    {
                        Log.Debug("Fix previousstop");
                        previousStopVisit.EndTimestamp = sv.EndTimestamp;
                        previousStopVisit.NextTrack = new TrackEntry();
                        previousTrack = previousStopVisit.NextTrack;
                    }

                    else
                    {
                        // TODO If new StopVisit:
                        if (previousTrack != null)
                        {
                            previousTrack.NextStop = sv;
                        }

                        sv.NextTrack = new TrackEntry();
                        previousTrack = sv.NextTrack;

                        stopVisits.Add(sv);

                        previousStopVisit = sv;
                        stopVisitPositionGroup = i;
                    }
                }
                else if (previousTrack != null)
                {
                    if (previousTrack.StartTime == default(DateTimeOffset))
                    {
                        PositionEntry first = grp.Positions.First();

                        previousTrack.StartTime = first.Timestamp;
                        previousTrack.FirstLatitude = first.Latitude;
                        previousTrack.FirstLongitude = first.Longitude;
                    }
                    PositionEntry last = grp.Positions.Last();


                    // TODO wrong 0 default val
                    double lat = previousTrack.LastLatitude > 0 ? previousTrack.LastLatitude : previousTrack.FirstLatitude;
                    double lon = previousTrack.LastLongitude > 0 ? previousTrack.LastLongitude : previousTrack.FirstLongitude;

                    previousTrack.DistanceTravelled += Util.DistanceBetween(lat,
                                                                            lon,
                                                                            last.Latitude, 
                                                                            last.Longitude);
                    previousTrack.EndTime = last.Timestamp;
                    previousTrack.LastLatitude = last.Latitude;
                    previousTrack.LastLongitude = last.Longitude;

                }

            }

            return (firstLooseTrack, stopVisits);
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
                stopRepository.Add(sv.Stop);
            }
            sv.StopId = sv.Stop.Id;

            stopVisitRepository.Add(sv);

            return sv;
        }

        public TrackEntry SaveTrackEntryChain(TrackEntry te)
        {
            if (te.NextStop != null)
            {
                te.NextStop = SaveStopVisitChain(te.NextStop);
                te.NextStopId = te.NextStop.Id;
            }

            trackEntryRepository.Add(te);
            return te;
        }

        public StopVisit DetermineStopVisitFromGroup(PositionGroup group, IList<Stop> existingStops, double stopDistance)
        {
            DateTimeOffset beginTimestamp = group.First().Timestamp;
            DateTimeOffset endTimestamp = group.Last().Timestamp;
            PositionEntry avg = Util.AveragePosition(group.Positions);
            (double maximumLatitude, double maximumLongitude) =
                AddMetersToPosition(avg.Latitude, avg.Longitude, stopDistance);
            (double minimumLatitude, double minimumLongitude) =
                AddMetersToPosition(avg.Latitude, avg.Longitude, -stopDistance);


            Stop stop = existingStops?.FirstOrDefault(s => (s.Latitude >= minimumLatitude &&
                                                           s.Latitude <= maximumLatitude &&
                                                           s.Longitude >= minimumLongitude &&
                                                           s.Longitude <= maximumLongitude));

            if (stop == null)
            {
                stop = new Stop
                {
                    Latitude = avg.Latitude,
                    Longitude = avg.Longitude
                };
                existingStops.Add(stop);
            }

            StopVisit sv = new StopVisit()
            {
                BeginTimestamp = beginTimestamp,
                EndTimestamp = endTimestamp,
                Stop = stop,
            };

            return sv;
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
                    stopRepository.Add(sv.Stop);
                    sv.StopId = sv.Stop.Id;
                }
                if (sv.NextTrackId == Guid.Empty)
                {
                    trackEntryRepository.Add(sv.NextTrack);
                    sv.NextTrackId = sv.NextTrack.Id;
                }

                stopVisitRepository.Add(sv);
            }
        }

        public (double latitude, double longitude) AddMetersToPosition(double latitude, double longitude,
            double distance)
        {
            const int radius = 6378137;
            double dLat = distance / radius;
            double dLon = distance / (radius * Math.Cos(Math.PI * latitude / 180));
            return (latitude + dLat * 180 / Math.PI, longitude + dLon * 180 / Math.PI);
        }
    }
}

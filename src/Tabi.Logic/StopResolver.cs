using System;
using System.Collections.Generic;
using System.Linq;
using Tabi.DataObjects;

namespace Tabi.Logic
{
    public class StopResolver : IStopResolver
    {
        private readonly IStopResolverConfiguration _configuration;

        public StopResolver(IStopResolverConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IList<ResolvedStop> ResolveStops(IList<PositionEntry> positions)
        {
            IList<PositionGroup> positionGroups = GroupPositions(positions);

            (ResolvedTrip firstTrack, IList<ResolvedStop> stopVisits) = DetermineStopVisits(positionGroups);

            return stopVisits;
        }

        public double DistanceBetweenPoints(PositionEntry position1, PositionEntry position2)
        {
            return position1.DistanceTo(position2);
        }

        bool PointsWithinAccuracy(PositionEntry position1, PositionEntry position2)
        {
            double distance = DistanceBetweenPoints(position1, position2);
            return distance < (position1.Accuracy + position2.Accuracy);
        }

        double DistanceWithoutAccuracy(PositionEntry position1, PositionEntry position2)
        {
            double distance = DistanceBetweenPoints(position1, position2);
            double accuraciesDistance = position1.Accuracy + position2.Accuracy;
            return distance - accuraciesDistance;
        }


        /// <summary>
        /// Divide positions up into groups based on the distance between the positions
        /// </summary>
        /// <param name="positions">positions that will be grouped</param>
        /// <returns>List of grouped positions</returns>
        public IList<PositionGroup> GroupPositions(IList<PositionEntry> positions)
        {
            int groupRadius = _configuration.GroupRadius;

            IEnumerable<PositionEntry> orderedPositions = positions.OrderBy(p => p.Timestamp);

            IList<PositionGroup> groupedPositions = new List<PositionGroup>();
            PositionGroup lastGroup = null;

            foreach (PositionEntry pe in orderedPositions)
            {
                bool positionBelongsInGroup = false;
                if (lastGroup != null)
                {
                    positionBelongsInGroup = true;

                    // Check if the current positions is within the set distance of all positions in the group
                    foreach (var positionInGroup in lastGroup)
                    {
                        double distance = DistanceWithoutAccuracy(positionInGroup, pe);
                        if (distance > groupRadius)
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

        bool CheckSameLocation(ResolvedStop s1, ResolvedStop s2, double radius)
        {
            return Util.DistanceBetween(s1.Latitude, s1.Longitude, s2.Latitude, s2.Longitude) < radius;
        }

        bool CheckSameLatLong(ResolvedStop s1, ResolvedStop s2)
        {
            double EPSILON = 0.000001;
            return Math.Abs(s1.Latitude - s2.Latitude) < EPSILON && Math.Abs(s1.Longitude - s2.Longitude) < EPSILON;
        }

        ResolvedStop ResolvedStopFromPositions(List<PositionEntry> positions)
        {
            PositionEntry avg = Util.AveragePosition(positions);

            (double average, double min, double max) = CalculateAccuracies(positions);

            ResolvedStop stop = new ResolvedStop()
            {
                Latitude = avg.Latitude,
                Longitude = avg.Longitude,
                BeginTimestamp = positions.First().Timestamp,
                EndTimestamp = positions.Last().Timestamp,
                AverageAccuracy = average,
                MinAccuracy = min,
                MaxAccuracy = max,
            };

            return stop;
        }


        (double average, double min, double max) CalculateAccuracies(List<PositionEntry> positions)
        {
            int count = 0;
            double average = 0;
            double max = 0;
            double min = int.MaxValue;

            foreach (PositionEntry position in positions)
            {
                if (position.Accuracy > max)
                    max = position.Accuracy;

                if (position.Accuracy < min)
                    min = position.Accuracy;

                average += position.Accuracy;

                count++;
            }

            return (average / count, min, max);
        }

        private void MergeStop(ref ResolvedStop previousResolvedStop, ResolvedStop sv)
        {
            List<PositionEntry> averagePos = new List<PositionEntry>();
            averagePos.Add(new PositionEntry()
            {
                Latitude = previousResolvedStop.Latitude,
                Longitude = previousResolvedStop.Longitude
            });
            averagePos.Add(new PositionEntry()
            {
                Latitude = sv.Latitude,
                Longitude = sv.Longitude
            });

            PositionEntry newAvg = Util.AveragePosition(averagePos);

            previousResolvedStop.Latitude = newAvg.Latitude;
            previousResolvedStop.Longitude = newAvg.Longitude;
            previousResolvedStop.EndTimestamp = sv.EndTimestamp;
            //previousResolvedStop.NextTrip = new ResolvedTrip();
        }

        /// <summary>
        /// Create StopVisits based on a list of grouped positions.
        /// </summary>
        /// <param name="groupedPositionEntries"></param>
        /// <returns></returns>
        public (ResolvedTrip firstTrack, IList<ResolvedStop> stopVisits) DetermineStopVisits(IList<PositionGroup> groupedPositionEntries)
        {
            IList<ResolvedStop> resolvedStops = new List<ResolvedStop>();

            ResolvedStop previousResolvedStop = null;

            ResolvedTrip previousResolvedTrip = null;
            ResolvedTrip firstTrip = null;

            foreach (PositionGroup grp in groupedPositionEntries)
            {
                TimeSpan time = TimeSpan.FromMinutes(_configuration.Time);
                // Create Stop
                if (grp.TimeSpent >= time && grp.MinAccuracy <= _configuration.MinStopAccuracy)
                {
                    ResolvedStop sv = ResolvedStopFromPositions(grp.Positions);

                    // This is not the first stop
                    bool mergePreviousStop = previousResolvedStop != null;

                    // Distance between the current stop and previous stop are lower than the stopMergeRadius 
                    mergePreviousStop = mergePreviousStop && CheckSameLocation(sv, previousResolvedStop, _configuration.StopMergeRadius);

                    // Travelled distance between previous stop and current stop is lower than the _stopMergeMaxTravelRadius
                    if (previousResolvedTrip != null)
                    {
                        mergePreviousStop = mergePreviousStop && previousResolvedTrip?.DistanceTravelled < _configuration.StopMergeMaxTravelRadius;
                    }

                    if (mergePreviousStop)
                    {
                        MergeStop(ref previousResolvedStop, sv);
                    }


                    else // Create new Stop
                    {
                        if (previousResolvedStop != null && previousResolvedStop.NextTrip == null)
                        {
                            // If no trip was added; add a trip manually between the stops.
                            previousResolvedTrip = previousResolvedStop.NextTrip = new ResolvedTrip()
                            {
                                StartTime = previousResolvedStop.EndTimestamp,
                                EndTime = sv.BeginTimestamp,
                                FirstLatitude = previousResolvedStop.Latitude,
                                FirstLongitude = previousResolvedStop.Longitude,
                                LastLatitude = sv.Latitude,
                                LastLongitude = sv.Longitude,
                                DistanceTravelled = Util.DistanceBetween(previousResolvedStop.Latitude, previousResolvedStop.Longitude, sv.Latitude, sv.Longitude),
                            };
                        }

                        if (previousResolvedTrip != null)
                        {
                            previousResolvedTrip.NextStop = sv;
                        }

                        resolvedStops.Add(sv);

                        previousResolvedStop = sv;
                    }
                }


                // Create Trip
                else
                {
                    ResolvedTrip trip;

                    // No first stop means we're adding to the firstTrip
                    if (previousResolvedStop == null)
                    {
                        if (firstTrip == null)
                        {
                            firstTrip = new ResolvedTrip();
                        }
                        trip = firstTrip;
                    }
                    else if (previousResolvedStop.NextTrip == null)
                    {
                        trip = previousResolvedStop.NextTrip = new ResolvedTrip();
                    }
                    else
                    {
                        trip = previousResolvedStop.NextTrip;
                    }


                    if (trip.StartTime == default(DateTimeOffset))
                    {
                        PositionEntry first = grp.Positions.First();

                        trip.StartTime = first.Timestamp;
                        trip.FirstLatitude = first.Latitude;
                        trip.FirstLongitude = first.Longitude;
                    }
                    PositionEntry last = grp.Positions.Last();

                    // TODO wrong 0 default val
                    double lat = trip.LastLatitude > 0 ? trip.LastLatitude : trip.FirstLatitude;
                    double lon = trip.LastLongitude > 0 ? trip.LastLongitude : trip.FirstLongitude;

                    trip.DistanceTravelled += Util.DistanceBetween(lat, lon, last.Latitude, last.Longitude);
                    trip.EndTime = last.Timestamp;
                    trip.LastLatitude = last.Latitude;
                    trip.LastLongitude = last.Longitude;

                    previousResolvedTrip = trip;
                }
            }

            return (firstTrip, resolvedStops);
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

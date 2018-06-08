using System;
using System.Collections.Generic;
using System.Linq;
using Tabi.DataObjects;

namespace Tabi.Logic
{
    public class StopResolver : IStopResolver
    {
        readonly TimeSpan _time;
        readonly double _groupRadius;
        readonly double _minStopAccuracy;
        readonly double _stopMergeRadius;
        readonly double _stopMergeMaxTravelRadius;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Tabi.Logic.StopResolver"/> class.
        /// </summary>
        /// <param name="time">Time in an area for it to be a stop.</param>
        /// <param name="groupRadius">Radius for a stop to be resolved.</param>
        /// <param name="stopMergeRadius">Maximum distance to other stop to allow merging</param>
        /// <param name="stopMergeMaxTravelRadius">Maximum distance previously travelled for stop to be merged</param>
        public StopResolver(TimeSpan time, double groupRadius, double minStopAccuracy, double stopMergeRadius, double stopMergeMaxTravelRadius)
        {
            _time = time;
            _groupRadius = groupRadius;
            _stopMergeRadius = stopMergeRadius;
            _minStopAccuracy = minStopAccuracy;
            _stopMergeMaxTravelRadius = stopMergeMaxTravelRadius;
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
            //Console.WriteLine($"Distance: {distance} Minacc {distance - accuraciesDistance}");
            return distance - accuraciesDistance;
        }


        /// <summary>
        /// Divide positions up into groups based on the distance between the positions
        /// </summary>
        /// <param name="positions">positions that will be grouped</param>
        /// <returns>List of grouped positions</returns>
        public IList<PositionGroup> GroupPositions(IList<PositionEntry> positions)
        {
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
                        if (distance > _groupRadius)
                        {
                            Console.WriteLine("Not in group!");
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


        /// <summary>
        /// Create StopVisits based on a list of grouped positions.
        /// </summary>
        /// <param name="groupedPositionEntries"></param>
        /// <returns></returns>
        public (ResolvedTrip firstTrack, IList<ResolvedStop> stopVisits) DetermineStopVisits(IList<PositionGroup> groupedPositionEntries)
        {
            IList<ResolvedStop> resolvedStops = new List<ResolvedStop>();

            ResolvedStop previousResolvedStop = null;

            ResolvedTrip previousResolvedTrip = new ResolvedTrip();
            ResolvedTrip firstLooseTrack = previousResolvedTrip;

            foreach (PositionGroup grp in groupedPositionEntries)
            {
                if (grp.TimeSpent >= _time && grp.MinAccuracy <= _minStopAccuracy)
                {
                    ResolvedStop sv = ResolvedStopFromPositions(grp.Positions);

                    // This is not the first stop
                    bool mergePreviousStop = previousResolvedStop != null;

                    // Distance between the current stop and previous stop are lower than the stopMergeRadius 
                    mergePreviousStop = mergePreviousStop && CheckSameLocation(sv, previousResolvedStop, _stopMergeRadius);

                    // Travelled distance between previous stop and current stop is lower than the _stopMergeMaxTravelRadius
                    mergePreviousStop = mergePreviousStop && previousResolvedTrip?.DistanceTravelled < _stopMergeMaxTravelRadius;

                    if (mergePreviousStop)
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
                        previousResolvedStop.NextTrip = new ResolvedTrip();
                        previousResolvedTrip = previousResolvedStop.NextTrip;
                    }

                    else
                    {
                        if (previousResolvedTrip != null)
                        {
                            previousResolvedTrip.NextStop = sv;
                        }

                        previousResolvedTrip = sv.NextTrip = new ResolvedTrip();

                        resolvedStops.Add(sv);

                        previousResolvedStop = sv;
                    }
                }

                else if (previousResolvedTrip != null)
                {
                    if (previousResolvedTrip.StartTime == default(DateTimeOffset))
                    {
                        PositionEntry first = grp.Positions.First();

                        previousResolvedTrip.StartTime = first.Timestamp;
                        previousResolvedTrip.FirstLatitude = first.Latitude;
                        previousResolvedTrip.FirstLongitude = first.Longitude;
                    }
                    PositionEntry last = grp.Positions.Last();


                    // TODO wrong 0 default val
                    double lat = previousResolvedTrip.LastLatitude > 0 ? previousResolvedTrip.LastLatitude : previousResolvedTrip.FirstLatitude;
                    double lon = previousResolvedTrip.LastLongitude > 0 ? previousResolvedTrip.LastLongitude : previousResolvedTrip.FirstLongitude;

                    previousResolvedTrip.DistanceTravelled += Util.DistanceBetween(lat,
                                                                            lon,
                                                                            last.Latitude,
                                                                            last.Longitude);
                    previousResolvedTrip.EndTime = last.Timestamp;
                    previousResolvedTrip.LastLatitude = last.Latitude;
                    previousResolvedTrip.LastLongitude = last.Longitude;

                }

            }

            return (firstLooseTrack, resolvedStops);
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

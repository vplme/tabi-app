using System;
using Tabi.DataObjects;
using Tabi.Logic;

namespace Tabi.Shared.DataSync
{
    public static class LogicModelConversionExtensions
    {
        public static StopVisit ToStopVisit(this ResolvedStop stop)
        {
            return new StopVisit()
            {
                BeginTimestamp = stop.BeginTimestamp,
                EndTimestamp = stop.EndTimestamp,
                StopAccuracy = stop.AverageAccuracy,
                Latitude = stop.Latitude,
                Longitude = stop.Longitude,
            };

        }

        public static StopVisit ToStopVisitAndStop(this ResolvedStop stop)
        {
            StopVisit sv = new StopVisit()
            {
                BeginTimestamp = stop.BeginTimestamp,
                EndTimestamp = stop.EndTimestamp,
                StopAccuracy = stop.AverageAccuracy,
                Latitude = stop.Latitude,
                Longitude = stop.Longitude,
                Stop = stop.ToStop(),

            };

            if (stop.NextTrip != null)
            {
                sv.NextTrack = stop.NextTrip.ToTrackEntry();
            }

            return sv;
        }

        public static Stop ToStop(this ResolvedStop stop)
        {
            return new Stop()
            {
                Latitude = stop.Latitude,
                Longitude = stop.Longitude,
            };

        }

        public static TrackEntry ToTrackEntry(this ResolvedTrip trip)
        {
            TrackEntry trackEntry = new TrackEntry()
            {
                StartTime = trip.StartTime,
                EndTime = trip.EndTime,
                FirstLatitude = trip.FirstLatitude,
                FirstLongitude = trip.FirstLongitude,
                LastLatitude = trip.LastLatitude,
                LastLongitude = trip.LastLongitude,
                DistanceTravelled = trip.DistanceTravelled,
            };

            if (trip.NextStop != null)
            {
                trackEntry.NextStop = trip.NextStop.ToStopVisitAndStop();
            }

            return trackEntry;
        }
    }
}

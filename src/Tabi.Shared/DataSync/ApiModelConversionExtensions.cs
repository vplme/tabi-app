using System;
using Tabi.DataObjects;

namespace Tabi.Shared.DataSync
{
    public static class ApiModelConversionExtensions
    {
        public static TabiApiClient.Models.Stop ToApiModel(this Stop stop)
        {
            return new TabiApiClient.Models.Stop()
            {
                Longitude = stop.Longitude,
                Latitude = stop.Latitude,
                Name = stop.Name,
                Timestamp = stop.Timestamp,
                LocalId = stop.Id,
            };

        }

        public static TabiApiClient.Models.StopVisit ToApiModel(this StopVisit stopVisit)
        {
            return new TabiApiClient.Models.StopVisit()
            {
                LocalId = stopVisit.Id,
                LocalStopId = stopVisit.StopId,
                BeginTimestamp = stopVisit.BeginTimestamp,
                EndTimestamp = stopVisit.EndTimestamp
            };

        }

        public static TabiApiClient.Models.UserStopMotive ToUserStopMotiveApiModel(this Motive motive)
        {
            return new TabiApiClient.Models.UserStopMotive()
            {
                LocalStopId = motive.StopId,
                Motive = motive.Text,
                Timestamp = motive.Timestamp,
            };

        }

        public static TabiApiClient.Models.TrackMotive ToTrackMotiveMotiveApiModel(this Motive motive)
        {
            return new TabiApiClient.Models.TrackMotive()
            {
                LocalTrackId = motive.TrackId,
                Motive = motive.Text,
                Timestamp = motive.Timestamp,
            };

        }

        public static TabiApiClient.Models.TransportationMode ToApiModel(this TransportationModeEntry entry)
        {
            return new TabiApiClient.Models.TransportationMode()
            {
                LocalTrackId = entry.TrackId,
                DeviceId = Settings.Current.Device,
                Timestamp = entry.Timestamp,
                Walk = entry.Walk,
                Run = entry.Run,
                MobilityScooter = entry.MobilityScooter,
                Car = entry.Car,
                Bike = entry.Bike,
                Moped = entry.Moped,
                Scooter = entry.Scooter,
                Motorcycle = entry.Motorcycle,
                Train = entry.Train,
                Subway = entry.Subway,
                Tram = entry.Tram,
                Bus = entry.Bus,
                Other = entry.Other
            };
        }
    }
}

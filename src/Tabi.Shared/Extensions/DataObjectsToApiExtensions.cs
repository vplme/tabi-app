using System;
using Tabi.DataObjects;

namespace Tabi.Shared.Extensions
{
    public static class DataObjectsToApiExtensions
    {
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
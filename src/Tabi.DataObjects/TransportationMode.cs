using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tabi.DataObjects
{
    public class TransportationModeEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public TransportationMode Mode { get; set; }

        [ManyToMany(typeof(TransportationModeTracks))]
        public List<TrackEntry> Tracks { get; set; }
    }

    public enum TransportationMode
    {
        Walk = 0,
        Run,
        MobilityScooter,
        Car,
        Bike,
        Moped,
        Scooter,
        Motorcycle,
        Train,
        Subway,
        Tram,
        Bus,
        Other,
    }
}

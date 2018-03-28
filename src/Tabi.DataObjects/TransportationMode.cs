using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tabi.DataObjects
{
    public class TransportationMode
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public TransportationModes Mode { get; set; }

        [ManyToMany(typeof(TransportationModeTracks))]
        public List<TrackEntry> Tracks { get; set; }
    }

    public enum TransportationModes
    {
        Walk = 0,
        Run,
        Mobility_Scooter,
        Car,
        Bike,
        Moped,
        Scooter,
        MotorCycle,
        Train,
        Subway,
        Tram,
        Bus,
        Other,
    }
}

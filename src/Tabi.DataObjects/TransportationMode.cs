using SQLite;
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
    }

    public enum TransportationModes
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

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

        public int TrackId { get; set; }

        /// <summary>
        /// Different active modes keys are stored comma separated
        /// </summary>
        /// <value>The active modes.</value>
        public string ActiveModes { get; set; }

        [Indexed]
        public DateTimeOffset Timestamp { get; set; }

    }
}

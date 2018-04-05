using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tabi.DataObjects
{
    public class TransportationModeTracks
    {
        [ForeignKey(typeof(TrackEntry))]
        public Guid TrackId { get; set; }

        [ForeignKey(typeof(TransportationModeEntry))]
        public int TransportationModeId { get; set; }
    }
}

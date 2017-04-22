using System;
using System.Collections.Generic;
using Tabi.DataObjects;

namespace Tabi
{
    public class LocationEventArgs
    {
        public List<PositionEntry> Positions { get; set; }
        public LocationEventArgs()
        {
        }
    }
}

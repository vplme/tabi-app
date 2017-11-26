using System;
namespace Tabi.DataObjects.CollectionProfile
{
    public class LocationRequestProfile
    {
        public long Interval { get; set; }
        public int Priority { get; set; }
        
        public bool AdjustIntervalBasedOnSpeed { get; set; }
        
    }
}

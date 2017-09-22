using System;
namespace Tabi.Shared.Collection
{
    public class LocationRequestProfile
    {
        public long Interval { get; set; }
        public int Priority { get; set; }
        
        public bool AdjustIntervalBasedOnSpeed { get; set; }
        
    }
}

using System;
namespace Tabi.Shared.Collection
{
    public class CoreLocationProfile
    {
        public bool AllowBackgroundLocationUpdates { get; set; } = false;
        public bool PauseLocationUpdatesAutomatically { get; set; } = false;
        public bool MonitorSignificantChanges { get; set; } = false;
        
        public double DesiredAccuracy { get; set; } = -1;

        public double DistanceFilter { get; set; } = -1;

        public bool AllowDeferredLocationUpdates { get; set; } = false;
        public double DeferredLocationUpdatesDistance { get; set; }
        public double DeferredLocationUpdatesTimeout { get; set; }
    }
}

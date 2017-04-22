using System;
namespace Tabi.Shared.Collection
{
    public class CoreLocationProfile
    {
        public bool AllowBackgroundLocationUpdates { get; set; }
        public bool PauseLocationUpdatesAutomatically { get; set; }
        public bool MonitorSignificantChanges { get; set; }

        public double DesiredAccuracy { get; set; }

        public double DistanceFilter { get; set; }

        public bool AllowDeferredLocationUpdates { get; set; }
        public double DeferredLocationUpdatesDistance { get; set; }
        public double DeferredLocationUpdatesTimeout { get; set; }
    }
}

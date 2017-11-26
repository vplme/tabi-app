using System;
using Tabi.DataObjects;

namespace Tabi.DataObjects.CollectionProfile
{
    public class CollectionProfile
    {

        public ProfileiOS iOSProfile { get; set; } = new ProfileiOS();
        public ProfileAndroid AndroidProfile { get; set; } = new ProfileAndroid();

        public CollectionProfile()
        {
        }

        public static CollectionProfile GetDefaultProfile()
        {
            CollectionProfile cProfile = new CollectionProfile()
            {


                iOSProfile = new ProfileiOS()
                {
                    DistanceDeltaLimitLowTracking = 40,
                    DistanceDeltaLowTracking = 100,
                    TimePeriodLowTracking = 60,
                    LowTrackingProfile = new CoreLocationProfile()
                    {
                        DesiredAccuracy = 200,
                        MonitorSignificantChanges = true,
                        AllowDeferredLocationUpdates = false,
                        AllowBackgroundLocationUpdates = true
                    },
                    HighTrackingProfile = new CoreLocationProfile()
                    {
                        DesiredAccuracy = -1,
                        MonitorSignificantChanges = true,
                        AllowDeferredLocationUpdates = true,
                        DeferredLocationUpdatesDistance = 1000,
                        DeferredLocationUpdatesTimeout = 600,
                        AllowBackgroundLocationUpdates = true,
                    }
                },
                AndroidProfile = new ProfileAndroid()
                {
                    DistanceDeltaLimitLowTracking = 40,
                    DistanceDeltaLowTracking = 100,
                    TimePeriodLowTracking = 60,
                    HighTrackingProfile = new LocationRequestProfile()
                    {
                        Interval = 1000,
                        Priority = 100,
                    },
                    LowTrackingProfile = new LocationRequestProfile()
                     {
                         Interval = 60 * 1000,
                         Priority = 102,
                     }
                }
            };

            return cProfile;
        }



    }

    public abstract class AbstractProfile
    {
        /// <summary>
        /// Gets or sets the distance delta limit low tracking.
        /// </summary>
        /// <value>Distance between previous location and current location
        ///  that triggers high tracking when in low tracking mode.</value>
        public double DistanceDeltaLimitLowTracking { get; set; }

        /// <summary>
        /// Gets or sets the distance delta low tracking.
        /// </summary>
        /// <value>Distance location updates must be lower than to trigger
        /// low tracking.</value>
        public double DistanceDeltaLowTracking { get; set; }

        /// <summary>
        /// Gets or sets the time period low tracking.
        /// </summary>
        /// <value>Time period in seconds that will trigger low tracking if
        /// the distance between location updates is under the specified 
        /// value in DistanceDeltaLowTracking.</value>
        public double TimePeriodLowTracking { get; set; }
    }

    public class ProfileAndroid : AbstractProfile
    {
        public LocationRequestProfile HighTrackingProfile { get; set; }
        public LocationRequestProfile LowTrackingProfile { get; set; }
    }

    public class ProfileiOS : AbstractProfile
    {
        public CoreLocationProfile LowTrackingProfile { get; set; }
        public CoreLocationProfile HighTrackingProfile { get; set; }

        
    }

}

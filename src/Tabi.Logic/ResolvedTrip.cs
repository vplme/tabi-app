using System;
namespace Tabi.Logic
{
    public class ResolvedTrip
    {
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }

        public double FirstLatitude { get; set; }
        public double FirstLongitude { get; set; }

        public double LastLatitude { get; set; }
        public double LastLongitude { get; set; }

        public TimeSpan TimeTravelled => EndTime - StartTime;

        public double DistanceTravelled { get; set; }

        public ResolvedStop NextStop { get; set; }
    }
}

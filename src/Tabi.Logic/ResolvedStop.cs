using System;
namespace Tabi.Logic
{
    public class ResolvedStop
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTimeOffset BeginTimestamp { get; set; }
        public DateTimeOffset EndTimestamp { get; set; }
        public TimeSpan TimeSpent { get => EndTimestamp - BeginTimestamp; }
        public double AverageAccuracy { get; set; }
        public double WeightedAccuracy { get; set; }
        public double MinAccuracy { get; set; }
        public double MaxAccuracy { get; set; }


        public ResolvedTrip NextTrip { get; set; }
        public ResolvedTrip PreviousTrip { get; set; }

        public override string ToString()
        {
            return $"{Latitude} {Longitude} - {BeginTimestamp} {EndTimestamp} ({TimeSpent}) - avg : {AverageAccuracy}m weighted: {WeightedAccuracy} min: {MinAccuracy} max: {MaxAccuracy}";
        }
    }
}

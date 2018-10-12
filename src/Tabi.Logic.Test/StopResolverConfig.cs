using System;
namespace Tabi.Logic.Test
{
    public class StopResolverConfig : IStopResolverConfiguration
    {
        public int Time { get; set; }

        public int GroupRadius { get; set; }

        public int MinStopAccuracy { get; set; }

        public int StopMergeRadius { get; set; }

        public int StopMergeMaxTravelRadius { get; set; }
    }
}

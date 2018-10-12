using System;
namespace Tabi.Logic
{
    public interface IStopResolverConfiguration
    {
        int Time { get; }
        int GroupRadius { get; }
        int MinStopAccuracy { get; }
        int StopMergeRadius { get; }
        int StopMergeMaxTravelRadius { get; }
    }
}

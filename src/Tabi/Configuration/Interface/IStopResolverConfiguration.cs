namespace Tabi
{
    public interface IStopResolverConfiguration
    {
        bool Enabled { get; set; }
        bool UserAdjustable { get; set; }
        int Time { get; set; }
        int GroupRadius { get; set; }
        int MinStopAccuracy { get; set; }
        int StopMergeRadius { get; set; }
        int StopMergeMaxTravelRadius { get; set; }
    }
}
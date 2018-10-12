namespace Tabi
{
    public interface IUserInterfaceConfiguration
    {
        bool StopNameReplaceAllEnabled { get; set; }
        bool SuggestPossibleNearbyStopsEnabled { get; set; }
        double SuggestPossibleNearbyStopsDistance { get; set; }
        double SuggestPossibleNearbyStopsCount { get; set; }
        bool ShowNotificationOnAppTermination { get; set; }
    }
}
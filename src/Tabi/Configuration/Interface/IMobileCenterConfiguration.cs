namespace Tabi
{
    public interface IMobileCenterConfiguration
    {
        bool Enabled { get; }
        bool Distribute { get; }
        bool Crashes { get; }
        bool Analytics { get; }
        string ApiKey { get; }
        bool DisableAnalyticsOption { get; }
        bool DisableCrashesOption { get; }
        bool ShouldAskConfirmation { get; }
    }
}
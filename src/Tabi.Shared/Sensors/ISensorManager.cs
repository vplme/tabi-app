namespace Tabi.Shared.Sensors
{
    public interface ISensorManager
    {
        bool IsListening { get; }
        void StartSensorUpdates();
        void StopSensorUpdates();
    }
}

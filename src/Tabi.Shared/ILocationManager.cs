using System;
namespace Tabi
{
    public interface ILocationManager
    {
        bool IsListening { get; }

        void StartLocationUpdates();
        void StopLocationUpdates();

        event EventHandler<LocationEventArgs> LocationsUpdated;
    }
}

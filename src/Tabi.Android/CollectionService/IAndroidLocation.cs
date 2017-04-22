using System;
namespace Tabi.Droid.CollectionService
{
    public interface IAndroidLocation
    {
        void RequestLocationUpdates();
        void StopLocationUpdates();
    }
}

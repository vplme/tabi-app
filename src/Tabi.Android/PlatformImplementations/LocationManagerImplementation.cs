using System;
using Android.App;
using Android.Content;
using Tabi.Droid.PlatformImplementations;

[assembly: Xamarin.Forms.Dependency(typeof(LocationManagerImplementation))]
namespace Tabi.Droid.PlatformImplementations
{
    public class LocationManagerImplementation : ILocationManager
    {
        public LocationManagerImplementation()
        {

        }

        public bool IsListening { get; private set; }
        public event EventHandler<LocationEventArgs> LocationsUpdated;

        public void StartLocationUpdates()
        {
            Intent serviceIntent = new Intent(Application.Context, typeof(LocationService));
            Application.Context.StartService(serviceIntent);
            IsListening = true;
            //Binding service:
			//LocationServiceConnection serviceConnection = new LocationServiceConnection(this);
			//Application.Context.BindService(serviceIntent, serviceConnection, Bind.AutoCreate);
		}

        public void StopLocationUpdates()
        {
			Intent serviceIntent = new Intent(Application.Context, typeof(LocationService));
            Application.Context.StopService(serviceIntent);
            IsListening = false;
        }
    }
}

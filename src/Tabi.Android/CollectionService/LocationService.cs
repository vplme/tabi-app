using System;
using Android;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Hardware;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Tabi.Droid.CollectionService;
using Tabi.Shared.Resx;
using static Android.OS.PowerManager;

namespace Tabi.Droid
{
    [Service]
    public class LocationService : Service
    {
        private LocationServiceBinder binder;

        public override void OnCreate()
        {
            base.OnCreate();
        }

        public const int ServiceRunningNotificationId = 134345;

        public LocationService()
        {
        }

        public override IBinder OnBind(Intent intent)
        {
            binder = new LocationServiceBinder(this);
            return binder;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            var notification = new Notification.Builder(this)
                .SetContentTitle(AppResources.ServiceTitle)
                .SetContentText(AppResources.ServiceText)
                .SetSmallIcon(Resource.Drawable.location)
                .SetOngoing(true)
                .Build();

            IAndroidLocation locationImplementation = null;

            if (IsGooglePlayApiAvailable())
            {
                locationImplementation = new GoogleLocationServicesAPI();
            }
            else
            {
                locationImplementation = new AndroidLocationAPI();
            }

            locationImplementation.RequestLocationUpdates();

            PowerManager sv = (Android.OS.PowerManager) GetSystemService(PowerService);
            WakeLock wklock = sv.NewWakeLock(WakeLockFlags.Partial, "TABI");
            wklock.Acquire();
            
            SensorCollection cs = new SensorCollection(() => locationImplementation.RequestUpdateNow());

            // Enlist this instance of the service as a foreground service
            StartForeground(ServiceRunningNotificationId, notification);

            return StartCommandResult.Sticky;
        }

        private bool IsGooglePlayApiAvailable()
        {
            GoogleApiAvailability googleApiAvailability = GoogleApiAvailability.Instance;
            int resultCode = googleApiAvailability.IsGooglePlayServicesAvailable(Application.Context);
            return resultCode == ConnectionResult.Success;
        }
    }
}
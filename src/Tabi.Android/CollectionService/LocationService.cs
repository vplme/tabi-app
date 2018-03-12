using System;
using System.Runtime.Remoting.Contexts;
using Android;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Hardware;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Plugin.CurrentActivity;
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
            Android.Content.Context context = CrossCurrentActivity.Current.Activity.ApplicationContext;
            var appIntent = context.PackageManager.GetLaunchIntentForPackage(context.PackageName);
            appIntent.AddFlags(ActivityFlags.ClearTop);

            var notification = new Notification.Builder(this)
                .SetContentTitle(AppResources.ServiceTitle)
                .SetContentText(AppResources.ServiceText)
                .SetContentIntent(PendingIntent.GetActivity(context, 0, appIntent, 0))
                .SetSmallIcon(Resource.Drawable.tabi_status_bar_icon)
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

            PowerManager sv = (Android.OS.PowerManager)GetSystemService(PowerService);
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
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.OS;
using Plugin.CurrentActivity;
using Tabi.Droid.CollectionService;
using Tabi.Shared.Resx;

namespace Tabi.Droid
{
    [Service]
    public class LocationService : Service
    {
        public const string SERVICE_CHANNEL = "com.tabiapp.tabi.service";

        public const int ServiceRunningNotificationId = 134345;

        private LocationServiceBinder binder;

        public override void OnCreate()
        {
            base.OnCreate();
        }

        public override IBinder OnBind(Intent intent)
        {
            binder = new LocationServiceBinder(this);
            return binder;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            var context = CrossCurrentActivity.Current.Activity.ApplicationContext;
            var appIntent = context.PackageManager.GetLaunchIntentForPackage(context.PackageName);
            appIntent.AddFlags(ActivityFlags.ClearTop);


            var chanName = AppResources.ServiceNotificationChannelLabel;
            var importance = NotificationImportance.Low;
            var chan = new NotificationChannel(SERVICE_CHANNEL, chanName, importance);

            var notificationManager =
                (NotificationManager) GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(chan);

            var notification = new Notification.Builder(this)
                .SetContentTitle(AppResources.ServiceTitle)
                .SetContentText(AppResources.ServiceText)
                .SetContentIntent(PendingIntent.GetActivity(context, 0, appIntent, 0))
                .SetSmallIcon(Resource.Drawable.tabi_status_bar_icon)
                .SetChannelId(SERVICE_CHANNEL)
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

            var sv = (PowerManager) GetSystemService(PowerService);
            var wklock = sv.NewWakeLock(WakeLockFlags.Partial, "TABI");
            wklock.Acquire();

            var cs = new SensorCollection(() => locationImplementation.RequestUpdateNow());

            // Enlist this instance of the service as a foreground service
            StartForeground(ServiceRunningNotificationId, notification);

            return StartCommandResult.Sticky;
        }

        private bool IsGooglePlayApiAvailable()
        {
            var googleApiAvailability = GoogleApiAvailability.Instance;
            var resultCode = googleApiAvailability.IsGooglePlayServicesAvailable(Application.Context);
            return resultCode == ConnectionResult.Success;
        }
    }
}
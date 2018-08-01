using System;
using Android.App;
using Android.Content;
using Android.OS;
using Tabi.Resx;

namespace Tabi.Droid.Helpers
{
    public class NotificationChannelHelper : ContextWrapper
    {
        public const string SERVICE_CHANNEL = "com.tabiapp.tabi.service";

        NotificationManager manager;

        public NotificationChannelHelper(Context context) : base(context)
        {
        }

        NotificationManager Manager
        {
            get
            {
                if (manager == null)
                {
                    manager = (NotificationManager)GetSystemService(NotificationService);
                }
                return manager;
            }
        }

        public void SetupNotificationChannels()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                CreateServiceChannel();
            }
        }

        private void CreateServiceChannel()
        {

            var chanName = AppResources.ServiceNotificationChannelLabel;
            var importance = NotificationImportance.Low;
            var chan = new NotificationChannel(SERVICE_CHANNEL, chanName, importance);

            Manager.CreateNotificationChannel(chan);
        }


    }
}

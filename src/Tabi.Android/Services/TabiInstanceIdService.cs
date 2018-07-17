using System;
using Android.App;
using Firebase.Iid;

namespace Tabi.Droid.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class TabiInstanceIdService : FirebaseInstanceIdService
    {
        const string TAG = "TabiInstanceIdService";

        public override void OnTokenRefresh()
        {
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            Log.Debug(TAG + "Refreshed token: " + refreshedToken);
            SendRegistrationToServer(refreshedToken);
        }
        void SendRegistrationToServer(string token)
        {
            Settings.Current.DeviceToken = token;
        }
    }
}

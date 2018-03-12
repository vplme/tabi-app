using Android.Content;
using Android.OS;

namespace Tabi.Droid
{
    public class LocationServiceConnection : Java.Lang.Object, IServiceConnection
    {
        ILocationServiceBinderClient client;

        public LocationServiceConnection(ILocationServiceBinderClient client)
        {
            this.client = client;
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            var demoServiceBinder = service as LocationServiceBinder;
            if (demoServiceBinder != null)
            {
                client.Binder = demoServiceBinder;
                client.IsBound = true;
                client.OnBinding();
            }
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            client.IsBound = false;
        }
    }
}

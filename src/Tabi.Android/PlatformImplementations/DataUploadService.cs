using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Autofac;
using Tabi.DataSync;

namespace Tabi.Droid.PlatformImplementations
{
    [Service]
    public class DataUploadService : IntentService
    {
        public DataUploadService() : base("DataUploadService")
        {
        }
        
        protected override async void OnHandleIntent(Intent intent)
        {
            int interval = intent.GetIntExtra("interval", 10);
            bool wifiOnly = intent.GetBooleanExtra("autoUpload", true);

            SyncService sync = App.Container.Resolve<SyncService>();
            await sync.AutoUpload(TimeSpan.FromMinutes(interval), wifiOnly);
        }
    }
}

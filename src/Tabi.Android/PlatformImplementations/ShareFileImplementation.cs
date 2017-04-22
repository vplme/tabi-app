using System;
using Android.Content;
using Android.Content.PM;
using Android.Support.V4.Content;
using Tabi.Droid;
using Tabi.Shared.Resx;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(ShareFileImplementation))]
namespace Tabi.Droid
{
    public class ShareFileImplementation : IShareFile
    {
        public void ShareFile(string path, string mime = "text/plain")
        {
            var context = Xamarin.Forms.Forms.Context;


            Log.Info($"Sharing file with path: {path}");
            //var uri = Android.Net.Uri.Parse("file://" + path);
            var intent = new Intent(Intent.ActionView);
            intent.SetFlags(ActivityFlags.GrantReadUriPermission);
            Java.IO.File file = new Java.IO.File(path);
            var urlFP = FileProvider.GetUriForFile(context, "com.tabiapp.fileprovider", file);
            intent.SetDataAndType(urlFP, mime);

            //intent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);

            try
            {
                //PackageManager pm = context.PackageManager;
                //if (intent.ResolveActivity(pm) != null)
                //{
                //    context.StartActivity(intent);
                //}
                context.StartActivity(Intent.CreateChooser(intent, AppResources.OpenFileInApp));
            }
            catch (Exception ex)
            {
                Log.Error($"Exception occured while sharing a file: {ex}");
            }
        }
    }
}

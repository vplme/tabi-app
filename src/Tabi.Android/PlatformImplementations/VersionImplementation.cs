using Tabi.Helpers;
using Tabi.Droid.PlatformImplementations;
using Android.Content;
using Plugin.CurrentActivity;

[assembly: Xamarin.Forms.Dependency(typeof(VersionImplementation))]
namespace Tabi.Droid.PlatformImplementations
{
    public class VersionImplementation : IVersion
    {
        public string GetVersion()
        {
            Context context = CrossCurrentActivity.Current.Activity;
            string versionName = context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionName;
            int versionBuild = context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionCode;

            return $"{versionName} ({versionBuild})";
        }
    }
}

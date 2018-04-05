using Tabi.Helpers;
using Xamarin.Forms;
using Tabi.Droid.PlatformImplementations;
using Android.Content;

[assembly: Xamarin.Forms.Dependency(typeof(VersionImplementation))]
namespace Tabi.Droid.PlatformImplementations
{
    public class VersionImplementation : IVersion
    {
        public string GetVersion()
        {
            Context context = Forms.Context;
            return context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionName;
        }
    }
}

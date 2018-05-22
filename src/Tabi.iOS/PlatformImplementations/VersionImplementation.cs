using Foundation;
using Tabi.Helpers;
using Tabi.iOS.PlatformImplementations;

[assembly: Xamarin.Forms.Dependency(typeof(VersionImplementation))]
namespace Tabi.iOS.PlatformImplementations
{
    public class VersionImplementation : IVersion
    {
        public string GetVersion()
        {
            
            string shortString = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"] as NSString;
            string version = NSBundle.MainBundle.InfoDictionary["CFBundleVersion"] as NSString;

            return $"{shortString} ({version})";
        }
    }
}

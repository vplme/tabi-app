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
            NSObject obj = NSBundle.MainBundle.InfoDictionary["CFBundleVersion"];
            if(obj != null)
            {
                return obj as NSString;
            }

            return "";
        }
    }
}

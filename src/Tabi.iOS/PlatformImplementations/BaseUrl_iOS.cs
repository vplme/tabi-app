using Foundation;
using Tabi.iOS.PlatformImplementations;
using Xamarin.Forms;

[assembly: Dependency (typeof (BaseUrl_iOS))]
namespace Tabi.iOS.PlatformImplementations
{
    public class BaseUrl_iOS : IBaseUrl
    {
        public string Get()
        {
            return NSBundle.MainBundle.BundlePath;
        }
    }
}
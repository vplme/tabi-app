using Tabi.Droid.PlatformImplementations;
using Tabi.Helpers;
using Xamarin.Forms;

[assembly: Dependency (typeof(BaseUrl_Android))]
namespace Tabi.Droid.PlatformImplementations {
    
  public class BaseUrl_Android : IBaseUrl {
    public string Get() {
      return "file:///android_asset/";
    }
  }
}
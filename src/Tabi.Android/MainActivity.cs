
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using FormsToolkit.Droid;
using Plugin.CurrentActivity;
using Acr.UserDialogs;
using Plugin.Permissions;

namespace Tabi.Droid
{
    [Activity(Label = "Tabi", Icon = "@drawable/icon", RoundIcon = "@drawable/ic_launcher_round", Theme = "@style/TabiTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            Toolkit.Init();
            Xamarin.FormsMaps.Init(this, bundle);
            UserDialogs.Init(() => CrossCurrentActivity.Current.Activity);

			var width = Resources.DisplayMetrics.WidthPixels;
			var height = Resources.DisplayMetrics.HeightPixels;
			var density = Resources.DisplayMetrics.Density;

            App.ScreenWidth = (width - 0.5f) / density;
            App.ScreenHeight = (height - 0.5f) / density;

            LoadApplication(new App());
		}

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

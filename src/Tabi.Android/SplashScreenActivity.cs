using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;

namespace Tabi.Droid
{
    [Activity(Theme = "@style/TabiTheme.Splash", Label = "Tabi", Icon = "@drawable/icon", RoundIcon = "@drawable/ic_launcher_round", MainLauncher = true, NoHistory = true)]
    public class SplashScreenActivity : AppCompatActivity
    {
        static readonly string TAG = "X:" + typeof(SplashScreenActivity).Name;

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
        }

        // Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }

        public override void OnBackPressed() { }
    }
}

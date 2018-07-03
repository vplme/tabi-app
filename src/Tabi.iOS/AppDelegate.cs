
using System;
using Autofac.Core;
using FFImageLoading.Forms.Touch;
using FFImageLoading.Svg.Forms;
using Foundation;
using Microsoft.AppCenter.Distribute;
using Tabi.iOS.PlatformImplementations;
using UIKit;
using Vpl.Xamarin.VideoPlayer;

namespace Tabi.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            CachedImageRenderer.Init();
            // to avoid linking issues:
            var ignore = typeof(SvgCachedImage);

            VideoPlayerKit.Init();

            global::Xamarin.Forms.Forms.Init();

            App.ScreenWidth = UIScreen.MainScreen.Bounds.Width;
            App.ScreenHeight = UIScreen.MainScreen.Bounds.Height;

            //UINavigationBar.Appearance.BarTintColor = UIColor.FromRGB(24, 150, 188);
            UIColor accentColor = UIColor.FromRGB(24, 150, 188);
            UINavigationBar.Appearance.TintColor = accentColor;
            UISwitch.Appearance.OnTintColor = accentColor;

            UITextAttributes textAttributes = null;
            if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
            {
                textAttributes = new UITextAttributes()
                {
                    Font = UIFont.FromName("SanFrancisco", (nfloat)20f),
                    TextShadowColor = UIColor.Clear,
                };
            }
            else
            {
                textAttributes = new UITextAttributes()
                {
                    Font = UIFont.FromName("HelveticaNeue-Light", (nfloat)20f),
                    TextShadowColor = UIColor.Clear,
                };
            }

            UINavigationBar.Appearance.SetTitleTextAttributes(textAttributes);

            Distribute.DontCheckForUpdatesInDebug();

            LoadApplication(new Tabi.App(new IModule[] { new PlatformContainerModule() }));

            return base.FinishedLaunching(app, options);
        }
    }
}

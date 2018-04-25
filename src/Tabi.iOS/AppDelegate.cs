﻿
using Autofac.Core;
using Foundation;
using Microsoft.Azure.Mobile.Distribute;
using Tabi.iOS.PlatformImplementations;
using UIKit;

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
            global::Xamarin.Forms.Forms.Init();

            App.ScreenWidth = UIScreen.MainScreen.Bounds.Width;
            App.ScreenHeight = UIScreen.MainScreen.Bounds.Height;

            Distribute.DontCheckForUpdatesInDebug();

            LoadApplication(new Tabi.App(new IModule[] { new PlatformContainerModule() }));

            return base.FinishedLaunching(app, options);
        }
    }
}

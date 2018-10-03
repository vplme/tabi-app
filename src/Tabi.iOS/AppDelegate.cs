
using System;
using System.Threading;
using Autofac.Core;
using CoreFoundation;
using FFImageLoading.Svg.Forms;
using Foundation;
using Microsoft.AppCenter.Distribute;
using Refractored.XamForms.PullToRefresh.iOS;
using Tabi.Helpers;
using Tabi.iOS.PlatformImplementations;
using Tabi.Logging;
using Tabi.Resx;
using UIKit;
using UserNotifications;
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
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            // to avoid linking issues:
            var ignore = typeof(SvgCachedImage);

            VideoPlayerKit.Init();
            PullToRefreshLayoutRenderer.Init();

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

            LoadApplication(new App(new IModule[] { new PlatformContainerModule() }));

            if (options != null && options.ContainsKey(UIApplication.LaunchOptionsLocationKey))
            {
                Log.Info("LaunchOptions contains locationkey");
            }

            if (App.TabiConfig.Notifications.Enabled)
            {
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            }

            UNUserNotificationCenter.Current.RemoveDeliveredNotifications(new string[] { "termination" });

            return base.FinishedLaunching(app, options);
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            Settings.Current.DeviceToken = deviceToken.ToString();
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            Log.Error("Failed to register for remote notifications " + error);
        }

        public override void WillTerminate(UIApplication uiApplication)
        {
            Log.Info("App was terminated");

            if (App.TabiConfig.UserInterface.ShowNotificationOnAppTermination &&
                Settings.Current.Tracking)
            {
                ScheduleAppTerminatedNotification();
            }
        }

        public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        {
        }

        public static void ScheduleAppTerminatedNotification()
        {
            UNMutableNotificationContent content = new UNMutableNotificationContent
            {
                Title = AppResources.TerminationNotificationTitle,
                Body = AppResources.TerminationNotificationBody
            };

            var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(5, false);

            var requestID = "termination";
            var request = UNNotificationRequest.FromIdentifier(requestID, content, trigger);

            UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
            {
                if (err != null)
                {
                }
            });

            // Needed to get the notification to schedule
            Thread.Sleep(2);
        }

        public override void DidEnterBackground(UIApplication uiApplication)
        {
            // Register empty background task to ensure we get willTerminate calls
            nint taskId = 0;

            taskId = UIApplication.SharedApplication.BeginBackgroundTask(() =>
            {
                UIApplication.SharedApplication.EndBackgroundTask(taskId);
                taskId = UIApplication.BackgroundTaskInvalid;
            });

            DispatchQueue.GetGlobalQueue(DispatchQueuePriority.Default).DispatchAsync(() =>
            {
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    taskId = UIApplication.BackgroundTaskInvalid;
                    UIApplication.SharedApplication.EndBackgroundTask(taskId);
                });
            });

        }
    }
}

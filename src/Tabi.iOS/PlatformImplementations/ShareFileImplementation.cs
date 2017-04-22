using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Foundation;
using Tabi.iOS;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(ShareFileImplementation))]
namespace Tabi.iOS
{
	public class ShareFileImplementation : IShareFile
	{
		public void ShareFile(string path, string mime)
		{
            Log.Info($"Sharing file with path: {path}");
            UIDocumentInteractionController viewer = UIDocumentInteractionController.FromUrl(NSUrl.FromFilename(path));
			var controller = GetVisibleViewController();
			viewer.Uti = "public.data";
			viewer.PresentOptionsMenu(controller.View.Frame, controller.View, true);

			var task = new TaskCompletionSource<bool>();
			viewer.DidDismissOptionsMenu += (sender, e) =>
			{
				task.TrySetResult(true);
			};
		}


		// Search recursively for the top most UIViewController
		// Source: http://stackoverflow.com/questions/41241508/xamarin-forms-warning-attempt-to-present-on-whose-view-is-not-in-the-window
		private UIViewController GetVisibleViewController(UIViewController controller = null)
		{
			controller = controller ?? UIApplication.SharedApplication.KeyWindow.RootViewController;

			if (controller.PresentedViewController == null)
				return controller;

			if (controller.PresentedViewController is UINavigationController)
			{
				return ((UINavigationController)controller.PresentedViewController).VisibleViewController;
			}

			if (controller.PresentedViewController is UITabBarController)
			{
				return ((UITabBarController)controller.PresentedViewController).SelectedViewController;
			}

			return GetVisibleViewController(controller.PresentedViewController);
		}
	}
}

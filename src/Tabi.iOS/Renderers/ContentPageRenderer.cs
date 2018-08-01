using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tabi.Controls;
using Tabi.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ContentPage), typeof(ContentPageRenderer))]
namespace Tabi.iOS.Renderers

{
    public class ContentPageRenderer : PageRenderer
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            var contentPage = this.Element as ContentPage;
            if (contentPage == null || NavigationController == null)
            {
                return;
            }

            IList<ToolbarItem> toolbarItems = contentPage.ToolbarItems;

            UINavigationItem navigationItem = this.NavigationController.TopViewController.NavigationItem;
            List<UIBarButtonItem> leftNativeButtons = (navigationItem.LeftBarButtonItems ?? new UIBarButtonItem[] { }).ToList();
            List<UIBarButtonItem> rightNativeButtons = (navigationItem.RightBarButtonItems ?? new UIBarButtonItem[] { }).ToList();

            List<UIBarButtonItem> newLeftButtons = new List<UIBarButtonItem>();
            List<UIBarButtonItem> newRightButtons = new List<UIBarButtonItem>();

            rightNativeButtons.ForEach(nativeItem =>
            {
                // [Hack] Get Xamarin private field "item"
                var field = nativeItem.GetType().GetField("_item", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field == null)
                    return;

                object value = field.GetValue(nativeItem);

                if (value is ExtendedToolbarItem)
                {
                    var info = value as ExtendedToolbarItem;

                    if (info.Done)
                    {
                        nativeItem.Style = UIBarButtonItemStyle.Done;
                    }

                    if (info.Left)
                    {
                        newLeftButtons.Add(nativeItem);
                    }
                    else
                    {
                        newRightButtons.Add(nativeItem);
                    }
                }
                else
                {
                    newRightButtons.Add(nativeItem);
                }
            });

            leftNativeButtons.ForEach(nativeItem =>
            {
                newLeftButtons.Add(nativeItem);
            });

            navigationItem.RightBarButtonItems = newRightButtons.ToArray();
            navigationItem.LeftBarButtonItems = newLeftButtons.ToArray();
        }
    }
}
using System;
using Tabi;
using Tabi.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
[assembly: ExportRenderer(typeof(UnanimatedListView), typeof(UnanimatedListViewRenderer))]

namespace Tabi.iOS.Renderers
{
    public class UnanimatedListViewRenderer : ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);
            InsertRowsAnimation = UIKit.UITableViewRowAnimation.None;

        }
    }
}

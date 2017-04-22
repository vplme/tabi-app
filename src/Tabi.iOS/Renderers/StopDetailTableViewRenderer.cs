using System;
using Tabi.iOS;
using Tabi;
using Xamarin.Forms;
using Xamarin.Forms.Maps.iOS;
using MapKit;
using Xamarin.Forms.Platform.iOS;
using CoreLocation;
using UIKit;
using Tabi.Logging;

[assembly: ExportRenderer(typeof(StopDetailTableView), typeof(StopDetailTableViewRenderer))]
namespace Tabi.iOS
{
    public class StopDetailTableViewRenderer : TableViewRenderer
    {
        public StopDetailTableViewRenderer()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<TableView> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null)
            {
      
            }

            if (e.NewElement != null)
            {
                Control.Bounces = false;

            }
        }
    }
}


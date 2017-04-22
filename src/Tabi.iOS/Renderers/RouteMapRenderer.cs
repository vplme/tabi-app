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

[assembly: ExportRenderer(typeof(RouteMap), typeof(RouteMapRenderer))]
namespace Tabi.iOS
{
    public class RouteMapRenderer : MapRenderer
    {
        public RouteMapRenderer()
        {
            MessagingCenter.Subscribe<RouteMap>(this, "Clear", ClearMap);
            MessagingCenter.Subscribe<RouteMap>(this, "DrawRoute", DrawRoute);
        }

        private void DrawRoute(RouteMap formsMap)
        {
            var nativeMap = Control as MKMapView;

            nativeMap.OverlayRenderer = GetOverlayRenderer;

            CLLocationCoordinate2D[] coords = new CLLocationCoordinate2D[formsMap.RouteCoordinates.Count];

            int index = 0;
            foreach (var position in formsMap.RouteCoordinates)
            {
                coords[index] = new CLLocationCoordinate2D(position.Latitude, position.Longitude);
                index++;
            }

            routeOverlay = MKPolyline.FromCoordinates(coords);
            routeOverlay.Title = "Route";
            nativeMap.AddOverlay(routeOverlay);
        }

        private void ClearMap(RouteMap obj)
        {
            var nativeMap = Control as MKMapView;

            foreach (IMKOverlay ov in nativeMap.Overlays)
            {
                nativeMap.RemoveOverlay(ov);
            }
        }

        MKPolylineRenderer polylineRenderer;
        private MKPolyline routeOverlay;

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                var nativeMap = Control as MKMapView;
                nativeMap.OverlayRenderer = null;
            }

            if (e.NewElement != null)
            {
                var formsMap = (RouteMap)e.NewElement;
                DrawRoute(formsMap);
            }
        }

        MKOverlayRenderer GetOverlayRenderer(MKMapView mapView, IMKOverlay overlay)
        {
            if (polylineRenderer == null)
            {
                polylineRenderer = new MKPolylineRenderer(overlay as MKPolyline)
                {
                    FillColor = UIColor.Blue,
                    StrokeColor = UIColor.Red,
                    LineWidth = 3,
                    Alpha = 0.4f
                };
            }
            return polylineRenderer;
        }
    }
}


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
using System.Collections.Generic;

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


            List<MKPolyline> polylines = new List<MKPolyline>();
            foreach (Line line in formsMap.Lines)
            {
                if(line.Color.Equals(Color.Red))
                {
                    nativeMap.OverlayRenderer = GetOverlayRendererRed;

                }
                else{
                    nativeMap.OverlayRenderer = GetOverlayRendererBlue;

                }

                CLLocationCoordinate2D[] cds = new CLLocationCoordinate2D[line.Positions.Count];

                int index = 0;
                foreach (var position in line.Positions)
                {
                    cds[index] = new CLLocationCoordinate2D(position.Latitude, position.Longitude);
                    index++;
                }

                MKPolyline overlay = MKPolyline.FromCoordinates(cds);
                polylines.Add(overlay);
            }
            nativeMap.AddOverlays(polylines.ToArray());
        }

        private void ClearMap(RouteMap obj)
        {
            var nativeMap = Control as MKMapView;

            nativeMap.RemoveOverlays(nativeMap.Overlays);
        }

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

        MKOverlayRenderer GetOverlayRendererRed(MKMapView mapView, IMKOverlay overlay)
        {
                var polylineRenderer = new MKPolylineRenderer(overlay as MKPolyline)
                {
                    FillColor = UIColor.Blue,
                    StrokeColor = UIColor.Red,
                    LineWidth = 3,
                    Alpha = 0.4f
                };
            return polylineRenderer;
        }

        MKOverlayRenderer GetOverlayRendererBlue(MKMapView mapView, IMKOverlay overlay)
        {
            var polylineRenderer = new MKPolylineRenderer(overlay as MKPolyline)
            {
                FillColor = UIColor.Blue,
                StrokeColor = UIColor.Blue,
                LineWidth = 3,
                Alpha = 0.4f
            };
            return polylineRenderer;
        }

    }
}


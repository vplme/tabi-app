using Tabi.iOS;
using Tabi;
using Xamarin.Forms;
using Xamarin.Forms.Maps.iOS;
using MapKit;
using Xamarin.Forms.Platform.iOS;
using CoreLocation;
using UIKit;
using System.Collections.Generic;
using Tabi.Shared.Controls;

[assembly: ExportRenderer(typeof(RouteMap), typeof(RouteMapRenderer))]
namespace Tabi.iOS
{
    public class RouteMapRenderer : MapRenderer, IMapControl
    {
        private RouteMap formsMap;

        public void Draw()
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

        public void Clear()
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
                formsMap = (RouteMap)e.NewElement;
                formsMap.MapControl = this;
                Draw();
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


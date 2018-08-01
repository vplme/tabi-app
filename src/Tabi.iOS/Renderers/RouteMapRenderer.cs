using Tabi.iOS;
using Tabi;
using Xamarin.Forms;
using Xamarin.Forms.Maps.iOS;
using MapKit;
using Xamarin.Forms.Platform.iOS;
using CoreLocation;
using UIKit;
using System.Collections.Generic;
using ObjCRuntime;
using System;
using Tabi.Controls;

[assembly: ExportRenderer(typeof(RouteMap), typeof(RouteMapRenderer))]
namespace Tabi.iOS
{
    public class RouteMapRenderer : MapRenderer, IMapControl
    {
        private RouteMap formsMap;

        private UIColor currentFillColor;
        private UIColor currentStrokeColor;
        private nfloat currentLineWidth;

        public void Draw()
        {
            var nativeMap = Control as MKMapView;
            nativeMap.OverlayRenderer = GetOverlayRenderer;

            List<MKPolyline> polylines = new List<MKPolyline>();
            foreach (Line line in formsMap.Lines)
            {
                currentFillColor = line.FillColor.ToUIColor();
                currentStrokeColor = line.StrokeColor.ToUIColor();

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

            List<MKCircle> circles = new List<MKCircle>();
            foreach (Circle circle in formsMap.Circles)
            {
                currentFillColor = circle.FillColor.ToUIColor();
                currentStrokeColor = circle.StrokeColor.ToUIColor();
                currentLineWidth = (nfloat)circle.LineWidth;

                MKCircle overlay = MKCircle.Circle(new CLLocationCoordinate2D(circle.Position.Latitude, circle.Position.Longitude), circle.Radius);
                circles.Add(overlay);
            }

            nativeMap.AddOverlays(polylines.ToArray());
            nativeMap.AddOverlays(circles.ToArray());

        }

        public void Clear()
        {
            var nativeMap = Control as MKMapView;
            if (nativeMap.Overlays != null)
            {
                nativeMap.RemoveOverlays(nativeMap.Overlays);
            }
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
                Clear();
                Draw();
            }
        }

        MKOverlayRenderer GetOverlayRenderer(MKMapView mapView, IMKOverlay overlayWrapper)
        {
            MKOverlayRenderer renderer = null;

            var overlay = Runtime.GetNSObject(overlayWrapper.Handle) as IMKOverlay;

            Type overlayType = overlayWrapper.GetType();
            if (overlayType == typeof(MKCircle))
            {
                MKCircleRenderer circleRenderer = new MKCircleRenderer(overlay as MKCircle)
                {
                    FillColor = currentFillColor.ColorWithAlpha(0.1f),
                };

                if (currentLineWidth != 0)
                {
                    circleRenderer.StrokeColor = currentStrokeColor;
                    circleRenderer.LineWidth = currentLineWidth;
                }

                renderer = circleRenderer;
            }

            else if (overlayType == typeof(MKPolyline))
            {
                renderer = new MKPolylineRenderer(overlay as MKPolyline)
                {
                    FillColor = currentFillColor,
                    StrokeColor = currentStrokeColor,
                    LineWidth = 3,
                    Alpha = 0.4f
                };
            }
            return renderer;
        }
    }
}


using System;
using System.Collections.Generic;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Tabi;
using Tabi.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;

[assembly: ExportRenderer(typeof(RouteMap), typeof(RouteMapRenderer))]
namespace Tabi.Droid.Renderers
{
    public class RouteMapRenderer : MapRenderer, IOnMapReadyCallback
    {
        GoogleMap googleMap;
        RouteMap formsMap;
        List<Position> routeCoordinates;
        bool mapReady;

        public RouteMapRenderer()
        {
            MessagingCenter.Subscribe<RouteMap>(this, "Clear", ClearMap);
            MessagingCenter.Subscribe<RouteMap>(this, "DrawRoute", DrawRoute);
        }

        private void DrawRoute(RouteMap formsMap)
        {
           // routeCoordinates = formsMap.RouteCoordinates;

            // Only run if OnMapReady() has already been called
            if(!mapReady)
            {
                Log.Info("OnMapReady not yet called. Returning");
                return;
            }

            Log.Debug("DrawRoute Executed");

            // Add line based on the coordinates in routeCoordinates.
            var polylineOptions = new PolylineOptions();
            polylineOptions.InvokeColor(0x66FF0000);

            foreach (var position in routeCoordinates)
            {
                polylineOptions.Add(new LatLng(position.Latitude, position.Longitude));
            }

            googleMap.AddPolyline(polylineOptions);

            // Add markers based on Forms Maps Pins
            foreach (Pin pin in formsMap.Pins)
            {
                MarkerOptions mO = new MarkerOptions();
                mO.SetTitle(pin.Label);
                mO.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
                googleMap.AddMarker(mO);
            }
        }

        private void ClearMap(RouteMap formsMap)
        {
            Log.Debug("RouteMap Cleared");

            googleMap?.Clear();
        }

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // Unsubscribe
            }

            if (e.NewElement != null)
            {
                formsMap = (RouteMap)e.NewElement;

                ((MapView)Control).GetMapAsync(this);
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            Log.Debug("GoogleMap Ready");
            mapReady = true;
            this.googleMap = googleMap;

            DrawRoute(formsMap);
        }
    }
}

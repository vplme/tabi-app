using System;
using System.Collections.Generic;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Tabi;
using Tabi.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;

[assembly: ExportRenderer(typeof(RouteMap), typeof(RouteMapRenderer))]
namespace Tabi.Droid
{
    public class RouteMapRenderer : MapRenderer, IOnMapReadyCallback
    {
		GoogleMap map;
		List<Position> routeCoordinates;

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				// Unsubscribe
			}

			if (e.NewElement != null)
			{
				var formsMap = (RouteMap)e.NewElement;
				routeCoordinates = formsMap.RouteCoordinates;

				((MapView)Control).GetMapAsync(this);
			}
        }

		public void OnMapReady(GoogleMap googleMap)
		{
			map = googleMap;

			var polylineOptions = new PolylineOptions();
			polylineOptions.InvokeColor(0x66FF0000);

			foreach (var position in routeCoordinates)
			{
				polylineOptions.Add(new LatLng(position.Latitude, position.Longitude));
			}

			map.AddPolyline(polylineOptions);
		}
    }
}

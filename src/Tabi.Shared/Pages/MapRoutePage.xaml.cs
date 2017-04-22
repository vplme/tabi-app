using System;
using System.Collections.Generic;
using System.Diagnostics;
using Tabi.DataObjects;
using Tabi.Model;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Tabi.Pages
{
    public partial class MapRoutePage : ContentPage
    {
        public Model.Stop Stop { get; set; }
        public List<PositionEntry> Route { get; set; }

        public MapRoutePage()
        {
            InitializeComponent();
            routeMap.ClearMap();

        }

        public void ShowMap()
        {
             if (Stop != null)
            {
                var pos = new Xamarin.Forms.Maps.Position(Stop.Latitude, Stop.Longitude);
                routeMap.MoveToRegion(MapSpan.FromCenterAndRadius(pos, Distance.FromMiles(1.0)));
                routeMap.Pins.Add(new Pin() { Label = "Stop", Position = pos, Type = PinType.SearchResult });
            }
            if (Route != null && Route.Count > 0)
            {
                PositionEntry avg = Util.AveragePosition(Route);
                foreach (PositionEntry p in Route)
                {
                    routeMap.RouteCoordinates.Add(new Xamarin.Forms.Maps.Position(p.Latitude, p.Longitude));
                }

                routeMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(avg.Latitude, avg.Longitude), Distance.FromMeters(2500)));

            }


        }

        private void Handle_Clicked(object sender, System.EventArgs e)
        {
            routeMap.ClearMap();
        }
    }
}

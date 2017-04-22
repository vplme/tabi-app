using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Tabi
{
    public class RouteMap : Map
    {
        public List<Position> RouteCoordinates { get; set; }
        public string Message = "";

        public RouteMap()
        {
            RouteCoordinates = new List<Position>();

        }

        public void ClearMap()
        {
            MessagingCenter.Send<RouteMap>(this, "Clear");
        }

        public void DrawRoute()
        {
            Message = "Hello!";
            MessagingCenter.Send<RouteMap>(this, "DrawRoute");
        }
    }
}

using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Tabi
{
    public class RouteMap : Map
    {
        public List<Line> Lines { get; set; } = new List<Line>();

        public string Message = "";

        public RouteMap()
        {
            Lines = new List<Line>();

        }

        public void ClearMap()
        {
            MessagingCenter.Send<RouteMap>(this, "Clear");
        }

        public void DrawRoute()
        {
            MessagingCenter.Send<RouteMap>(this, "DrawRoute");
        }
    }

    public class Line {
        
        public List<Position> Positions { get; set; } = new List<Position>();

        public Color Color { get; set; } = Color.Red;

    }
}

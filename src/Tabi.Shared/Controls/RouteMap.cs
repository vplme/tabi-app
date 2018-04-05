using System.Collections.Generic;
using Tabi.Shared.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Tabi
{
    public class RouteMap : Map
    {
        public List<Line> Lines { get; set; } = new List<Line>();
        public IMapControl MapControl { get; set; }

        public string Message = "";

        public RouteMap()
        {
            Lines = new List<Line>();
        }

        public void ClearMap()
        {
            if(MapControl != null)
            {
                MapControl.Clear();
            }
        }

        public void DrawRoute()
        {
            if (MapControl != null)
            {
                MapControl.Draw();
            }
        }
    }

    public class Line {
        
        public List<Position> Positions { get; set; } = new List<Position>();

        public Color Color { get; set; } = Color.Red;

    }
}

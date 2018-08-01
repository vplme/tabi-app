using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Tabi.Controls
{
    public class RouteMap : Map
    {
        public List<Line> Lines { get; set; } = new List<Line>();

        public List<Circle> Circles { get; set; } = new List<Circle>();


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

        public Color FillColor { get; set; } = Color.Red;
        public Color StrokeColor { get; set; } = Color.Red;
    }

    public class Circle
    {
        public Position Position { get; set; }

        public double Radius { get; set; }

        public double LineWidth { get; set; }

        public Color FillColor { get; set; } = Color.LightBlue;

        public Color StrokeColor { get; set; } = Color.DarkBlue;
    }
}

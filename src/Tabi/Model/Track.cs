using MvvmHelpers;
using Tabi.DataObjects;
using Xamarin.Forms;

namespace Tabi.Model
{
    public class Track : ObservableObject
    {
        public Track()
        {
        }

        public TrackEntry TrackEntry { get; set; }

        string text;
        public string Text
        {
            get { return text; }
            set { SetProperty(ref text, value); }
        }

        bool visible;
        public bool Visible
        {
            get { return visible; }
            set { SetProperty(ref visible, value); }
        }


        Color color;
        public Color Color
        {
            get { return color; }
            set { SetProperty(ref color, value); }
        }

        double height;
        public double Height
        {
            get { return height; }
            set { SetProperty(ref height, value); }
        }

        TrackType movementType;
        public TrackType MovementType
        {
            get
            {
                return movementType;
            }
            set
            {
                SetProperty(ref movementType, value);
            }
        }
    }

    public enum TrackType { Automotive, Walking, Cycling }
}

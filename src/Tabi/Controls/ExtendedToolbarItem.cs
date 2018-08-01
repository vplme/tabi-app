using Xamarin.Forms;

namespace Tabi.Controls
{
    public class ExtendedToolbarItem : ToolbarItem
    {
        static readonly BindableProperty DoneProperty = BindableProperty.Create("Done", typeof(bool), typeof(ExtendedToolbarItem), false);
        static readonly BindableProperty LeftProperty = BindableProperty.Create("Left", typeof(bool), typeof(ExtendedToolbarItem), false);

        public bool Done
        {
            get { return (bool)GetValue(DoneProperty); }
            set { SetValue(DoneProperty, value); }
        }

        public bool Left
        {
            get { return (bool)GetValue(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }

    }
}

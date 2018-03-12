using Xamarin.Forms;

namespace Tabi.Shared.Controls
{

    public partial class TabiTextCell : ViewCell
    {

        public TabiTextCell()
        {
            InitializeComponent();
        }
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create("Text", typeof(string), typeof(TabiTextCell), "");

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
}

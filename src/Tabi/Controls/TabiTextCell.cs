using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tabi.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class TabiTextCell : TextCell
    {

        public static readonly BindableProperty UITableViewStyleProperty =
            BindableProperty.Create("UITableViewStyle", typeof(string), typeof(TabiTextCell), null);

        public string UITableViewStyle
        {
            get { return (string)GetValue(UITableViewStyleProperty); }
            set { SetValue(UITableViewStyleProperty, value); }
        }

        public static readonly BindableProperty UITableViewCellAccessoryProperty =
            BindableProperty.Create("UITableViewCellAccessory", typeof(string), typeof(TabiTextCell), null);

        public string UITableViewCellAccessory
        {
            get { return (string)GetValue(UITableViewCellAccessoryProperty); }
            set { SetValue(UITableViewCellAccessoryProperty, value); }
        }
    }
}


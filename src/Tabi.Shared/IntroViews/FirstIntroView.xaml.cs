using Xamarin.Forms;

namespace Tabi.Shared.IntroViews
{
    public partial class FirstIntroView : ContentView
    {
        public FirstIntroView()
        {
            InitializeComponent();
        }


        public FirstIntroView(Color c)
        {
            InitializeComponent();
            BackgroundColor = c;
        }
    }
}

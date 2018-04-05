using System;
using Xamarin.Forms;

namespace Tabi
{
    public partial class TabiRaisedButton : ContentView
    {
        public TabiRaisedButton()
        {
            InitializeComponent();
        }

        void OnTapGestureRecognizerTapped(object sender, EventArgs args)
        {
            frame.BackgroundColor = Color.FromHex("#42A5F5");

            Log.Debug("ON THE CLICK");
        }
    }
}

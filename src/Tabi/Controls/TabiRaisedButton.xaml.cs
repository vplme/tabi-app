using System;
using Tabi.Logging;
using Xamarin.Forms;

namespace Tabi.Controls
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

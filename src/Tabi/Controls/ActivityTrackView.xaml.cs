using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Tabi.Controls
{
    public partial class ActivityTrackView : ContentView
    {
        public ActivityTrackView()
        {
            InitializeComponent();
        }

        async void Handle_Tapped(object sender, System.EventArgs e)
        {
            BackgroundColor = (Color)Application.Current.Resources["TintColor"];
            await Task.Delay(200);
            BackgroundColor = Color.Transparent;
        }
    }
}

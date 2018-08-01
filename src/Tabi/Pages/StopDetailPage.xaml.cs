using Autofac;
using Tabi.Controls;
using Tabi.DataObjects;
using Tabi.Helpers;
using Tabi.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Tabi.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StopDetailPage : ContentPage
    {
        StopDetailViewModel ViewModel => vm ?? (vm = BindingContext as StopDetailViewModel);
        StopDetailViewModel vm;

        public StopDetailPage(StopVisit sv)
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<StopDetailViewModel>(new TypedParameter(typeof(StopVisit), sv), new TypedParameter(typeof(INavigation), Navigation));

            SetupMap();
            SetMapLocation(ViewModel.Name, ViewModel.Latitude, ViewModel.Longitude, ViewModel.StopVisit.Accuracy);
            AdjustListViewStopHeight();
        }

        void SetupMap()
        {
            routeMap.HeightRequest = App.ScreenHeight * 0.30;
            routeMap.ClearMap();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        void AdjustListViewStopHeight()
        {
            var adjust = Xamarin.Forms.Device.RuntimePlatform != Xamarin.Forms.Device.Android ? 1 : -ViewModel.DataItems.Count + 1;
            ListViewStop.HeightRequest = (ViewModel.DataItems.Count * ListViewStop.RowHeight) - adjust;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private void SetMapLocation(string name, double latitude, double longitude, double accuracy)
        {
            var pos = new Xamarin.Forms.Maps.Position(latitude, longitude);

            string labelPin = "Stop";
            if (!string.IsNullOrEmpty(name))
            {
                labelPin = name;
            }

            routeMap.Pins.Add(new Xamarin.Forms.Maps.Pin() { Label = labelPin, Position = pos });
            routeMap.Circles.Add(new Circle()
            {
                Position = new Position(latitude, longitude),
                Radius = accuracy,
                FillColor = System.Drawing.Color.FromArgb(20, Color.Blue),
                LineWidth = 0.0f,
                StrokeColor = Color.Red,
            });
            routeMap.MoveToRegion(MapSpan.FromCenterAndRadius(pos, Distance.FromMeters(300)));
            routeMap.DrawRoute();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            vm = null;
        }

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            // Only run on Android. iOS uses TextCell with Command property.
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
            {
                ICommandable item = e.SelectedItem as ICommandable;
                if (item != null && item.Command != null && item.Command.CanExecute(null))
                {
                    item.Command.Execute(null);
                }
            }

            ((ListView)sender).SelectedItem = null;

        }
    }
}

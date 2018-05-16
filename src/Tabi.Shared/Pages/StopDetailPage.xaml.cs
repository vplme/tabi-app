using Autofac;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.Shared.Model;
using Tabi.Shared.ViewModels;
using Tabi.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Tabi
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StopDetailPage : ContentPage
    {
        StopDetailViewModel ViewModel => vm ?? (vm = BindingContext as StopDetailViewModel);
        StopDetailViewModel vm;

        public StopDetailPage(StopVisit sv)
        {
            Setup();

            ViewModel.Stop = new StopViewModel(sv.Stop);
            ViewModel.Navigation = Navigation;
            SetMapLocation(ViewModel.Stop.Name, ViewModel.Stop.Latitude, ViewModel.Stop.Longitude);
        }

        void Setup()
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<StopDetailViewModel>();
            routeMap.HeightRequest = App.ScreenHeight * 0.30;
            routeMap.ClearMap();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var adjust = Xamarin.Forms.Device.RuntimePlatform != Xamarin.Forms.Device.Android ? 1 : - ViewModel.DataItems.Count + 1;
            ListViewStop.HeightRequest = (ViewModel.DataItems.Count * ListViewStop.RowHeight) - adjust;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ViewModel.UpdateStop();
        }

        private void SetMapLocation(string name, double latitude, double longitude)
        {
            var pos = new Xamarin.Forms.Maps.Position(latitude, longitude);

            string labelPin = "Stop";
            if (!string.IsNullOrEmpty(name))
            {
                labelPin = name;
            }

            routeMap.Pins.Add(new Xamarin.Forms.Maps.Pin() { Label = labelPin, Position = pos });
            routeMap.MoveToRegion(MapSpan.FromCenterAndRadius(pos, Distance.FromMeters(300)));
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
                ListItem item = e.SelectedItem as ListItem;
                if (item != null && item.Command != null && item.Command.CanExecute(null))
                {
                    item.Command.Execute(null);
                }
            }

            ((ListView)sender).SelectedItem = null;

        }
    }
}

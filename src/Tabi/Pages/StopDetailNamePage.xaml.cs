using Autofac;
using Tabi.Controls;
using Tabi.Helpers;
using Tabi.Resx;
using Tabi.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tabi.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StopDetailNamePage : ContentPage
    {
        StopDetailNameViewModel ViewModel => vm ?? (vm = BindingContext as StopDetailNameViewModel);
        StopDetailNameViewModel vm;

        public StopDetailNamePage(StopVisitViewModel stopVisitViewModel)
        {
            InitializeComponent();

            if (Device.RuntimePlatform == Device.iOS)
            {
                ExtendedToolbarItem saveToolbarItem = new ExtendedToolbarItem() { Done = true, Text = AppResources.SaveText };
                saveToolbarItem.SetBinding(ExtendedToolbarItem.CommandProperty, "SaveCommand");
                ToolbarItems.Add(saveToolbarItem);
            }

            ExtendedToolbarItem cancelToolbarItem = new ExtendedToolbarItem() { Left = true, Text = AppResources.CancelText };
            cancelToolbarItem.SetBinding(ExtendedToolbarItem.CommandProperty, "CancelCommand");
            ToolbarItems.Add(cancelToolbarItem);


            BindingContext = App.Container.Resolve<StopDetailNameViewModel>(new TypedParameter(typeof(INavigation), Navigation),
                                                                            new TypedParameter(typeof(StopVisitViewModel), stopVisitViewModel));

            AdjustPossibleStopsListHeight();
        }

        void AdjustPossibleStopsListHeight()
        {
            //var adjust = Xamarin.Forms.Device.RuntimePlatform != Xamarin.Forms.Device.Android ? 1 : -ViewModel.PossibleStops.Count + 1;
            //PossibleStopsList.HeightRequest = (ViewModel.PossibleStops.Count * PossibleStopsList.RowHeight) - adjust;
        }

        protected override void OnDisappearing()
        {
            ViewModel.Disappear();

            base.OnDisappearing();
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

            ListView listView = sender as ListView;
            listView.SelectedItem = null;
        }
    }
}

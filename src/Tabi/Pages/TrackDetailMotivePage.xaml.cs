using Autofac;
using Tabi.Controls;
using Tabi.Resx;
using Tabi.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tabi.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrackDetailMotivePage : ContentPage
    {
        TrackDetailMotiveViewModel ViewModel => vm ?? (vm = BindingContext as TrackDetailMotiveViewModel);
        TrackDetailMotiveViewModel vm;

        public TrackDetailMotivePage(TrackMotiveViewModel motiveViewModel)
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<TrackDetailMotiveViewModel>(new TypedParameter(typeof(TrackMotiveViewModel), motiveViewModel), new TypedParameter(typeof(INavigation), Navigation));

            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
            {
                ExtendedToolbarItem saveToolbarItem = new ExtendedToolbarItem() { Done = true, Text = AppResources.SaveText };
                saveToolbarItem.SetBinding(ExtendedToolbarItem.CommandProperty, "SaveCommand");
                ToolbarItems.Add(saveToolbarItem);
            }

            ExtendedToolbarItem cancelToolbarItem = new ExtendedToolbarItem() { Left = true, Text = AppResources.CancelText };
            cancelToolbarItem.SetBinding(ExtendedToolbarItem.CommandProperty, "CancelCommand");
            ToolbarItems.Add(cancelToolbarItem);

            ViewModel.PossibleMotives.CollectionChanged += (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
            {
                AdjustListViewHeight();
            };

            AdjustListViewHeight();
        }

        void AdjustListViewHeight()
        {
            var adjust = Xamarin.Forms.Device.RuntimePlatform != Xamarin.Forms.Device.Android ? 1 : -ViewModel.PossibleMotives.Count + 1;
            PossibleMotivesListView.HeightRequest = (ViewModel.PossibleMotives.Count * PossibleMotivesListView.RowHeight) - adjust;
        }

        async void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                await ViewModel.OptionSelected((MotiveOptionViewModel)e.SelectedItem);
                ((ListView)sender).SelectedItem = null;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.CheckSave();
        }

        protected override void OnDisappearing()
        {
            ViewModel.Disappear();

            base.OnDisappearing();
        }
    }
}

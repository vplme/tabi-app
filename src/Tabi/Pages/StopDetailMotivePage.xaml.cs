using Autofac;
using Tabi.Controls;
using Tabi.Resx;
using Tabi.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tabi.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StopDetailMotivePage : ContentPage
    {
        StopDetailMotiveViewModel ViewModel => vm ?? (vm = BindingContext as StopDetailMotiveViewModel);
        StopDetailMotiveViewModel vm;

        public StopDetailMotivePage(StopMotiveViewModel motiveViewModel)
        {
            InitializeComponent();

            ExtendedToolbarItem cancelToolbarItem = new ExtendedToolbarItem() { Left = true, Text = AppResources.CancelText };
            cancelToolbarItem.SetBinding(ExtendedToolbarItem.CommandProperty, "CancelCommand");
            ToolbarItems.Add(cancelToolbarItem);

            BindingContext = App.Container.Resolve<StopDetailMotiveViewModel>(new TypedParameter(typeof(StopMotiveViewModel), motiveViewModel), new TypedParameter(typeof(INavigation), Navigation));

            ViewModel.PossibleMotives.CollectionChanged += (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
            {
                AdjustListViewHeight();
            };
        }

        protected override void OnDisappearing()
        {
            ViewModel.Disappear();

            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.CheckSave();
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
    }
}

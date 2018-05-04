using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Tabi.Pages;
using Tabi.Shared.Resx;
using Tabi.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tabi
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityOverviewPage : ContentPage
    {
        ActivityOverviewViewModel ViewModel => vm ?? (vm = BindingContext as ActivityOverviewViewModel);
        ActivityOverviewViewModel vm;
        DateTimeOffset lastLoad;

        public ActivityOverviewPage()
        {
            InitializeComponent();

            BindingContext = App.Container.Resolve<ActivityOverviewViewModel>();

            ViewModel.Navigation = Navigation;
            ViewModel.Title = AppResources.ActivityOverviewPageTitle;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }


        protected async override void OnAppearing()
        {
            base.OnAppearing();

            await UpdateAsync();
        }

        async Task UpdateAsync()
        {
            await ViewModel.UpdateStopVisitsAsync();
            lastLoad = DateTimeOffset.Now;
        }

        async void ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var lastPageType = Navigation.NavigationStack.Last().GetType();
                if (lastPageType != typeof(StopDetailPage) && lastPageType != typeof(TrackDetailPage))
                {
                    ActivityEntry ae = (ActivityEntry)e.SelectedItem;

                    Page page = null;
                    if (ae.ShowStop)
                    {
                        page = new StopDetailPage(ae.StopVisit);
                    }
                    else if (ae.ShowTrack)
                    {
                        page = new TrackDetailPage(ae.Track);
                    }

                    await Navigation.PushAsync(page);
                }

                ((ListView)sender).SelectedItem = null;

            }
        }
    }
}

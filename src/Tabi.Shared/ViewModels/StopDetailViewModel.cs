using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmHelpers;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.Shared.Model;
using Tabi.Shared.Pages;
using Tabi.Shared.Resx;
using Tabi.Shared.ViewModels;
using Xamarin.Forms;

namespace Tabi.ViewModels
{
    public class StopDetailViewModel : ObservableObject
    {
        private readonly IRepoManager _repoManager;

        public StopViewModel Stop { get; set; }

        public ObservableRangeCollection<ListItem> DataItems { get; set; }

        public StopDetailViewModel(IRepoManager repoManager)
        {
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));

            DataItems = new ObservableRangeCollection<ListItem>();

            OpenStopNameCommand = new Command(async () =>
            {
                await OpenPage(new StopDetailNamePage());
            });

            OpenStopMotiveCommand = new Command(async () =>
            {
                await OpenPage(new StopDetailMotivePage());
            });

            SetListData();
        }

        private async Task OpenPage(Page page)
        {
            // On iOS wrap the page in a NavigationPage so we get a NavigationBar for the modalscreen.
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
            {
                page = new NavigationPage(page);
                await Navigation.PushModalAsync(page);
            }
            else
            {
                // On Android use existing NavigationPage/no modal. Since we can't have
                // a back button on the left side easily.
                await Navigation.PushAsync(page);
            }
        }

        private void SetListData()
        {
            DataItems.Clear();

            string nameSubtitle = Stop?.Name ?? AppResources.SetStopNameHint;
            string motiveSubtitle = Stop?.Motive ?? AppResources.SetStopMotiveHint;


            DataItems.Add(new ListItem()
            {
                Name = AppResources.StopNameLabel,
                Subtitle = nameSubtitle,
                Command = OpenStopNameCommand
            });

            DataItems.Add(new ListItem()
            {
                Name = AppResources.StopMotiveLabel,
                Subtitle = motiveSubtitle,
                Command = OpenStopMotiveCommand
            });
        }

        public INavigation Navigation { get; set; }

        public ICommand OpenStopNameCommand { get; set; }
        public ICommand OpenStopMotiveCommand { get; set; }


        private string title;

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                SetProperty(ref title, value);
            }
        }

        public void UpdateStop()
        {
            //_repoManager.StopRepository.Update(Stop);
        }

    }
}

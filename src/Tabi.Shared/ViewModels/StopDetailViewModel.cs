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

        private readonly StopVisit _stopVisit;

        private readonly INavigation _navigation;

        private ListItem _nameListItem;

        private ListItem _motiveListItem;

        public ObservableRangeCollection<ListItem> DataItems { get; private set; }

        public StopDetailViewModel(IRepoManager repoManager, INavigation navigation, StopVisit stopVisit)
        {
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
            _stopVisit = stopVisit ?? throw new ArgumentNullException(nameof(stopVisit));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));

            Motive stopMotive = _repoManager.MotiveRepository.GetByStopId(_stopVisit.StopId);

            // Initialize a new motive since the ViewModel needs one.
            stopMotive = stopMotive ?? new Motive() { StopId = _stopVisit.StopId };

            Motive = new MotiveViewModel(stopMotive);

            StopVisit = new StopVisitViewModel(stopVisit);

            OpenStopNameCommand = new Command(async () =>
            {
                StopDetailNamePage namePage = new StopDetailNamePage(StopVisit);
                await OpenPage(namePage);
            });

            OpenStopMotiveCommand = new Command(async () =>
            {
                StopDetailMotivePage motivePage = new StopDetailMotivePage(Motive);
                await OpenPage(motivePage);
            });

            DataItems = new ObservableRangeCollection<ListItem>();

            _nameListItem = new ListItem()
            {
                Name = AppResources.StopNameLabel,
                Subtitle = StopVisit.Name ?? AppResources.SetStopMotiveHint,
                Command = OpenStopNameCommand
            };

            _motiveListItem = new ListItem()
            {
                Name = AppResources.StopMotiveLabel,
                Subtitle = Motive.Text ?? AppResources.SetStopMotiveHint,
                Command = OpenStopMotiveCommand
            };

            DataItems.Add(_nameListItem);
            DataItems.Add(_motiveListItem);

            Motive.PropertyChanged += MotiveViewModel_PropertyChanged;
            StopVisit.PropertyChanged += StopVisit_PropertyChanged; ;

        }



        public MotiveViewModel Motive { get; private set; }

        public StopVisitViewModel StopVisit { get; private set; }

        public double Latitude
        {
            get => _stopVisit.Stop.Latitude;
        }

        public double Longitude
        {
            get => _stopVisit.Stop.Longitude;
        }

        public string Name
        {
            get => _stopVisit.Stop.Name;
        }

        private async Task OpenPage(Page page)
        {
            // On iOS wrap the page in a NavigationPage so we get a NavigationBar for the modalscreen.
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
            {
                page = new NavigationPage(page);
                await _navigation.PushModalAsync(page);
            }
            else
            {
                // On Android use existing NavigationPage/no modal. Since we can't have
                // a back button on the left side easily.
                await _navigation.PushAsync(page);
            }
        }


        public ICommand OpenStopNameCommand { get; set; }
        public ICommand OpenStopMotiveCommand { get; set; }

        void StopVisit_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                _nameListItem.Subtitle = StopVisit.Name ?? AppResources.SetStopMotiveHint;
            }
        }

        void MotiveViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Text")
            {
                _motiveListItem.Subtitle = Motive.Text ?? AppResources.SetStopMotiveHint;
            }
        }




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

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

            // Find an existing Motive for the current stopvisit.
            Motive stopMotive = _repoManager.MotiveRepository.GetByStopVisitId(_stopVisit.Id);
            // Initialize a new motive since the ViewModel needs one.
            stopMotive = stopMotive ?? new Motive() { StopVisitId = _stopVisit.Id };

            Motive = new StopMotiveViewModel(stopMotive);

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
                Subtitle = StopNameFromString(StopVisit.Name),
                Command = OpenStopNameCommand
            };

            _motiveListItem = new ListItem()
            {
                Name = AppResources.StopMotiveLabel,
                Subtitle = MotiveTextFromString(Motive.Text),
                Command = OpenStopMotiveCommand
            };

            DataItems.Add(_nameListItem);
            DataItems.Add(_motiveListItem);

            Motive.PropertyChanged += MotiveViewModel_PropertyChanged;
            StopVisit.PropertyChanged += StopVisit_PropertyChanged;

        }

        public StopMotiveViewModel Motive { get; private set; }

        public StopVisitViewModel StopVisit { get; private set; }

        public double Latitude
        {
            // Use StopVisit coords if possible. Old databases on phones
            // may only have Stop.StopVisit.Lat/Long coords.
            // Temporary fix, should be removed in the future.
            // 0,0 is valid but in the ocean. Same for longitude
            // TODO Remove in August 2018. 
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator

            get => _stopVisit.Latitude != 0 && _stopVisit.Longitude != 0 ? _stopVisit.Latitude : _stopVisit.Stop.Latitude;
        }

        public double Longitude
        {
            get => _stopVisit.Latitude != 0 && _stopVisit.Longitude != 0 ? _stopVisit.Longitude : _stopVisit.Stop.Longitude;
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
        }

        public string Name
        {
            get => _stopVisit.Stop.Name;
        }

        private async Task OpenPage(Page page)
        {
            page = new NavigationPage(page);
            await _navigation.PushModalAsync(page);
        }


        public ICommand OpenStopNameCommand { get; set; }
        public ICommand OpenStopMotiveCommand { get; set; }

        void StopVisit_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                _nameListItem.Subtitle = StopNameFromString(StopVisit.Name);
            }
        }

        void MotiveViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Text")
            {
                _motiveListItem.Subtitle = MotiveTextFromString(Motive.Text);
            }
        }

        string StopNameFromString(string name)
        {
            return !string.IsNullOrEmpty(name) ? name : AppResources.SetStopNameHint;
        }

        string MotiveTextFromString(string motive)
        {
            return !string.IsNullOrEmpty(motive) ? motive : AppResources.SetStopMotiveHint;
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
    }
}

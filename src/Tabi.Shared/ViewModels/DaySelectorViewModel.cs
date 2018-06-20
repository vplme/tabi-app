using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmHelpers;
using Tabi.DataStorage;
using Tabi.Model;
using Tabi.Shared.Helpers;
using Xamarin.Forms;

namespace Tabi.ViewModels
{
    public class DaySelectorViewModel : ObservableObject
    {

        private ObservableRangeCollection<Day> items = new ObservableRangeCollection<Day>();
        private INavigation _navigation;
        private IRepoManager _repoManager;
        private readonly DateService _dateService;

        public DaySelectorViewModel(INavigation navigation, IRepoManager repoManager, DateService dateService)
        {
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
            _dateService = dateService ?? throw new ArgumentNullException(nameof(dateService));

            DataObjects.StopVisit stopVisit = repoManager.StopVisitRepository.First();

            DateTimeOffset firstDate = DateTime.Today;

            if (stopVisit != null)
            {
                firstDate = stopVisit.BeginTimestamp;
            }

            double days = (DateTime.Today - firstDate.Date).TotalDays;

            for (int i = 0; i <= days; i++)
            {
                DateTime currentDate = DateTime.Today.AddDays(-1 * i);
                Items.Add(new Day() { Time = currentDate });
            }

            CancelCommand = new Command(async () =>
            {
                await _navigation.PopModalAsync();
            });
        }

        public ICommand CancelCommand { get; set; }

        public ObservableRangeCollection<Day> Items
        {
            get
            {
                return items;
            }
            set
            {
                SetProperty(ref items, value);
            }
        }

        public Day SelectedDay
        {
            get
            {
                return _dateService.SelectedDay;

            }
            set
            {
                if (value != null)
                {
                    _dateService.SelectedDay = value;
                }
            }
        }

        public async Task ListSelectedDay(Day day)
        {
            _dateService.SelectedDay = day;
            await _navigation.PopModalAsync();
        }
    }
}

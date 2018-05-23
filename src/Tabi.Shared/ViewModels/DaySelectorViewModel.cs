using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmHelpers;
using Tabi.Model;
using Tabi.Shared.Helpers;
using Tabi.Shared.Model;
using Xamarin.Forms;

namespace Tabi.ViewModels
{
    public class DaySelectorViewModel : ObservableObject
    {

        private ObservableRangeCollection<Day> items = new ObservableRangeCollection<Day>();
        private INavigation _navigation;
        private readonly DateService _dateService;

        public DaySelectorViewModel(INavigation navigation, DateService dateService)
        {
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            _dateService = dateService ?? throw new ArgumentNullException(nameof(dateService));

            for (int i = 0; i < 5; i++)
            {
                Items.Add(new Day() { Time = DateTime.Now.AddDays(-1 * i) });
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

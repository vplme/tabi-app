using System;
using MvvmHelpers;
using Tabi.Model;
using Tabi.Shared.Helpers;
using Tabi.Shared.Model;

namespace Tabi.ViewModels
{
    public class DaySelectorViewModel : ObservableObject
    {

        private ObservableRangeCollection<Day> items = new ObservableRangeCollection<Day>();
        private readonly DateService _dateService;

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

        public DateTime SelectedDate
        {
            get
            {
                return _dateService.SelectedDate;

            }
            set
            {
                _dateService.SelectedDate = value;
            }
        }

        public DaySelectorViewModel(DateService dateService)
        {
            _dateService = dateService ?? throw new ArgumentNullException(nameof(dateService));

            for (int i = 0; i < 5; i++)
            {
                Items.Add(new Day() { Time = DateTime.Now.AddDays(-1 * i) });
            }
        }
    }
}

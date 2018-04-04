using System;
using MvvmHelpers;
using Tabi.Model;
using Tabi.Shared.Model;

namespace Tabi.ViewModels
{
    public class DaySelectorViewModel : ObservableObject
    {

        private ObservableRangeCollection<Day> items = new ObservableRangeCollection<Day>();

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
        public DaySelectorViewModel()
        {
            for (int i = 0; i < 5; i++)
            {
                Items.Add(new Day() { Time = DateTime.Now.AddDays(-1 * i)});
            }
        }
    }
}

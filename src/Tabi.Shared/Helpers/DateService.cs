using System;
using System.ComponentModel;
using Tabi.Model;

namespace Tabi.Shared.Helpers
{
    public class DateService : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public DateService()
        {
            SelectedDay = new Day { Time = DateTime.Now };
        }

        private Day selectedDay;
        public Day SelectedDay
        {
            get
            {
                return selectedDay;
            }
            set
            {
                selectedDay = value;
                OnPropertyChanged("SelectedDay");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

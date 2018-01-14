using System;
using System.ComponentModel;

namespace Tabi.Shared.Helpers
{
    public class DateService : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public DateService()
        {
            selectedDate = DateTime.Now;
        }

        private DateTime selectedDate;
        public DateTime SelectedDate
        {
            get
            {
                return selectedDate;
            }
            set
            {
                selectedDate = value;
                OnPropertyChanged("SelectedDate");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

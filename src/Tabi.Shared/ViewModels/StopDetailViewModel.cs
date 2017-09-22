using System;
using MvvmHelpers;
using Tabi.DataObjects;

namespace Tabi.ViewModels
{
    public class StopDetailViewModel : ObservableObject
    {
        public Stop Stop { get; set; }

        public StopDetailViewModel(StopVisit s)
        {
            Stop = s.Stop;
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

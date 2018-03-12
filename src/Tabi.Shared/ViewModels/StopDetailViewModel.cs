using MvvmHelpers;
using Tabi.DataObjects;

namespace Tabi.ViewModels
{
    public class StopDetailViewModel : ObservableObject
    {
        private Stop stop;

        public Stop Stop
        {
            get => stop;
            set => SetProperty(ref stop, value);
        }
        public StopDetailViewModel()
        {
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

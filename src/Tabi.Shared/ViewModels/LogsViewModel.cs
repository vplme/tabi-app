using System;
namespace Tabi.ViewModels
{
    public class LogsViewModel : BaseViewModel
    {
        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }
    }
}

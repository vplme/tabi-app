using System.ComponentModel;
using System.Runtime.CompilerServices;
using Tabi.Helpers;

namespace Tabi.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public Settings Settings
        {
            get { return Settings.Current; }
        }


        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string name = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;
            changed(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        public void SetProperty<T>(ref T field, T value, [CallerMemberName]string name = "")
        {
            if (value != null && !value.Equals(field))
            {
                field = value;

                var changed = PropertyChanged;
                if (changed == null)
                    return;
                changed(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}

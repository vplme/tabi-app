using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Tabi
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
    }
}

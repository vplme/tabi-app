using System.Windows.Input;
using MvvmHelpers;
using Tabi.Helpers;

namespace Tabi.Model
{
    public class ListItem : ObservableObject, ICommandable
    {
        private string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private string subtitle;
        public string Subtitle
        {
            get => subtitle;
            set => SetProperty(ref subtitle, value);
        }

        private ICommand command;
        public ICommand Command
        {
            get => command;
            set => SetProperty(ref command, value);
        }

        private object parameter;
        public object Parameter
        {
            get => parameter;
            set => SetProperty(ref parameter, value);
        }
    }
}

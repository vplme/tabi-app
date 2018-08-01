using System.Windows.Input;

namespace Tabi.Helpers
{
    public interface ICommandable
    {
        ICommand Command
        {
            get;
        }
    }
}

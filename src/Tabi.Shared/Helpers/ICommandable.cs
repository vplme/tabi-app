using System;
using System.Windows.Input;

namespace Tabi.Shared.Helpers
{
    public interface ICommandable
    {
        ICommand Command
        {
            get;
        }
    }
}

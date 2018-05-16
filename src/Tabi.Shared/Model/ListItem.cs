using System;
using System.Windows.Input;

namespace Tabi.Shared.Model
{
    public class ListItem
    {
        public string Name { get; set; }

        public string Subtitle { get; set; }

        public ICommand Command { get; set; }

        public object Parameter { get; set; }
    }
}

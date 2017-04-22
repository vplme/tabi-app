using System;
using System.Windows.Input;
using MvvmHelpers;

namespace Tabi
{
    public class ActivityEntry : ObservableObject
    {
        public ActivityEntry()
        {
        }

        string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        string time;
        public string Time
        {
            get { return time; }
            set { SetProperty(ref time, value); }
        }

        ICommand stopCommand;
        public ICommand StopCommand
        {
            get { return stopCommand; }
            set { SetProperty(ref stopCommand, value); }
        }

        public Track Track { get; set; }

        public bool ShowTrack => Track != null;

        public bool ShowStop => name != null && !name.Equals("");
    }
}
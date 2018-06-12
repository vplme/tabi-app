﻿using System.Windows.Input;
using MvvmHelpers;
using Tabi.DataObjects;
using Tabi.Shared.Resx;

namespace Tabi
{
    public class ActivityEntry : ObservableObject
    {
        public ActivityEntry()
        {
        }

        StopVisit stopVisit;
        public StopVisit StopVisit
        {
            get { return stopVisit; }
            set { SetProperty(ref stopVisit, value); }
        }


        string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        public string StopName
        {
            get
            {
                string result = AppResources.UnsetStopName;
                if (stopVisit != null && !string.IsNullOrEmpty(stopVisit.Stop.Name) )
                {
                    result = stopVisit.Stop.Name;
                }
                return result;
            }
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

        public bool ShowStop => stopVisit != null;
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MvvmHelpers;

namespace Tabi.ViewModels
{
    public class ActivityOverviewViewModel : ObservableObject
    {
        public ObservableCollection<ActivityEntry> ActivityEntries { get; } = new ObservableCollection<ActivityEntry>();

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

        public ActivityOverviewViewModel()
        {
        }

    }
}

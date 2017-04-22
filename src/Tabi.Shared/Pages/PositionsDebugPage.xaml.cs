using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tabi.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Tabi.DataStorage;
using Tabi.DataObjects;

namespace Tabi
{
    public partial class PositionsDebugPage : ContentPage
    {
        ObservableCollection<PositionEntry> positions = new ObservableCollection<PositionEntry>();
        public PositionsDebugPage()
        {
            InitializeComponent();
            PositionView.ItemsSource = positions;

           
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            IPositionEntryRepository postitionEntryRepo = App.RepoManager.PositionEntryRepository;

            var begin = DateTimeOffset.MinValue;
            var end = DateTimeOffset.MaxValue;

            foreach (PositionEntry p in postitionEntryRepo.FilterPeriodAccuracy(begin, end, 100))
            {
                positions.Add(p);
            }
        }

        void OnSelected(object sender, SelectedItemChangedEventArgs e)
        {
            DisplayAlert("Position", e.SelectedItem.ToString(), "OK");
        }
    }
}

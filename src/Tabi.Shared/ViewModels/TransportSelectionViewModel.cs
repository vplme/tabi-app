using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MvvmHelpers;
using Tabi.Controls;
using Tabi.DataObjects;
using Tabi.Pages;
using Tabi.Shared.Model;
using Xamarin.Forms;
using static Tabi.ViewModels.TrackDetailViewModel;

namespace Tabi.ViewModels
{
    public class TransportSelectionViewModel : ObservableObject
    {
        private SelectableObservableCollection<TransportModeItem> items = new SelectableObservableCollection<TransportModeItem>();

        public SelectableObservableCollection<TransportModeItem> Items
        {
            get
            {
                return items;
            }
            set
            {
                SetProperty(ref items, value);
            }
        }

        public TrackEntry TrackEntry { get; set; }


        public TransportSelectionViewModel(INavigation navigation)
        {
            Items = new SelectableObservableCollection<TransportModeItem>(TransportModeItem.GetPossibleTransportModes());
        }

        public IList<TransportationMode> GetSelectedTransportModes()
        {
            var selectedItems = Items.Where(x => x.IsSelected).Select(x => x.Data);
            IList<TransportationMode> selectedModes = TransportModeItem.GetTransportModeEnums(selectedItems);



            return selectedModes;
        }

        public void FinishedTransportSelection()
        {

            //get track with transportation, remoce the transportmodes
            var trackWithTransportationMode = App.RepoManager.TrackEntryRepository.GetWithChildren(TrackEntry.Id);
            trackWithTransportationMode.TransportationModes = null;

            //convert to transportationmodeentry objects
            List<TransportationModeEntry> transportationModes = new List<TransportationModeEntry>();

            foreach (var transportationMode in GetSelectedTransportModes())
            {
                transportationModes.Add(new TransportationModeEntry()
                {
                    Mode = transportationMode
                });
            }

            trackWithTransportationMode.TransportationModes = transportationModes;

            // update the track new transportationmodes
            App.RepoManager.TrackEntryRepository.Update(trackWithTransportationMode);
        }
    }
}

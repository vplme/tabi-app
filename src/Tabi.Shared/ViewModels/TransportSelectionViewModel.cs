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
            //convert to transportationmodeentry objects
            var selectedTransportModes = GetSelectedTransportModes();

            var Timestamp = DateTimeOffset.Now;

            TransportationModeEntry selectedTransportationModeEntry = new TransportationModeEntry
            {
                Timestamp = Timestamp,
                TrackId = TrackEntry.Id
            };

            foreach (var transportMode in selectedTransportModes)
            {
                switch (transportMode)
                {
                    case TransportationMode.Walk:
                        selectedTransportationModeEntry.Walk = true;
                        break;
                    case TransportationMode.Run:
                        selectedTransportationModeEntry.Run = true;
                        break;
                    case TransportationMode.MobilityScooter:
                        selectedTransportationModeEntry.MobilityScooter = true;
                        break;
                    case TransportationMode.Car:
                        selectedTransportationModeEntry.Car = true;
                        break;
                    case TransportationMode.Bike:
                        selectedTransportationModeEntry.Bike = true;
                        break;
                    case TransportationMode.Moped:
                        selectedTransportationModeEntry.Moped = true;
                        break;
                    case TransportationMode.Scooter:
                        selectedTransportationModeEntry.Scooter = true;
                        break;
                    case TransportationMode.Motorcycle:
                        selectedTransportationModeEntry.Motorcycle = true;
                        break;
                    case TransportationMode.Train:
                        selectedTransportationModeEntry.Train = true;
                        break;
                    case TransportationMode.Subway:
                        selectedTransportationModeEntry.Subway = true;
                        break;
                    case TransportationMode.Tram:
                        selectedTransportationModeEntry.Tram = true;
                        break;
                    case TransportationMode.Bus:
                        selectedTransportationModeEntry.Bus = true;
                        break;
                    case TransportationMode.Other:
                        selectedTransportationModeEntry.Other = true;
                        break;
                    default:
                        break;
                }

                //insert in database
                App.RepoManager.TransportationModeRepository.Add(selectedTransportationModeEntry);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MvvmHelpers;
using Tabi.Controls;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.Pages;
using Tabi.Shared.Model;
using Xamarin.Forms;
using static Tabi.ViewModels.TrackDetailViewModel;

namespace Tabi.ViewModels
{
    public class TransportSelectionViewModel : ObservableObject
    {
        private readonly IRepoManager _repoManager;
        private readonly INavigation _navigation;
        private readonly TrackEntry _trackEntry;

        public TransportSelectionViewModel(IRepoManager repoManager, INavigation navigation, TrackEntry trackEntry)
        {
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            _trackEntry = trackEntry ?? throw new ArgumentNullException(nameof(trackEntry));

            Items = new SelectableObservableCollection<TransportModeItem>();

            SaveCommand = new Command(async () =>
            {
                FinishedTransportSelection();
                await _navigation.PopModalAsync();

            });

            CancelCommand = new Command(async () =>
            {
                await _navigation.PopModalAsync();
            });

            SetActualTransportModes();
        }

        public ICommand SaveCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        private void SetActualTransportModes()
        {
            IList<TransportModeItem> transports = TransportModeItem.GetPossibleTransportModes();
            TransportationModeEntry trackEntry = _repoManager.TransportationModeRepository.GetLastWithTrackEntry(_trackEntry.Id);

            foreach (TransportModeItem mi in transports)
            {
                items.Add(mi, CheckModeIsInTransportEntry(mi.Mode, trackEntry));
            }
        }

        private bool CheckModeIsInTransportEntry(TransportationMode mode, TransportationModeEntry entry)
        {
            bool result = false;

            if (entry != null)
            {
                switch (mode)
                {
                    case TransportationMode.Bus:
                        result = entry.Bus;
                        break;
                    case TransportationMode.Bike:
                        result = entry.Bike;
                        break;
                    case TransportationMode.Car:
                        result = entry.Car;
                        break;
                    case TransportationMode.MobilityScooter:
                        result = entry.MobilityScooter;
                        break;
                    case TransportationMode.Moped:
                        result = entry.Moped;
                        break;
                    case TransportationMode.Motorcycle:
                        result = entry.Motorcycle;
                        break;
                    case TransportationMode.Other:
                        result = entry.Other;
                        break;
                    case TransportationMode.Run:
                        result = entry.Run;
                        break;
                    case TransportationMode.Scooter:
                        result = entry.Scooter;
                        break;
                    case TransportationMode.Subway:
                        result = entry.Subway;
                        break;
                    case TransportationMode.Train:
                        result = entry.Train;
                        break;
                    case TransportationMode.Tram:
                        result = entry.Tram;
                        break;
                    case TransportationMode.Walk:
                        result = entry.Walk;
                        break;
                }
            }
            return result;
        }

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
                TrackId = _trackEntry.Id
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
            }

            _repoManager.TransportationModeRepository.Add(selectedTransportationModeEntry);
        }
    }
}

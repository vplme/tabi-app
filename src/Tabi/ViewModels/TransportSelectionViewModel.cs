using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmHelpers;
using Tabi.Controls.MultiSelectListView;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.Model;
using Tabi.Pages;
using Xamarin.Forms;

namespace Tabi.ViewModels
{
    public class TransportSelectionViewModel : ObservableObject
    {
        private readonly IRepoManager _repoManager;
        private readonly INavigation _navigation;
        private readonly TrackEntry _trackEntry;
        private readonly ITransportationModeConfiguration _transportConfig;

        public TransportSelectionViewModel(IRepoManager repoManager, INavigation navigation, ITransportationModeConfiguration transportConfiguration, TrackEntry trackEntry)
        {
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            _trackEntry = trackEntry ?? throw new ArgumentNullException(nameof(trackEntry));
            _transportConfig = transportConfiguration ?? throw new ArgumentNullException(nameof(transportConfiguration));

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

            CustomTransportModeCommand = new Command(async () =>
            {
                await _navigation.PushModalAsync(new NavigationPage(new CustomTransportSelectionPage(Items)));
            });

            SetActualTransportModes();
        }

        public ITransportationModeConfiguration TransportModeConfiguration => _transportConfig;

        public ICommand SaveCommand { get; set; }

        public ICommand CustomTransportModeCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        private void SetActualTransportModes()
        {
            List<TransportModeItem> transports = TransportModeItem.GetPossibleTransportModes(_transportConfig);
            TransportationModeEntry trackEntry = _repoManager.TransportationModeRepository.GetLastWithTrackEntry(_trackEntry.Id);

            IList<string> activeModes = new List<string>();
            if (trackEntry?.ActiveModes != null)
            {
                activeModes = trackEntry.ActiveModes.Split(',').ToList();
            }

            // Add the default transport modes, while removing them from (ref) activeModes
            foreach (TransportModeItem mi in transports)
            {
                items.Add(mi, CheckModeIsInTransportEntry(mi.Id, ref activeModes));
            }

            // Add any remaining activemodes
            foreach (string aMode in activeModes)
            {
                items.Add(new TransportModeItem() { Id = aMode, Name = aMode }, true);
            }
        }

        private bool CheckModeIsInTransportEntry(string id, ref IList<string> activeModes)
        {
            bool result = false;
            if (activeModes.Contains(id))
            {
                activeModes.Remove(id);
                result = true;
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

        public IEnumerable<string> GetSelectedTransportModes()
        {
            var selectedItems = Items.Where(x => x.IsSelected).Select(x => x.Data);
            //IList<string> selectedModes = TransportModeItem.GetTransportModeEnums(selectedItems);


            return selectedItems.Select(x => x.Id);
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

            StringBuilder stringBuilder = new StringBuilder();


            foreach (var sel in selectedTransportModes)
            {
                if (stringBuilder.Length != 0)
                {
                    stringBuilder.Append(",");
                }
                stringBuilder.Append(sel);
            }

            selectedTransportationModeEntry.ActiveModes = stringBuilder.ToString();

            _repoManager.TransportationModeRepository.Add(selectedTransportationModeEntry);
        }
    }
}
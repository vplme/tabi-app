using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.Shared.Helpers;
using Tabi.Shared.Resx;
using Xamarin.Forms;

namespace Tabi.Shared.ViewModels
{
    public class StopDetailNameViewModel : BaseViewModel
    {
        private readonly IRepoManager _repoManager;
        private readonly INavigation _navigation;
        private readonly StopVisitViewModel _stopVisitViewModel;
        private readonly UserInterfaceConfiguration _userInterfaceConfiguration;
        private bool saved;
        private bool nameChanged;
        private Stop useExistingStop;

        public StopDetailNameViewModel(IRepoManager repoManager, INavigation navigation, UserInterfaceConfiguration userInterfaceConfiguration, StopVisitViewModel stopVisitViewModel)

        {
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            _stopVisitViewModel = stopVisitViewModel ?? throw new ArgumentNullException(nameof(stopVisitViewModel));
            _userInterfaceConfiguration = userInterfaceConfiguration ?? throw new ArgumentNullException(nameof(userInterfaceConfiguration));

            SaveCommand = new Command(async () =>
            {
                saved = true;
                Stop newStop = StopVisit.SaveViewModelToStop();
                if (useExistingStop != null)
                {
                    stopVisitViewModel.StopVisit.StopId = useExistingStop.Id; ;
                }
                else if (ChangeAll && nameChanged)
                {
                    newStop.Id = stopVisitViewModel.StopVisit.StopId;
                    stopVisitViewModel.StopVisit.Stop = newStop;
                    repoManager.StopRepository.Update(newStop);
                }
                else if (nameChanged)
                {
                    // Add to repo and newStop will have an ID set
                    repoManager.StopRepository.Add(newStop);
                    stopVisitViewModel.StopVisit.StopId = newStop.Id;
                    stopVisitViewModel.StopVisit.Stop = newStop;
                }

                repoManager.StopVisitRepository.Update(stopVisitViewModel.StopVisit);

                await PopPageAsync();
            });
            CancelCommand = new Command(async () =>
            {
                StopVisit.ResetViewModel();
                await PopPageAsync();
            });

            ChangeAllVisible = ShouldChangeAllBeVisible();
            if (ChangeAllVisible)
            {
                changeAll = true;
            }

            StopVisit.PropertyChanged += NameChanged;


            if (_userInterfaceConfiguration.StopNameReplaceAllEnabled)
            {
                // Only show the list of possible nearby stops if there are more than 1 stops nearby.
                int existingStopVisits = _repoManager.StopVisitRepository.MatchingStop(stopVisitViewModel.StopVisit.Stop).ToList().Count;
                if (existingStopVisits > 1)
                {
                    changeAllVisible = true;
                    changeAll = true;
                }
            }

            if (_userInterfaceConfiguration.SuggestPossibleNearbyStopsEnabled)
            {
                PossibleStops = ListNearbyStops(stopVisitViewModel, _userInterfaceConfiguration.SuggestPossibleNearbyStopsDistance);
                PossibleStopsVisible = PossibleStops.Count >= _userInterfaceConfiguration.SuggestPossibleNearbyStopsCount;
            }

        }

        private List<StopOption> ListNearbyStops(StopVisitViewModel stopVisitViewModel, double radius)
        {
            // Populate possible nearby stops if needed.
            double lat = stopVisitViewModel.StopVisit.Latitude;
            double lon = stopVisitViewModel.StopVisit.Longitude;
            List<Stop> nearestStops = _repoManager.StopRepository.NearestStops(lat, lon, radius).ToList();
            List<StopOption> options = new List<StopOption>();
            foreach (Stop ns in nearestStops)
            {
                // Name of stop should not be empty and it should be not the current selected stop.
                if (!string.IsNullOrWhiteSpace(ns.Name) && ns.Id != _stopVisitViewModel.StopVisit.StopId)
                {
                    StopOption stopOption = new StopOption(ns);
                    stopOption.Command = new Command(async () =>
                    {
                        bool result = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync(
                            string.Format(AppResources.ReplaceWithExistingStopText, ns.Name),
                            AppResources.ReplaceWithExistingStopTitle,
                            AppResources.ReplaceWithExistingStopOkText,
                            AppResources.CancelText);

                        if (result)
                        {
                            ChangeAllVisible = false;
                            useExistingStop = ns;

                            //Unsub to make sure this name change doesn't reset things
                            StopVisit.PropertyChanged -= NameChanged;

                            // Event handler to check if the user wants to manually change the name
                            stopVisitViewModel.Name = ns.Name;
                            StopVisit.PropertyChanged += NameChanged;
                        }

                    });

                    stopOption.Distance = Util.DistanceBetween(lat, lon, ns.Latitude, ns.Longitude);
                    options.Add(stopOption);
                }
            }

            return options.OrderBy(ps => ps.Distance).ToList();

        }

        private async Task PopPageAsync()
        {
            await _navigation.PopModalAsync();
        }

        private bool ShouldChangeAllBeVisible()
        {
            bool result = false;
            if (_userInterfaceConfiguration.StopNameReplaceAllEnabled)
            {
                // Only show the list of possible nearby stops if there are more than 1 stops nearby.
                int existingStopVisits = _repoManager.StopVisitRepository.MatchingStop(_stopVisitViewModel.StopVisit.Stop).ToList().Count;
                result = existingStopVisits > 1;
            }

            return result;
        }

        public StopVisitViewModel StopVisit { get => _stopVisitViewModel; }

        private bool changeAll;

        public bool ChangeAll { get => changeAll; set => SetProperty(ref changeAll, value); }

        private bool changeAllVisible;

        public bool ChangeAllVisible { get => changeAllVisible; set => SetProperty(ref changeAllVisible, value); }

        private bool possibleStopsVisible;
        public bool PossibleStopsVisible { get => possibleStopsVisible; set => SetProperty(ref possibleStopsVisible, value); }

        public IList<StopOption> PossibleStops { get; set; } = new List<StopOption>();

        public ICommand SaveCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        public void Disappear()
        {
            if (!saved)
            {
                StopVisit.ResetViewModel();
            }
        }

        void NameChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StopVisit.Name))
            {
                // Reset use existing stop if the user has changed the stop name
                // after selecting a stop.
                ChangeAllVisible = ShouldChangeAllBeVisible();
                nameChanged = true;
                useExistingStop = null;
                StopVisit.PropertyChanged -= NameChanged;
            }
        }
    }

    public class StopOption : ICommandable
    {
        private Stop _stop;

        public StopOption(Stop stop)
        {
            _stop = stop ?? throw new ArgumentNullException(nameof(stop));
        }

        public double Distance { get; set; }

        public ICommand Command { get; set; }

        public string Name { get => _stop.Name; }

        public string Subtitle
        {
            get => $"{Math.Round(Distance, 0)}m";
        }

    }
}

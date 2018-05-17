using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Xamarin.Forms;

namespace Tabi.Shared.ViewModels
{
    public class StopDetailNameViewModel : BaseViewModel
    {
        private readonly IRepoManager _repoManager;
        private readonly INavigation _navigation;
        private readonly StopVisitViewModel _stopVisitViewModel;

        public StopDetailNameViewModel(IRepoManager repoManager, INavigation navigation, StopVisitViewModel stopVisitViewModel)

        {
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            _stopVisitViewModel = stopVisitViewModel ?? throw new ArgumentNullException(nameof(stopVisitViewModel));

            SaveCommand = new Command(async () =>
            {
                Stop newStop = StopVisit.SaveViewModelToStop();

                repoManager.StopRepository.Add(newStop);

                StopVisit sv = stopVisitViewModel.StopVisit;
                sv.StopId = newStop.Id;
                sv.Stop = newStop;

                repoManager.StopVisitRepository.Update(sv);

                await PopPageAsync();
            });
            CancelCommand = new Command(async () =>
            {
                StopVisit.ResetViewModel();
                await PopPageAsync();
            });

        }

        private async Task PopPageAsync()
        {
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
            {
                await _navigation.PopModalAsync();
            }
            else
            {
                await _navigation.PopAsync();
            }
        }

        public StopVisitViewModel StopVisit { get => _stopVisitViewModel; }

        public ICommand SaveCommand { get; set; }

        public ICommand CancelCommand { get; set; }
    }
}

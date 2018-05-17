using System;
using System.Threading.Tasks;
using System.Windows.Input;
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
            if (Device.RuntimePlatform == Device.iOS)
            {
                await Navigation.PopModalAsync();
            }
            else
            {
                await Navigation.PopAsync();
            }
        }

        public StopVisitViewModel StopVisit { get => _stopVisitViewModel; }

        public INavigation Navigation { get; set; }

        public ICommand SaveCommand { get; set; }

        public ICommand CancelCommand { get; set; }
    }
}

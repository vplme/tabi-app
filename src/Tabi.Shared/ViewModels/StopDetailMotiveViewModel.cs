using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Xamarin.Forms;

namespace Tabi.Shared.ViewModels
{
    public class StopDetailMotiveViewModel : BaseViewModel
    {
        private readonly IRepoManager _repoManager;
        private readonly INavigation _navigation;


        public StopDetailMotiveViewModel(IRepoManager repoManager, INavigation navigation, MotiveViewModel motiveViewModel)
        {
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
            Motive = motiveViewModel ?? throw new ArgumentNullException(nameof(motiveViewModel));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));

            SaveCommand = new Command(async () =>
            {
                await PopPageAsync();
                Motive newMotive = Motive.SaveViewModelToModel();
                repoManager.MotiveRepository.Add(newMotive);
            });
            CancelCommand = new Command(async () =>
            {
                Motive.ResetViewModel();
                await PopPageAsync();
            });
        }

        public MotiveViewModel Motive { get; private set; }

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

        public ICommand SaveCommand { get; set; }

        public ICommand CancelCommand { get; set; }

    }
}

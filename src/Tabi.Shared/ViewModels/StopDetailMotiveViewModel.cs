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
        private bool saved;

        public StopDetailMotiveViewModel(IRepoManager repoManager, INavigation navigation, MotiveViewModel motiveViewModel)
        {
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
            Motive = motiveViewModel ?? throw new ArgumentNullException(nameof(motiveViewModel));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));

            SaveCommand = new Command(async () =>
            {
                saved = true;
                Motive newMotive = Motive.SaveViewModelToModel();
                repoManager.MotiveRepository.Add(newMotive);

                await PopPageAsync();

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
            await _navigation.PopModalAsync();
        }

        public ICommand SaveCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        public void Disappear()
        {
            if (!saved)
            {
                Motive.ResetViewModel();
            }
        }
    }
}

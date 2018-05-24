using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Xamarin.Forms;

namespace Tabi.Shared.ViewModels
{
    public class TrackDetailMotiveViewModel : BaseViewModel
    {
        private readonly IRepoManager _repoManager;
        private readonly INavigation _navigation;
        private bool saved;

        public TrackDetailMotiveViewModel(IRepoManager repoManager, INavigation navigation, TrackMotiveViewModel motiveViewModel)
        {
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
            Motive = motiveViewModel ?? throw new ArgumentNullException(nameof(motiveViewModel));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));

            SaveCommand = new Command(async () =>
            {
                saved = true;
                Motive newMotive = Motive.SaveViewModelToModel();
                repoManager.MotiveRepository.Add(newMotive);

                await _navigation.PopModalAsync();

            });
            CancelCommand = new Command(async () =>
            {
                Motive.ResetViewModel();
                await _navigation.PopModalAsync();
            });
        }

        public TrackMotiveViewModel Motive { get; private set; }

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

using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmHelpers;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.Shared.Pages;
using Tabi.Shared.Resx;
using Xamarin.Forms;

namespace Tabi.Shared.ViewModels
{
    public class StopDetailMotiveViewModel : DetailMotiveViewModel
    {


        public StopDetailMotiveViewModel(IRepoManager repoManager,
                                         INavigation navigation,
                                         TabiConfiguration configuration,
                                         StopMotiveViewModel motiveViewModel,
                                         MotiveSelectionViewModel motiveSelectionViewModel) : base(repoManager, navigation, configuration, motiveSelectionViewModel)
        {
            Motive = motiveViewModel ?? throw new ArgumentNullException(nameof(motiveViewModel));

            SetupMotives();
        }

        public StopMotiveViewModel Motive { get; private set; }

        protected override void ResetViewModel()
        {
            Motive.ResetViewModel();
        }

        protected override bool SaveViewModel()
        {
            if (_motiveSelectionViewModel.ShouldSave)
            {
                saved = true;
                Motive.Text = _motiveSelectionViewModel.SelectedMotiveOption.Id;
                Motive newMotive = Motive.SaveViewModelToModel();
                _repoManager.MotiveRepository.Add(newMotive);
            }

            return _motiveSelectionViewModel.ShouldSave;
        }

        protected override string MotiveText => Motive?.Text;

    }
}

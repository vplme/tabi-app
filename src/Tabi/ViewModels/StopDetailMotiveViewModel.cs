using System;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Xamarin.Forms;

namespace Tabi.ViewModels
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

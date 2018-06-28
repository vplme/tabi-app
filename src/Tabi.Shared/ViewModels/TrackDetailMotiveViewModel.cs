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
    public class TrackDetailMotiveViewModel : DetailMotiveViewModel
    {

        public TrackDetailMotiveViewModel(IRepoManager repoManager,
                                          INavigation navigation,
                                          TabiConfiguration configuration,
                                          TrackMotiveViewModel motiveViewModel,
                                          MotiveSelectionViewModel motiveSelectionViewModel) : base(repoManager, navigation, configuration, motiveSelectionViewModel)
        {
            Motive = motiveViewModel ?? throw new ArgumentNullException(nameof(motiveViewModel));
            SetupMotives();
        }

        public TrackMotiveViewModel Motive { get; private set; }

        protected override string MotiveText => Motive?.Text;

        protected override void ResetViewModel()
        {
            if (_motiveSelectionViewModel.ShouldSave)
            {
                saved = true;
                Motive.Text = _motiveSelectionViewModel.SelectedMotiveOption.Id;
                Motive newMotive = Motive.SaveViewModelToModel();
                _repoManager.MotiveRepository.Add(newMotive);
            }
        }

        protected override void SaveViewModel()
        {
            Motive.ResetViewModel();
        }
    }

    public class MotiveOptionViewModel : ObservableObject
    {
        string id;
        public string Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        string text;
        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }

        bool selected;
        public bool Selected
        {
            get => selected;
            set => SetProperty(ref selected, value);
        }
    }
}

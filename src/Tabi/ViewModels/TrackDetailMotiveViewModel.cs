using System;
using MvvmHelpers;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Xamarin.Forms;

namespace Tabi.ViewModels
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

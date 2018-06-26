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
    public class StopDetailMotiveViewModel : BaseViewModel
    {
        private readonly IRepoManager _repoManager;
        private readonly INavigation _navigation;
        private readonly MotiveSelectionViewModel _motiveSelectionViewModel;
        private MotiveOptionViewModel CustomMotiveOption = new MotiveOptionViewModel();

        private bool saved;

        public StopDetailMotiveViewModel(IRepoManager repoManager,
                                         INavigation navigation,
                                         TabiConfiguration configuration,
                                         StopMotiveViewModel motiveViewModel,
                                         MotiveSelectionViewModel motiveSelectionViewModel)
        {
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
            Motive = motiveViewModel ?? throw new ArgumentNullException(nameof(motiveViewModel));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            _motiveSelectionViewModel = motiveSelectionViewModel ?? throw new ArgumentNullException(nameof(motiveSelectionViewModel));

            SaveCommand = new Command(async () =>
            {
                if (motiveSelectionViewModel.ShouldSave)
                {
                    saved = true;
                    Motive.Text = motiveSelectionViewModel.SelectedMotiveOption.Id;
                    Motive newMotive = Motive.SaveViewModelToModel();
                    repoManager.MotiveRepository.Add(newMotive);
                }
                await _navigation.PopModalAsync();

            });
            CancelCommand = new Command(async () =>
            {
                Motive.ResetViewModel();
                await PopPageAsync();
            });

            _motiveSelectionViewModel.PropertyChanged += _motiveSelectionViewModel_PropertyChanged;

            PossibleMotives = new ObservableRangeCollection<MotiveOptionViewModel>();

            // Setup possible motives
            foreach (MotiveOption mo in configuration.Motive.Options)
            {
                string translatedText = typeof(AppResources).GetProperty($"{mo.Id}MotiveText")?.GetValue(null) as string;
                string text = translatedText ?? mo.Text;
                MotiveOptionViewModel motive = new MotiveOptionViewModel() { Text = text, Id = mo.Id };
                if (motive.Id == Motive.Text)
                {
                    motive.Selected = true;
                    motiveSelectionViewModel.SelectedMotiveOption = motive;
                }
                PossibleMotives.Add(motive);
            }

            foreach (MotiveOption mo in configuration.Motive.OtherOptions)
            {
                string translatedText = typeof(AppResources).GetProperty($"{mo.Id}MotiveText")?.GetValue(null) as string;
                string text = translatedText ?? mo.Text;
                if (mo.Id == Motive.Text)
                {
                    CustomMotiveOption = new MotiveOptionViewModel() { Text = text, Id = mo.Id };
                    motiveSelectionViewModel.SelectedMotiveOption = CustomMotiveOption;
                    CustomMotiveOption.Selected = true;
                    motiveSelectionViewModel.CustomMotive = true;

                    break;
                }
            }
        }

        ObservableRangeCollection<MotiveOptionViewModel> possibleMotives;

        public ObservableRangeCollection<MotiveOptionViewModel> PossibleMotives
        {
            get => possibleMotives;
            set => SetProperty(ref possibleMotives, value);
        }

        public StopMotiveViewModel Motive { get; private set; }

        private async Task PopPageAsync()
        {
            await _navigation.PopModalAsync();
        }

        public ICommand SaveCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        public void OptionSelected(MotiveOptionViewModel motive)
        {
            if (motive.Id == "Other")
            {
                if (_motiveSelectionViewModel.SelectedMotiveOption != null)
                {
                    _motiveSelectionViewModel.SelectedMotiveOption.Selected = false;
                    _motiveSelectionViewModel.CustomMotive = false;
                }

                _navigation.PushAsync(new SearchMotivePage(_motiveSelectionViewModel));
                _motiveSelectionViewModel.SelectedMotiveOption = null;

            }
            else if (_motiveSelectionViewModel.SelectedMotiveOption != motive)
            {
                _motiveSelectionViewModel.ShouldSave = true;
                _motiveSelectionViewModel.CustomMotive = false;

                if (_motiveSelectionViewModel.SelectedMotiveOption != null)
                {
                    // Set previously selected motive to false
                    _motiveSelectionViewModel.SelectedMotiveOption.Selected = false;
                }
                motive.Selected = true;
                _motiveSelectionViewModel.SelectedMotiveOption = motive;
            }
            else
            {
                // Clicked the same motive twice; reset selected
                _motiveSelectionViewModel.CustomMotive = false;
                _motiveSelectionViewModel.SelectedMotiveOption.Selected = false;
                _motiveSelectionViewModel.SelectedMotiveOption = null;
                _motiveSelectionViewModel.ShouldSave = false;
            }
        }

        public void Disappear()
        {
            if (!saved)
            {
                Motive.ResetViewModel();
            }
        }

        void _motiveSelectionViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_motiveSelectionViewModel.CustomMotive))
            {
                if (_motiveSelectionViewModel.CustomMotive)
                {
                    CustomMotiveOption.Text = _motiveSelectionViewModel.SelectedMotiveOption.Text;
                    CustomMotiveOption.Id = _motiveSelectionViewModel.SelectedMotiveOption.Id;
                    CustomMotiveOption.Selected = true;

                    PossibleMotives.Add(CustomMotiveOption);
                }
                else
                {
                    PossibleMotives.Remove(CustomMotiveOption);
                }
            }
        }
    }
}

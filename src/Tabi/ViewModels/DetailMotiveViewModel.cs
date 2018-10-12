using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmHelpers;
using Tabi.DataStorage;
using Tabi.Pages;
using Tabi.Resx;
using Xamarin.Forms;

namespace Tabi.ViewModels
{
    public abstract class DetailMotiveViewModel : BaseViewModel
    {

        protected readonly IRepoManager _repoManager;
        protected readonly ITabiConfiguration _configuration;

        protected readonly INavigation _navigation;
        protected readonly MotiveSelectionViewModel _motiveSelectionViewModel;
        protected MotiveOptionViewModel CustomMotiveOption = new MotiveOptionViewModel();

        protected bool saved;

        protected DetailMotiveViewModel(IRepoManager repoManager,
                                         INavigation navigation,
                                         ITabiConfiguration configuration,
                                     MotiveSelectionViewModel motiveSelectionViewModel)
        {
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            _motiveSelectionViewModel = motiveSelectionViewModel ?? throw new ArgumentNullException(nameof(motiveSelectionViewModel));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));

            SaveCommand = new Command(Save);
            CancelCommand = new Command(async () =>
            {
                ResetViewModel();
                await PopPageAsync();
            });

            _motiveSelectionViewModel.PropertyChanged += _motiveSelectionViewModel_PropertyChanged;

            PossibleMotives = new ObservableRangeCollection<MotiveOptionViewModel>();
        }

        protected void SetupMotives()
        {
            bool foundMotive = false;
            bool motiveIsSet = !string.IsNullOrEmpty(MotiveText);

            // Setup possible motives
            foreach (MotiveOption mo in _configuration.Motive.Options)
            {
                string translatedText = typeof(AppResources).GetProperty($"{mo.Id}MotiveText")?.GetValue(null) as string;
                string text = translatedText ?? mo.Text;
                MotiveOptionViewModel motive = new MotiveOptionViewModel() { Text = text, Id = mo.Id };
                if (motiveIsSet && motive.Id == MotiveText)
                {
                    motive.Selected = true;
                    _motiveSelectionViewModel.SelectedMotiveOption = motive;
                    foundMotive = true;
                }
                PossibleMotives.Add(motive);
            }

            foreach (MotiveOption mo in _configuration.Motive.OtherOptions)
            {
                string translatedText = typeof(AppResources).GetProperty($"{mo.Id}MotiveText")?.GetValue(null) as string;
                string text = translatedText ?? mo.Text;
                if (motiveIsSet && mo.Id == MotiveText)
                {
                    CustomMotiveOption = new MotiveOptionViewModel() { Text = text, Id = mo.Id };
                    _motiveSelectionViewModel.SelectedMotiveOption = CustomMotiveOption;
                    CustomMotiveOption.Selected = true;
                    _motiveSelectionViewModel.CustomMotive = true;
                    foundMotive = true;
                    break;
                }
            }

            if (motiveIsSet && !foundMotive)
            {
                CustomMotiveOption = new MotiveOptionViewModel() { Text = MotiveText, Id = MotiveText };
                _motiveSelectionViewModel.SelectedMotiveOption = CustomMotiveOption;
                CustomMotiveOption.Selected = true;
                _motiveSelectionViewModel.CustomMotive = true;
                foundMotive = true;
            }
        }

        private async void Save()
        {
            SaveViewModel();
            await _navigation.PopModalAsync();
        }

        public async void CheckSave()
        {
            if (SaveViewModel())
            {
                await _navigation.PopModalAsync();
            }
        }

        protected abstract string MotiveText { get; }
        protected abstract void ResetViewModel();

        protected abstract bool SaveViewModel();

        ObservableRangeCollection<MotiveOptionViewModel> possibleMotives;

        public ObservableRangeCollection<MotiveOptionViewModel> PossibleMotives
        {
            get => possibleMotives;
            set => SetProperty(ref possibleMotives, value);
        }

        private async Task PopPageAsync()
        {
            await _navigation.PopModalAsync();
        }

        public ICommand SaveCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        public async Task OptionSelected(MotiveOptionViewModel motive)
        {
            if (motive.Id == "Other")
            {
                if (_motiveSelectionViewModel.SelectedMotiveOption != null)
                {
                    _motiveSelectionViewModel.SelectedMotiveOption.Selected = false;
                    _motiveSelectionViewModel.CustomMotive = false;
                }

                await _navigation.PushAsync(new SearchMotivePage(_motiveSelectionViewModel));
                _motiveSelectionViewModel.SelectedMotiveOption = null;

            }
            else if (motive == CustomMotiveOption)
            {
                await _navigation.PushAsync(new SearchMotivePage(_motiveSelectionViewModel));
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
                Save();
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
                ResetViewModel();
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

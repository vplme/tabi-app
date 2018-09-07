using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmHelpers;
using Tabi.Resx;
using Xamarin.Forms;

namespace Tabi.ViewModels
{
    public class SearchMotiveViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;
        private readonly TabiConfiguration _configuration;
        private readonly MotiveSelectionViewModel _motiveSelectionViewModel;


        public SearchMotiveViewModel(TabiConfiguration configuration, INavigation navigation, MotiveSelectionViewModel motiveSelectionViewModel)
        {

            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _motiveSelectionViewModel = motiveSelectionViewModel ?? throw new ArgumentNullException(nameof(motiveSelectionViewModel));

            PossibleMotives = new ObservableRangeCollection<MotiveOptionViewModel>();

            CustomMotiveCommand = new Command(async (obj) =>
            {
                _motiveSelectionViewModel.SelectedMotiveOption = new MotiveOptionViewModel()
                {
                    Id = SearchText,
                    Text = SearchText,
                };

                _motiveSelectionViewModel.CustomMotive = true;
                _motiveSelectionViewModel.ShouldSave = true;

                await _navigation.PopAsync();
            });

            SetupMotive(null);
            // Setup possible motives

            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(SearchText))
                {
                    OnPropertyChanged(nameof(CustomMotiveText));
                    SetupMotive(searchText);
                    CustomMotiveVisible = SearchText.Length > 0;
                    OtherMotivesVisible = SearchText.Length > 2;
                }
            };
        }

        public void SetupMotive(string filter)
        {
            PossibleMotives.Clear();
            foreach (MotiveOption mo in _configuration.Motive.OtherOptions)
            {
                string translatedText = typeof(AppResources).GetProperty($"{mo.Id}MotiveText")?.GetValue(null) as string;
                string text = translatedText ?? mo.Text;

                if (filter == null || filter == "" || text.ToLower().Contains(filter.ToLower()))
                {
                    MotiveOptionViewModel motive = new MotiveOptionViewModel() { Text = text, Id = mo.Id };
                    if (motive.Id == _motiveSelectionViewModel.SelectedMotiveOption?.Id)
                    {
                        motive.Selected = true;
                        _motiveSelectionViewModel.SelectedMotiveOption = motive;
                    }
                    PossibleMotives.Add(motive);
                }
            }

        }

        public async Task SelectMotive(MotiveOptionViewModel motiveOptionView)
        {
            _motiveSelectionViewModel.SelectedMotiveOption = motiveOptionView;
            _motiveSelectionViewModel.SelectedMotiveOption.Selected = true;
            _motiveSelectionViewModel.ShouldSave = true;

            _motiveSelectionViewModel.CustomMotive = true;
            await _navigation.PopAsync();
        }

        public ICommand CustomMotiveCommand { get; set; }

        private string searchText;

        public string SearchText
        {
            get => searchText;
            set
            {
                SetProperty(ref searchText, value);
            }
        }

        private bool customMotiveVisible;
        public bool CustomMotiveVisible
        {
            get => customMotiveVisible;
            set => SetProperty(ref customMotiveVisible, value);
        }

        private bool otherMotivesVisible;
        public bool OtherMotivesVisible
        {
            get => otherMotivesVisible;
            set => SetProperty(ref otherMotivesVisible, value);
        }


        ObservableRangeCollection<MotiveOptionViewModel> possibleMotives;

        public ObservableRangeCollection<MotiveOptionViewModel> PossibleMotives
        {
            get => possibleMotives;
            set => SetProperty(ref possibleMotives, value);
        }

        public string CustomMotiveText
        {
            get => string.Format(AppResources.CreateCustomMotive, SearchText);
        }

    }
}

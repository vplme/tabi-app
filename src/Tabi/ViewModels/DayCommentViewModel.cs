using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Tabi.Controls.MultiSelectListView;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.Helpers;
using Tabi.Resx;
using Xamarin.Forms;

namespace Tabi.ViewModels
{
    public class DayCommentViewModel : BaseViewModel
    {
        private readonly IRepoManager _repoManager;
        private readonly DateService _dateService;
        public const string NotesQuestion = "DAY-NOTES";
        public const string TravelQuestion = "DAY-TRAVEL";
        public const string PhoneQuestion = "DAY-PHONE";


        public DayCommentViewModel(INavigation navigation, DateService dateService, IRepoManager repoManager)
        {
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
            _dateService = dateService ?? throw new ArgumentNullException(nameof(dateService));

            SaveCommand = new Command(async () =>
            {
                _repoManager.QuestionRepository.Add(new Question()
                {
                    QuestionKey = NotesQuestion,
                    QuestionDate = _dateService.SelectedDay.Time,
                    Answer = Notes,
                    Timestamp = DateTimeOffset.Now
                });

                _repoManager.QuestionRepository.Add(new Question()
                {
                    QuestionKey = TravelQuestion,
                    QuestionDate = _dateService.SelectedDay.Time,
                    Answer = SaveItemsToString(TravelItems),
                    Timestamp = DateTimeOffset.Now
                });

                _repoManager.QuestionRepository.Add(new Question()
                {
                    QuestionKey = PhoneQuestion,
                    QuestionDate = _dateService.SelectedDay.Time,
                    Answer = SaveItemsToString(PhoneItems),
                    Timestamp = DateTimeOffset.Now
                });

                await navigation.PopAsync();
            });

            CancelCommand = new Command(async () =>
            {
                await navigation.PopAsync();
            });

            TravelItems.Add(new SelectionKeyItem() { Key = "NoSpecial", Item = AppResources.NoSpecialQuestionAnswerLabel });
            TravelItems.Add(new SelectionKeyItem() { Key = "NotApplicable", Item = AppResources.NotApplicableQuestionAnswerLabel });
            TravelItems.Add(new SelectionKeyItem() { Key = "DifferentAddresses", Item = AppResources.DifferentAddressesQuestionAnswerLabel });
            TravelItems.Add(new SelectionKeyItem() { Key = "DifferentTransportModes", Item = AppResources.DifferentTransportModesQuestionAnswerLabel });
            TravelItems.Add(new SelectionKeyItem() { Key = "DifferentTravelTimes", Item = AppResources.DifferentTravelTimesQuestionAnswerLabel });
            TravelItems.Add(new SelectionKeyItem() { Key = "DifferentTravelDuration", Item = AppResources.DifferentTravelDurationQuestionAnswerLabel });
            TravelItems.Add(new SelectionKeyItem() { Key = "DifferentTravelRoutes", Item = AppResources.DifferentTravelRoutesQuestionAnswerLabel });

            PhoneItems.Add(new SelectionKeyItem() { Key = "TRUE", Item = AppResources.YesQuestionAnswerLabel });
            PhoneItems.Add(new SelectionKeyItem() { Key = "FALSE", Item = AppResources.NoQuestionAnswerLabel });

        }

        public void LoadExistingQuestions()
        {
            DateTimeOffset dayDate = _dateService.SelectedDay.Time;

            Question previousNotesQuestion = _repoManager.QuestionRepository.GetLastWithDateTime(NotesQuestion, dayDate);
            if (previousNotesQuestion != null)
            {
                Notes = previousNotesQuestion.Answer;
            }

            Question previousTravelQuestion = _repoManager.QuestionRepository.GetLastWithDateTime(TravelQuestion, dayDate);
            if (previousTravelQuestion != null)
            {
                LoadDataFromString(previousTravelQuestion.Answer, TravelItems);
            }

            Question previousPhoneQuestion = _repoManager.QuestionRepository.GetLastWithDateTime(PhoneQuestion, dayDate);
            if (previousPhoneQuestion != null)
            {
                LoadDataFromString(previousPhoneQuestion.Answer, PhoneItems);
            }
        }

        public string notes;
        public string Notes
        {
            get => notes;
            set => SetProperty(ref notes, value);
        }

        public void LoadDataFromString(string data, SelectableObservableCollection<SelectionKeyItem> items)
        {
            List<string> split = data.Split(',').ToList();
            foreach (SelectableItem<SelectionKeyItem> item in items)
            {
                if (split.Contains(item.Data.Key))
                {
                    item.IsSelected = true;
                }

            }
        }

        public string SaveItemsToString(SelectableObservableCollection<SelectionKeyItem> items)
        {
            StringBuilder sb = new StringBuilder();

            bool firstEntryDone = false;

            foreach (SelectableItem<SelectionKeyItem> item in items)
            {
                if (item.IsSelected)
                {
                    if (firstEntryDone)
                    {
                        sb.Append(',');
                    }

                    sb.Append(item.Data.Key);
                    firstEntryDone = true;
                }
            }

            return sb.ToString();
        }

        private SelectableObservableCollection<SelectionKeyItem> travelItems = new SelectableObservableCollection<SelectionKeyItem>();

        public SelectableObservableCollection<SelectionKeyItem> TravelItems
        {
            get
            {
                return travelItems;
            }
            set
            {
                SetProperty(ref travelItems, value);
            }
        }

        private SelectableObservableCollection<SelectionKeyItem> phoneItems = new SelectableObservableCollection<SelectionKeyItem>();

        public SelectableObservableCollection<SelectionKeyItem> PhoneItems
        {
            get
            {
                return phoneItems;
            }
            set
            {
                SetProperty(ref phoneItems, value);
            }
        }


        public ICommand SaveCommand { get; set; }

        public ICommand CancelCommand { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Windows.Input;
using Tabi.Shared.Resx;
using Xamarin.Forms;

namespace Tabi.Shared.ViewModels
{
    public class TourViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;
        private int currentItem = 0;

        public TourViewModel(INavigation navigation)
        {
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));

            tourItems.Add(new TourItem()
            {
                Title = AppResources.StopTourTitle,
                Text = AppResources.StopTourText,
                Gif = $"stop_50_{AppResources.LangCode}.gif",
            });

            tourItems.Add(new TourItem()
            {
                Title = AppResources.MotiveTourTitle,
                Text = AppResources.MotiveTourText,
                Gif = $"motive_50_{AppResources.LangCode}.gif",
            });

            tourItems.Add(new TourItem()
            {
                Title = AppResources.TransportModeTourTitle,
                Text = AppResources.TransportModeTourText,
                Gif = $"transport_50_{AppResources.LangCode}.gif",
            });

            tourItems.Add(new TourItem()
            {
                Title = AppResources.SettingsTourTitle,
                Text = AppResources.SettingsTourText,
                Gif = $"settings_50_{AppResources.LangCode}.gif",
            });

            NextCommand = new Command(async () =>
            {
                if (currentItem + 1 == tourItems.Count)
                {
                    await _navigation.PopModalAsync();
                }
                else
                {
                    currentItem++;

                    ApplyNewTourItem(tourItems[currentItem]);
                    StatusText = $"{currentItem + 1} / {tourItems.Count}";
                }
            });

            SkipCommand = new Command(async () =>
            {
                await _navigation.PopModalAsync();
            });

            ApplyNewTourItem(tourItems[0]);
            StatusText = $"1 / {tourItems.Count}";
        }

        private void ApplyNewTourItem(TourItem item)
        {
            Title = item.Title;
            Text = item.Text;
            Gif = item.Gif;
        }

        private List<TourItem> tourItems = new List<TourItem>();

        private string statusText;
        public string StatusText
        {
            get => statusText;
            set => SetProperty(ref statusText, value);
        }

        private string gif;
        public string Gif
        {
            get => gif;
            set => SetProperty(ref gif, value);
        }

        private string title;
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        private string text;
        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }


        public ICommand NextCommand { get; set; }

        public ICommand SkipCommand { get; set; }
    }

    public class TourItem
    {
        public string Title { get; set; }

        public string Text { get; set; }

        public string Gif { get; set; }
    }
}

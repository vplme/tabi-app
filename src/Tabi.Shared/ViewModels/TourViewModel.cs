using System;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.AppCenter.Analytics;
using Tabi.Shared.Resx;
using Vpl.Xamarin.VideoPlayer;
using Xamarin.Forms;

namespace Tabi.Shared.ViewModels
{
    public class TourViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;
        private int currentItem = 0;
        bool finishedTour;

        public TourViewModel(INavigation navigation)
        {
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));

            // P3 (screenshot iPhone) converted to --> Srgb
            BackgroundColor = Color.FromHex("#0296BF");

            tourItems.Add(new TourItem()
            {
                Title = AppResources.StopTourTitle,
                Text = AppResources.StopTourText,
                Gif = $"stop_50_{AppResources.LangCode}.gif",
                Video = $"tour_video_stop_name_{AppResources.LangCode}.mp4",
            });

            tourItems.Add(new TourItem()
            {
                Title = AppResources.MotiveTourTitle,
                Text = AppResources.MotiveTourText,
                Gif = $"motive_50_{AppResources.LangCode}.gif",
                Video = $"tour_video_stop_motive_{AppResources.LangCode}.mp4",
            });

            tourItems.Add(new TourItem()
            {
                Title = AppResources.TransportModeTourTitle,
                Text = AppResources.TransportModeTourText,
                Gif = $"transport_50_{AppResources.LangCode}.gif",
                Video = $"tour_video_transportmode_{AppResources.LangCode}.mp4"
            });

            tourItems.Add(new TourItem()
            {
                Title = AppResources.SettingsTourTitle,
                Text = AppResources.SettingsTourText,
                Gif = $"settings_50_{AppResources.LangCode}.gif",
                Video = $"tour_video_settings_{AppResources.LangCode}.mp4"
            });

            NextCommand = new Command(async () =>
            {
                if (!finishedTour && currentItem + 1 == tourItems.Count)
                {
                    finishedTour = true; // Fast users could click twice
                    await _navigation.PopModalAsync();
                    Analytics.TrackEvent("Tour finished");
                }
                else if (!finishedTour)
                {
                    currentItem++;

                    ApplyNewTourItem(tourItems[currentItem]);
                    StatusText = $"{currentItem + 1} / {tourItems.Count}";
                }
            });

            SkipCommand = new Command(async () =>
            {
                Analytics.TrackEvent("Tour skip clicked", new Dictionary<string, string> {
                    { "CurrentItem", currentItem.ToString() }
                });

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
            VideoSource = new ResourceVideoSource() { Path = item.Video };
        }

        private List<TourItem> tourItems = new List<TourItem>();

        private string statusText;
        public string StatusText
        {
            get => statusText;
            set => SetProperty(ref statusText, value);
        }

        private Color backgroundColor;

        public Color BackgroundColor
        {
            get => backgroundColor;
            set => SetProperty(ref backgroundColor, value);
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

        private ResourceVideoSource videoSource;
        public ResourceVideoSource VideoSource
        {
            get => videoSource;
            set => SetProperty(ref videoSource, value);
        }

        public ICommand NextCommand { get; set; }

        public ICommand SkipCommand { get; set; }
    }

    public class TourItem
    {
        public string Title { get; set; }

        public string Text { get; set; }

        public string Gif { get; set; }

        public string Video { get; set; }
    }
}

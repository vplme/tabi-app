using System;
using System.Collections.Generic;
using Autofac;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Tabi.Shared.ViewModels;
using Xamarin.Forms;
using Tabi.Shared.Helpers;
using Xamarin.Forms.Xaml;

namespace Tabi
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TourVideoPage : ContentPage
    {
        TourViewModel ViewModel => vm ?? (vm = BindingContext as TourViewModel);
        TourViewModel vm;

        public TourVideoPage()
        {
            InitializeComponent();

            BindingContext = App.Container.Resolve<TourViewModel>(new TypedParameter(typeof(INavigation), Navigation));

            if (Device.RuntimePlatform == Device.iOS)
            {
                // Video size: 750w x 1000h
                (double widthRequest, double heightRequest) = CalculateVideoSize(750, 1000, 0.85, App.ScreenWidth, App.ScreenHeight);

                VideoPlayer.WidthRequest = widthRequest;
                VideoPlayer.HeightRequest = heightRequest;

                IIOSHelper iOSHelper = App.Container.ResolveOptional<IIOSHelper>();

                var safeInsets = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();

                Thickness margin = new Thickness(0, -20, 0, 0);
                Thickness padding = new Thickness(0, 20, 0, 0);

                if (iOSHelper != null && iOSHelper.IsiPhoneX)
                {
                    margin.Top = 0;
                    padding.Top = 30;
                }
                ContentLayout.Margin = margin;
                ContentLayout.Padding = padding;
            }
            else
            {
                (double widthRequest, double heightRequest) = CalculateVideoSize(916, 1874, 0.70, App.ScreenWidth, App.ScreenHeight);

                VideoPlayer.WidthRequest = widthRequest;
                VideoPlayer.HeightRequest = heightRequest;
            }
        }


        private (double width, double height) CalculateVideoSize(double videoWidth, double videoHeight, double maxHeightRatio, double screenWidth, double screenHeight)
        {
            double widthRequest = screenWidth;
            double heightRequest = (widthRequest / videoWidth) * videoHeight;

            if (heightRequest > screenHeight * maxHeightRatio)
            {
                heightRequest = screenHeight * maxHeightRatio;
                widthRequest = (heightRequest / videoHeight) * videoWidth;
            }

            return (widthRequest, heightRequest);
        }
    }
}

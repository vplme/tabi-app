using System;
using System.Collections.Generic;
using Autofac;
using Tabi.Shared.ViewModels;
using Xamarin.Forms;

namespace Tabi
{
    public partial class TourGifPage : ContentPage
    {
        TourViewModel ViewModel => vm ?? (vm = BindingContext as TourViewModel);
        TourViewModel vm;

        public TourGifPage()
        {
            InitializeComponent();

            image.WidthRequest = App.ScreenWidth * 0.65;
            image.HeightRequest = (image.WidthRequest / 607) * 1080;

            BindingContext = App.Container.Resolve<TourViewModel>(new TypedParameter(typeof(INavigation), Navigation));
        }
    }
}

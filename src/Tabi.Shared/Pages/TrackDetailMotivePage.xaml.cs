using System;
using System.Collections.Generic;
using Autofac;
using Tabi.Shared.Resx;
using Tabi.Shared.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tabi
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrackDetailMotivePage : ContentPage
    {
        TrackDetailMotiveViewModel ViewModel => vm ?? (vm = BindingContext as TrackDetailMotiveViewModel);
        TrackDetailMotiveViewModel vm;

        public TrackDetailMotivePage(TrackMotiveViewModel motiveViewModel)
        {
            InitializeComponent();

            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
            {
                ExtendedToolbarItem saveToolbarItem = new ExtendedToolbarItem() { Done = true, Text = AppResources.SaveText };
                saveToolbarItem.SetBinding(ExtendedToolbarItem.CommandProperty, "SaveCommand");
                ToolbarItems.Add(saveToolbarItem);
            }

            ExtendedToolbarItem cancelToolbarItem = new ExtendedToolbarItem() { Left = true, Text = AppResources.CancelText };
            cancelToolbarItem.SetBinding(ExtendedToolbarItem.CommandProperty, "CancelCommand");
            ToolbarItems.Add(cancelToolbarItem);

            BindingContext = App.Container.Resolve<TrackDetailMotiveViewModel>(new TypedParameter(typeof(TrackMotiveViewModel), motiveViewModel), new TypedParameter(typeof(INavigation), Navigation));
        }

        protected override void OnDisappearing()
        {
            ViewModel.Disappear();

            base.OnDisappearing();
        }
    }
}

using System;
using System.Collections.Generic;
using Autofac;
using Tabi.DataObjects;
using Tabi.Shared.Resx;
using Tabi.Shared.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tabi.Shared.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StopDetailMotivePage : ContentPage
    {
        StopDetailMotiveViewModel ViewModel => vm ?? (vm = BindingContext as StopDetailMotiveViewModel);
        StopDetailMotiveViewModel vm;

        public StopDetailMotivePage(MotiveViewModel motiveViewModel)
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

            BindingContext = App.Container.Resolve<StopDetailMotiveViewModel>(new TypedParameter(typeof(MotiveViewModel), motiveViewModel), new TypedParameter(typeof(INavigation), Navigation));
        }

        protected override void OnDisappearing()
        {
            ViewModel.Disappear();

            base.OnDisappearing();
        }
    }
}

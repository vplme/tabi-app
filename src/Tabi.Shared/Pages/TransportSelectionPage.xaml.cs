using System;
using System.Collections.Generic;
using Autofac;
using Tabi.DataObjects;
using Tabi.Shared.Resx;
using Tabi.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tabi.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TransportSelectionPage : ContentPage
    {
        TransportSelectionViewModel ViewModel => vm ?? (vm = BindingContext as TransportSelectionViewModel);
        TransportSelectionViewModel vm;

        public TransportSelectionPage(TrackEntry trackEntry)
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<TransportSelectionViewModel>(
                new TypedParameter(typeof(INavigation), Navigation),
                new TypedParameter(typeof(TrackEntry), trackEntry));

            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
            {
                ExtendedToolbarItem saveToolbarItem = new ExtendedToolbarItem() { Done = true, Text = AppResources.SaveText };
                saveToolbarItem.SetBinding(ExtendedToolbarItem.CommandProperty, "SaveCommand");
                ToolbarItems.Add(saveToolbarItem);
            }

            ExtendedToolbarItem cancelToolbarItem = new ExtendedToolbarItem() { Left = true, Text = AppResources.CancelText };
            cancelToolbarItem.SetBinding(ExtendedToolbarItem.CommandProperty, "CancelCommand");
            ToolbarItems.Add(cancelToolbarItem);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
    }
}

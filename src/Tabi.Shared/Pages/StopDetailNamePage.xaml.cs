using System;
using System.Collections.Generic;
using Autofac;
using Tabi.Shared.Resx;
using Tabi.Shared.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tabi.Shared.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StopDetailNamePage : ContentPage
    {
        StopDetailNameViewModel ViewModel => vm ?? (vm = BindingContext as StopDetailNameViewModel);
        StopDetailNameViewModel vm;

        public StopDetailNamePage(StopVisitViewModel stopVisitViewModel)
        {
            InitializeComponent();

            if (Device.RuntimePlatform == Device.iOS)
            {
                ExtendedToolbarItem cancelToolbarItem = new ExtendedToolbarItem() { Left = true, Text = AppResources.CancelText };
                cancelToolbarItem.SetBinding(ExtendedToolbarItem.CommandProperty, "CancelCommand");
                ToolbarItems.Add(cancelToolbarItem);
            }

            ExtendedToolbarItem saveToolbarItem = new ExtendedToolbarItem() { Done = true, Text = AppResources.SaveText };
            saveToolbarItem.SetBinding(ExtendedToolbarItem.CommandProperty, "SaveCommand");
            ToolbarItems.Add(saveToolbarItem);

            BindingContext = App.Container.Resolve<StopDetailNameViewModel>(new TypedParameter(typeof(INavigation), Navigation),
                                                                            new TypedParameter(typeof(StopVisitViewModel), stopVisitViewModel));
        }
    }
}

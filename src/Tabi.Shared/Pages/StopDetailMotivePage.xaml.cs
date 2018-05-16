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
    public partial class StopDetailMotivePage : ContentPage
    {
        StopDetailMotiveViewModel ViewModel => vm ?? (vm = BindingContext as StopDetailMotiveViewModel);
        StopDetailMotiveViewModel vm;

        public StopDetailMotivePage()
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

            BindingContext = App.Container.Resolve<StopDetailMotiveViewModel>();

            ViewModel.Navigation = Navigation;
        }
    }
}

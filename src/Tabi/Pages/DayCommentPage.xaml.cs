using System;
using System.Collections.Generic;
using System.Windows.Input;
using Autofac;
using Tabi.Controls;
using Tabi.Resx;
using Tabi.ViewModels;
using Xamarin.Forms;

namespace Tabi.Pages
{
    public partial class DayCommentPage : ContentPage
    {
        DayCommentViewModel ViewModel => vm ?? (vm = BindingContext as DayCommentViewModel);
        DayCommentViewModel vm;

        public DayCommentPage()
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<DayCommentViewModel>(new TypedParameter(typeof(INavigation), Navigation));

            if (Device.RuntimePlatform == Device.iOS)
            {
                ExtendedToolbarItem saveToolbarItem = new ExtendedToolbarItem() { Done = true, Text = AppResources.SaveText };
                saveToolbarItem.SetBinding(ExtendedToolbarItem.CommandProperty, "SaveCommand");
                ToolbarItems.Add(saveToolbarItem);
            }

            ExtendedToolbarItem cancelToolbarItem = new ExtendedToolbarItem() { Left = true, Text = AppResources.CancelText };
            cancelToolbarItem.SetBinding(ExtendedToolbarItem.CommandProperty, "CancelCommand");
            ToolbarItems.Add(cancelToolbarItem);

        }
    }
}

using System;
using System.Collections.Generic;
using System.Windows.Input;
using Tabi.Controls;
using Tabi.Resx;
using Xamarin.Forms;

namespace Tabi.Pages
{
    public partial class DayCommentPage : ContentPage
    {
        public DayCommentPage()
        {

            if (Device.RuntimePlatform == Device.iOS)
            {
                ExtendedToolbarItem saveToolbarItem = new ExtendedToolbarItem() { Done = true, Text = AppResources.SaveText };
                saveToolbarItem.SetBinding(ExtendedToolbarItem.CommandProperty, "SaveCommand");
                ToolbarItems.Add(saveToolbarItem);
            }

            ExtendedToolbarItem cancelToolbarItem = new ExtendedToolbarItem() { Left = true, Text = AppResources.CancelText };
            cancelToolbarItem.SetBinding(ExtendedToolbarItem.CommandProperty, "CancelCommand");
            ToolbarItems.Add(cancelToolbarItem);


            InitializeComponent();

            CancelCommand = new Command(async () =>
            {
                await Navigation.PopAsync();
            });

            SaveCommand = new Command(async () =>
            {
                await Navigation.PopAsync();
            });

            BindingContext = this;
        }

        public ICommand CancelCommand { get; set; }

        public ICommand SaveCommand { get; set; }

    }
}

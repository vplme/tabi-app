using System;
using System.Collections.Generic;
using System.Windows.Input;
using Autofac;
using Tabi.Controls;
using Tabi.Controls.MultiSelectListView;
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

            ViewModel.PhoneItems.CollectionChanged += Items_CollectionChanged;
            ViewModel.TravelItems.CollectionChanged += Items_CollectionChanged;

        }

        void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            AdjustListViewHeight(SelectListView, ViewModel.TravelItems);
            AdjustListViewHeight(SelectListView2, ViewModel.PhoneItems);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.LoadExistingQuestions();
        }

        void AdjustListViewHeight(ListView listView, SelectableObservableCollection<SelectionKeyItem> list)
        {
            var adjust = Xamarin.Forms.Device.RuntimePlatform != Xamarin.Forms.Device.Android ? 1 : -list.Count + 1;
            listView.HeightRequest = (list.Count * listView.RowHeight) - adjust;
        }
    }
}

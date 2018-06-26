using System;
using System.Collections.Generic;
using Autofac;
using Tabi.Shared.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tabi.Shared.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchMotivePage : ContentPage
    {
        SearchMotiveViewModel ViewModel => vm ?? (vm = BindingContext as SearchMotiveViewModel);
        SearchMotiveViewModel vm;

        public SearchMotivePage(MotiveSelectionViewModel motiveSelectionViewModel)
        {
            InitializeComponent();

            BindingContext = App.Container.Resolve<SearchMotiveViewModel>(
                new TypedParameter(typeof(INavigation), Navigation),
                new TypedParameter(typeof(MotiveSelectionViewModel), motiveSelectionViewModel));

            ViewModel.PossibleMotives.CollectionChanged += (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
            {
                AdjustListViewHeight();
            };
        }
        async void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            await ViewModel.SelectMotive((MotiveOptionViewModel)e.SelectedItem);
        }

        void AdjustListViewHeight()
        {
            var adjust = Xamarin.Forms.Device.RuntimePlatform != Xamarin.Forms.Device.Android ? 1 : -ViewModel.PossibleMotives.Count + 1;
            PossibleMotivesListView.HeightRequest = (ViewModel.PossibleMotives.Count * PossibleMotivesListView.RowHeight) - adjust;
        }

    }
}

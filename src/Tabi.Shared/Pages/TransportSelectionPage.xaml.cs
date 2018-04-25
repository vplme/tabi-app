using System;
using System.Collections.Generic;
using Autofac;
using Tabi.DataObjects;
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
            BindingContext = App.Container.Resolve<TransportSelectionViewModel>();

            ViewModel.Navigation = Navigation;

            ViewModel.TrackEntry = trackEntry;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            ViewModel.FinishedTransportSelection();
        }
    }
}

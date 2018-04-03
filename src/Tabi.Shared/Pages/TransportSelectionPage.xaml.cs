using System;
using System.Collections.Generic;
using Tabi.ViewModels;
using Xamarin.Forms;

namespace Tabi.Pages
{
    public partial class TransportSelectionPage : ContentPage
    {
        TransportSelectionViewModel ViewModel => vm ?? (vm = BindingContext as TransportSelectionViewModel);
        TransportSelectionViewModel vm;

        public TransportSelectionPage()
        {
            InitializeComponent();
            BindingContext = new TransportSelectionViewModel(this.Navigation);

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            ViewModel.FinishedTransportSelection();
        }
    }
}

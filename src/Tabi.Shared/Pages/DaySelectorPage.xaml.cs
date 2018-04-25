using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Tabi.Model;
using Tabi.ViewModels;
using Xamarin.Forms;

namespace Tabi.Pages
{
    public partial class DaySelectorPage : ContentPage
    {
        DaySelectorViewModel ViewModel => vm ?? (vm = BindingContext as DaySelectorViewModel);
        DaySelectorViewModel vm;

        public DaySelectorPage()
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<DaySelectorViewModel>();
        }

        async void ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {

            if (e.SelectedItem == null)
            {
                return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
            }

            Day selectedDay = (Day)e.SelectedItem;

            ViewModel.SelectedDate = selectedDay.Time;

            await Navigation.PopAsync();
        }
    }
}

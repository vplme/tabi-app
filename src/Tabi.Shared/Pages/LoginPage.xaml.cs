using System;
using System.Collections.Generic;
using Tabi.ViewModels;
using Xamarin.Forms;

namespace Tabi.Pages
{
    public partial class LoginPage : ContentPage
    {
        LoginViewModel ViewModel => vm ?? (vm = BindingContext as LoginViewModel);
        LoginViewModel vm;
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = new LoginViewModel();
        }


    }
}

using Autofac;
using Tabi.ViewModels;
using Xamarin.Forms;

namespace Tabi.Pages.OnBoarding
{
    public partial class LoginPage : ContentPage
    {
        LoginViewModel ViewModel => vm ?? (vm = BindingContext as LoginViewModel);
        LoginViewModel vm;

        public LoginPage()
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<LoginViewModel>();
            ViewModel.Page = this;
            ViewModel.Navigation = Navigation;
        }

        void Handle_UsernameEntryCompleted(object sender, System.EventArgs e)
        {
            passwordEntry.Focus();
        }

        async void Handle_PasswordEntryCompletedAsync(object sender, System.EventArgs e)
        {
        }
    }
}

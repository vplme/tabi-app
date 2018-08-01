using Autofac;
using Tabi.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tabi.Pages.OnBoarding
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WelcomePage : ContentPage
    {
        WelcomeViewModel ViewModel => vm ?? (vm = BindingContext as WelcomeViewModel);
        WelcomeViewModel vm;

        public WelcomePage()
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<WelcomeViewModel>();
            ViewModel.Navigation = this.Navigation;
            ViewModel.Page = this;
        }

       
    }
}

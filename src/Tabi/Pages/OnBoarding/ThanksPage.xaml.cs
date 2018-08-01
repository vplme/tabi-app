using Autofac;
using Tabi.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tabi.Pages.OnBoarding
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ThanksPage : ContentPage
    {
        ThanksViewModel ViewModel => vm ?? (vm = BindingContext as ThanksViewModel);
        ThanksViewModel vm;

        public ThanksPage()
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<ThanksViewModel>();
            ViewModel.Navigation = this.Navigation;
            ViewModel.Page = this;
        }
    }
}

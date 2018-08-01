using Autofac;
using Tabi.ViewModels;
using Xamarin.Forms;

namespace Tabi.Pages.OnBoarding
{
    public partial class MotionAccessPage : ContentPage
    {
        MotionAccessViewModel ViewModel => vm ?? (vm = BindingContext as MotionAccessViewModel);
        MotionAccessViewModel vm;

        public MotionAccessPage()
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<MotionAccessViewModel>();
            ViewModel.Page = this;
            ViewModel.Navigation = Navigation;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ViewModel.OnAppearingAsync();
        }
    }
}

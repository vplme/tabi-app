using Tabi.Shared.ViewModels;
using Xamarin.Forms;

namespace Tabi.Pages
{
    public partial class IntroPage : ContentPage
    {
        IntroViewModel ViewModel => vm ?? (vm = BindingContext as IntroViewModel);
        IntroViewModel vm;

        public IntroPage()
        {
            BindingContext = new IntroViewModel(this);
            InitializeComponent();
        }
    }
}

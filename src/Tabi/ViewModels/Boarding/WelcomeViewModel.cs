using System.Windows.Input;
using Tabi.Pages.OnBoarding;
using Xamarin.Forms;

namespace Tabi.ViewModels
{
    public class WelcomeViewModel : BaseViewModel
    {

        public WelcomeViewModel()
        {
            NextCommand = new Command(Next);
        }

        public INavigation Navigation { get; set; }

        public Page Page { get; set; }

        public ICommand NextCommand { get; set; }

        private async void Next()
        {
            await Navigation.PushAsync(new LoginPage(), false);
        }
    }
}

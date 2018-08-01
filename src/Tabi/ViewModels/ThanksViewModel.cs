using System.Threading.Tasks;
using System.Windows.Input;
using Tabi.Helpers;
using Tabi.Pages;
using Xamarin.Forms;

namespace Tabi.ViewModels
{
    public class ThanksViewModel : BaseViewModel
    {
        public INavigation Navigation { get; set; }

        public Page Page { get; set; }

        public ThanksViewModel()
        {
            NextCommand = new Command(async () => { await Next(); });
        }
        public ICommand NextCommand { get; set; }

        private async Task Next()
        {
            Settings.Current.PermissionsGranted = true;
            Settings.ShowTour = true;
            Application.Current.MainPage = new NavigationPage(new ActivityOverviewPage());
        }

    }
}

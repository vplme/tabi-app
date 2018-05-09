using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Tabi.Shared.ViewModels
{
    public class ThanksViewModel : BaseViewModel
    {
        public INavigation Navigation { get; set; }

        public Page Page { get; set; }

        public ThanksViewModel()
        {
            NextCommand = new Command(Next);
        }
        public ICommand NextCommand { get; set; }

        private void Next()
        {
            Settings.Current.PermissionsGranted = true;
            Application.Current.MainPage = new NavigationPage(new ActivityOverviewPage());
        }

    }
}

using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Tabi.Shared.ViewModels
{
    public class StopDetailNameViewModel : BaseViewModel
    {
        public StopDetailNameViewModel()
        {

            SaveCommand = new Command(async () => { await PopPageAsync(); });
            CancelCommand = new Command(async () => { await PopPageAsync(); });

        }

        private async Task PopPageAsync()
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                await Navigation.PopModalAsync();
            }
            else
            {
                await Navigation.PopAsync();
            }
        }

        public INavigation Navigation { get; set; }

        public ICommand SaveCommand { get; set; }

        public ICommand CancelCommand { get; set; }
    }
}

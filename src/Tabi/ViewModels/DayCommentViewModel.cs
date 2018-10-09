using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Tabi.ViewModels
{
    public class DayCommentViewModel : BaseViewModel
    {
        public DayCommentViewModel(INavigation navigation)
        {
            SaveCommand = new Command(async () =>
            {
                await navigation.PopAsync();
            });

            CancelCommand = new Command(async () =>
            {
                await navigation.PopAsync();
            });
        }

        public ICommand SaveCommand { get; set; }

        public ICommand CancelCommand { get; set; }
    }
}

using Tabi.ViewModels;
using Xamarin.Forms;

namespace Tabi.Pages
{
    public partial class AboutSettings : ContentPage
    {
        SettingsViewModel ViewModel => vm ?? (vm = BindingContext as SettingsViewModel);
        SettingsViewModel vm;

        public AboutSettings(SettingsViewModel viewModel)
        {
            BindingContext = viewModel;
            InitializeComponent();
        }
    }
}

using Tabi.ViewModels;
using Xamarin.Forms;

namespace Tabi.Pages
{
    public partial class SettingsPrivacyPage : ContentPage
    {
        SettingsViewModel ViewModel => vm ?? (vm = BindingContext as SettingsViewModel);
        SettingsViewModel vm;

        public SettingsPrivacyPage(SettingsViewModel viewModel)
        {
            BindingContext = viewModel;

            InitializeComponent();
        }
    }
}

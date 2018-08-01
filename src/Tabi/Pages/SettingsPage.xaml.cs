using Autofac;
using Tabi.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tabi.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        SettingsViewModel ViewModel => vm ?? (vm = BindingContext as SettingsViewModel);
        SettingsViewModel vm;

        public SettingsPage()
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<SettingsViewModel>(new TypedParameter(typeof(INavigation), Navigation));
        }
    }
}

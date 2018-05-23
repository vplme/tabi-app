using Autofac;
using Tabi.Model;
using Tabi.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tabi.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DaySelectorPage : ContentPage
    {
        DaySelectorViewModel ViewModel => vm ?? (vm = BindingContext as DaySelectorViewModel);
        DaySelectorViewModel vm;

        public DaySelectorPage()
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<DaySelectorViewModel>(new TypedParameter(typeof(INavigation), Navigation));
        }

        async void ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

            if (e.SelectedItem == null)
            {
                return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
            }

            Day selectedDay = e.SelectedItem as Day;
            await ViewModel.ListSelectedDay(selectedDay);
        }
    }
}

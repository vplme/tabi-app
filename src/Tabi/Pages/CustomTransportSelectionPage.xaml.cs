using Autofac;
using Tabi.Controls;
using Tabi.Controls.MultiSelectListView;
using Tabi.Resx;
using Tabi.ViewModels;
using Xamarin.Forms;

namespace Tabi.Pages
{
    public partial class CustomTransportSelectionPage : ContentPage
    {
        CustomTransportSelectionViewModel ViewModel => vm ?? (vm = BindingContext as CustomTransportSelectionViewModel);
        CustomTransportSelectionViewModel vm;

        public CustomTransportSelectionPage(SelectableObservableCollection<Model.TransportModeItem> items)
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                ExtendedToolbarItem saveToolbarItem = new ExtendedToolbarItem() { Done = true, Text = AppResources.SaveText };
                saveToolbarItem.SetBinding(ExtendedToolbarItem.CommandProperty, "SaveCommand");
                ToolbarItems.Add(saveToolbarItem);
            }

            ExtendedToolbarItem cancelToolbarItem = new ExtendedToolbarItem() { Left = true, Text = AppResources.CancelText };
            cancelToolbarItem.SetBinding(ExtendedToolbarItem.CommandProperty, "CancelCommand");
            ToolbarItems.Add(cancelToolbarItem);

            InitializeComponent();

            BindingContext = App.Container.Resolve<CustomTransportSelectionViewModel>(
                new TypedParameter(typeof(INavigation), Navigation),
                new NamedParameter("activeItems", items));
        }
    }
}

using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Tabi
{
    public partial class TabiTabbedPage : TabbedPage
    {
        public TabiTabbedPage()
        {
            InitializeComponent();
        }



        void SettingsButton_Clicked(object sender, System.EventArgs e)
        {
            ToolbarItem item = sender as ToolbarItem;
            NavigationPage nPage = item.Parent as NavigationPage;
            if (nPage != null)
            {
                SettingsPage sPage = new SettingsPage();
                nPage.PushAsync(sPage);
            }
        }
    }
}

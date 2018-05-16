using System;
using AListView = Android.Widget.ListView;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Tabi.Droid.Effects;

[assembly: ResolutionGroupName("Tabi")]
[assembly: ExportEffect(typeof(NoScrollListViewEffect), "NoScrollListViewEffect")]
namespace Tabi.Droid.Effects
{
    public class NoScrollListViewEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            var listView = Control as AListView;
            if (listView != null)
            {
                listView.VerticalScrollBarEnabled = false;
                listView.HorizontalScrollBarEnabled = false;
                listView.FastScrollEnabled = false;
            }
        }

        protected override void OnDetached()
        {
            var listView = Control as AListView;
            if (listView != null)
            {
                listView.VerticalScrollBarEnabled = true;
                listView.HorizontalScrollBarEnabled = true;
                listView.FastScrollEnabled = true;
            }
        }
    }
}

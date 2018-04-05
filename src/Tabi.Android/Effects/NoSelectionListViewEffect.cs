using System;
using Tabi.Droid.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AListView = Android.Widget.ListView;
using Android.Graphics.Drawables;

[assembly: ExportEffect(typeof(NoSelectionListViewEffect), "NoSelectionListViewEffect")]
namespace Tabi.Droid.Effects
{
    public class NoSelectionListViewEffect : PlatformEffect
    {
        Drawable dr;

        public NoSelectionListViewEffect()
        {
        }

        protected override void OnAttached()
        {
            var aListView = Control as AListView;
            dr = aListView.Selector;
            if (aListView != null)
            {
                aListView.Focusable = false;
                //aListView.Enabled = false;
                aListView.FocusableInTouchMode = false;
                //aListView.SetSelector(0);

            }
            else
            {
                throw new InvalidOperationException(string.Format("Attaching effect to wrong kind of class. Expected: ListView Got: {0}", Control.GetType()));
            }
        }

        protected override void OnDetached()
        {

            var aListView = Control as AListView;
            dr = aListView.Selector;
            if (aListView != null)
            {

                //aListView.SetSelector(Drawable.);

            }
            else
            {
                throw new InvalidOperationException(string.Format("Attaching effect to wrong kind of class. Expected: ListView Got: {0}", Control.GetType()));
            }
        }
    }
}

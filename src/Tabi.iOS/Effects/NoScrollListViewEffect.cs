using System;
using Tabi.iOS.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(NoScrollListViewEffect), "NoScrollListViewEffect")]
namespace Tabi.iOS.Effects
{
    public class NoScrollListViewEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            var tableView = Control as UITableView;
            if (tableView != null)
            {
                tableView.ScrollEnabled = false;
            }
            else
            {
                throw new InvalidOperationException(string.Format("Attaching effect to wrong kind of class. Expected: UITableView Got: {0}", Control.GetType()));
            }
        }

        protected override void OnDetached()
        {
            var tableView = Control as UITableView;
            if (tableView != null)
            {
                tableView.ScrollEnabled = true;
            }
            else
            {
                throw new InvalidOperationException(string.Format("Attaching effect to wrong kind of class. Expected: UITableView Got: {0}", Control.GetType()));
            }
        }
    }
}

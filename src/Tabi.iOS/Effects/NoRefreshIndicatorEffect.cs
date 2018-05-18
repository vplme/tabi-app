using System;
using Tabi.iOS.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(NoRefreshIndicatorEffect), "NoRefreshIndicatorEffect")]
namespace Tabi.iOS.Effects
{
    public class NoRefreshIndicatorEffect : PlatformEffect
    {
        private UIColor previousColor;

        protected override void OnAttached()
        {
            var tableView = Control as UITableView;
            if (tableView != null)
            {
                previousColor = tableView.RefreshControl.TintColor;
                tableView.RefreshControl.TintColor = UIColor.Clear;
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
                tableView.RefreshControl.TintColor = previousColor;
            }
            else
            {
                throw new InvalidOperationException(string.Format("Attaching effect to wrong kind of class. Expected: UITableView Got: {0}", Control.GetType()));
            }
        }
    }
}
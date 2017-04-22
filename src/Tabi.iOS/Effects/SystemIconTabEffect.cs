using System;
using Tabi.iOS.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(SystemIconTabEffect), "SystemIconTabEffect")]
namespace Tabi.iOS.Effects
{
    public class SystemIconTabEffect : PlatformEffect
	{
		protected override void OnAttached()
		{
			var tableView = Control as UITableView;
			if (tableView != null)
			{
				tableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;

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
				tableView.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
			}
			else
			{
				throw new InvalidOperationException(string.Format("Attaching effect to wrong kind of class. Expected: UITableView Got: {0}", Control.GetType()));
			}
		}
	}
}

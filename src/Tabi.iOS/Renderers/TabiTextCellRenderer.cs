using System;
using Tabi.iOS.Renderers;
using Tabi.Shared.Controls;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TabiTextCell), typeof(TabiTextCellRenderer))]
namespace Tabi.iOS.Renderers
{
    public class TabiTextCellRenderer : TextCellRenderer
    {
        static readonly Color DefaultDetailColor = new Color(.32, .4, .57);
        static readonly Color DefaultTextColor = Color.Black;

        // Method partially copied from TextCellRenderer. 
        // Can't base() since we need to create the UITableViewCell in our version of the method.
        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            var textCell = (TabiTextCell)item;

            var tvc = reusableCell as CellTableViewCell;
            if (tvc == null)
            {
                // Change 1: use our selected cellstyle
                UITableViewCellStyle cellStyle = StringToStyle(textCell.UITableViewStyle);
                tvc = new CellTableViewCell(cellStyle, item.GetType().FullName);
            }
            else
            {
                tvc.Cell.PropertyChanged -= tvc.HandlePropertyChanged;
            }

            tvc.Cell = textCell;
            textCell.PropertyChanged += tvc.HandlePropertyChanged;
            tvc.PropertyChanged = HandlePropertyChanged;

            tvc.TextLabel.Text = textCell.Text;
            tvc.DetailTextLabel.Text = textCell.Detail;
            tvc.TextLabel.TextColor = textCell.TextColor.ToUIColor(DefaultTextColor);
            tvc.DetailTextLabel.TextColor = textCell.DetailColor.ToUIColor(DefaultDetailColor);

            // Change 2: add accessory
            tvc.Accessory = StringToAccessoryType(textCell.UITableViewCellAccessory);

            WireUpForceUpdateSizeRequested(item, tvc, tv);

            UpdateIsEnabled(tvc, textCell);

            UpdateBackground(tvc, item);

            return tvc;
        }

        private UITableViewCellStyle StringToStyle(string str)
        {
            UITableViewCellStyle style = UITableViewCellStyle.Subtitle;

            str = str ?? "";

            switch (str.ToLower())
            {
                case "default":
                    style = UITableViewCellStyle.Default;
                    break;
                case "value1":
                    style = UITableViewCellStyle.Value1;
                    break;
                case "value2":
                    style = UITableViewCellStyle.Value2;
                    break;
                case "subtitle":
                    style = UITableViewCellStyle.Subtitle;
                    break;
                default:
                    style = UITableViewCellStyle.Subtitle;
                    break;
            }

            return style;
        }

        private UITableViewCellAccessory StringToAccessoryType(string str)
        {
            UITableViewCellAccessory accessory = UITableViewCellAccessory.None;

            str = str ?? "";

            switch (str.ToLower())
            {
                case "none":
                    accessory = UITableViewCellAccessory.None;
                    break;
                case "checkmark":
                    accessory = UITableViewCellAccessory.Checkmark;
                    break;
                case "detail-button":
                    accessory = UITableViewCellAccessory.DetailButton;
                    break;
                case "detail-disclosure-button":
                    accessory = UITableViewCellAccessory.DetailDisclosureButton;
                    break;
                case "disclosure":
                    accessory = UITableViewCellAccessory.DisclosureIndicator;
                    break;
                default:
                    break;
            }

            return accessory;
        }

        // Method copied from TextCellRenderer. Can't override since it's private.
        static void UpdateIsEnabled(CellTableViewCell cell, TextCell entryCell)
        {
            cell.UserInteractionEnabled = entryCell.IsEnabled;
            cell.TextLabel.Enabled = entryCell.IsEnabled;
            cell.DetailTextLabel.Enabled = entryCell.IsEnabled;
        }
    }
}

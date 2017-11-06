using System;
using Tabi.iOS.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(BorderlessEntryEffect), "BorderlessEntryEffect")]
namespace Tabi.iOS.Effects
{
    public class BorderlessEntryEffect : PlatformEffect
    {
        public BorderlessEntryEffect()
        {
        }

        protected override void OnAttached()
        {
            var textView = Control as UITextField;
            if (textView != null)
            {
                textView.BorderStyle = UITextBorderStyle.None;
            }
            else
            {
                throw new InvalidOperationException(string.Format("Attaching effect to wrong kind of class. Expected: UITextField Got: {0}", Control.GetType()));
            }
        }

        protected override void OnDetached()
        {
            var textView = Control as UITextField;
            if (textView != null)
            {
                textView.BorderStyle = UITextBorderStyle.Line;

            }
            else
            {
                throw new InvalidOperationException(string.Format("Attaching effect to wrong kind of class. Expected: UITextField Got: {0}", Control.GetType()));
            }
        }
    }
}

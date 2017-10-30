using System;
using Tabi.iOS.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(NoHelperEntryEffect), "NoHelperEntryEffect")]
namespace Tabi.iOS.Effects
{
    public class NoHelperEntryEffect : PlatformEffect
    {
        public NoHelperEntryEffect()
        {
        }

        protected override void OnAttached()
        {
            var textView = Control as UITextField;
            if (textView != null)
            {
                textView.SpellCheckingType = UITextSpellCheckingType.No;
                textView.AutocorrectionType = UITextAutocorrectionType.No;
                textView.AutocapitalizationType = UITextAutocapitalizationType.None;
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
                textView.SpellCheckingType = UITextSpellCheckingType.Default;
                textView.AutocorrectionType = UITextAutocorrectionType.Default;
                textView.AutocapitalizationType = UITextAutocapitalizationType.Sentences;
            }
            else
            {
                throw new InvalidOperationException(string.Format("Attaching effect to wrong kind of class. Expected: UITextField Got: {0}", Control.GetType()));
            }
        }
    }
}

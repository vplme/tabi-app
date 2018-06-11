using System;
using Tabi.Droid.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(BorderlessEntryEffect), "BorderlessEntryEffect")]
namespace Tabi.Droid.Effects
{
    public class BorderlessEntryEffect : PlatformEffect
    {
        Color previousColor;
        protected override void OnAttached()
        {
            FormsEditText formEdit = Control as FormsEditText;
            if (formEdit != null)
            {
                formEdit.SetBackgroundColor(Android.Graphics.Color.Transparent);   
            }
        }

        protected override void OnDetached()
        {
        }
    }
}

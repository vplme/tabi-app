using System;
using System.ComponentModel;
using System.Diagnostics;
using Tabi.iOS.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(LeftToolbarItemEffect), "LeftToolbarItemEffect")]
namespace Tabi.iOS.Effects
{
    public class LeftToolbarItemEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            Debug.WriteLine("Testing Effect");
        }

        protected override void OnDetached()
        {
            
        }
    }
}

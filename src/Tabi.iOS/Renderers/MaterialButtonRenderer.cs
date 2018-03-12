using CoreGraphics;
using Tabi.iOS.Renderers;
using Tabi.Shared.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MaterialButton), typeof(MaterialButtonRenderer))]
namespace Tabi.iOS.Renderers
{

    public class MaterialButtonRenderer : ButtonRenderer
    {
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            //if (Element.BackgroundColor.A == 0)
            //    return;
            //Layer.ShadowRadius = 2.0f;
            //Layer.ShadowColor = UIColor.Gray.CGColor;
            //Layer.ShadowOffset = new CGSize(1, 1);
            //Layer.ShadowOpacity = 0.80f;
            //Layer.ShadowPath = UIBezierPath.FromRect(Layer.Bounds).CGPath;
            //Layer.MasksToBounds = false;

        }
    }
}

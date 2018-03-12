using Tabi.iOS;
using Tabi;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(StopDetailTableView), typeof(StopDetailTableViewRenderer))]
namespace Tabi.iOS
{
    public class StopDetailTableViewRenderer : TableViewRenderer
    {
        public StopDetailTableViewRenderer()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<TableView> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null)
            {
      
            }

            if (e.NewElement != null)
            {
                Control.Bounces = false;

            }
        }
    }
}


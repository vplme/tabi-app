using Xamarin.Forms;

namespace Tabi.Controls
{
    public class DynamicHeightListView : ListView
    {
        public DynamicHeightListView()
        {
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            var minimumSize = new Size(40, 40);
            Size request;
            double requestHeight = 0;
            double minimumHeight = 0;

            var itemsView = this as ITemplatedItemsView<Cell>;
            foreach (Cell cell in itemsView.TemplatedItems)
            {
                ViewCell vCell = cell as ViewCell;

                SizeRequest cellRequest = vCell.View.Measure(widthConstraint, heightConstraint);
                requestHeight += cellRequest.Request.Height;
                minimumHeight += cellRequest.Minimum.Height;
            }

            minimumHeight -= itemsView.TemplatedItems.Count;

            request = new Size(widthConstraint, requestHeight);

            return new SizeRequest(request, minimumSize);
        }
    }
}

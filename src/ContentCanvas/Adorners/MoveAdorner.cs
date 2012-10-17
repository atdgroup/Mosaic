using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace DiagramDesigner
{
    public class MoveAdorner : Adorner
    {
        private VisualCollection visuals;
        private MoveChrome chrome;

        protected override int VisualChildrenCount
        {
            get
            {
                return this.visuals.Count;
            }
        }

        public MoveAdorner(ContentControl designerItem)
            : base(designerItem)
        {
            this.chrome = new MoveChrome();            
            this.visuals = new VisualCollection(this);
            this.visuals.Add(this.chrome);
            this.chrome.DataContext = designerItem;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            this.chrome.Arrange(new Rect(arrangeBounds));
            return arrangeBounds;
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.visuals[index];
        }
    }
}

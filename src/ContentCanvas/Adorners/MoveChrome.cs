using System.Windows;
using System.Windows.Controls;

namespace DiagramDesigner
{
    public class MoveChrome : Control
    {
        static MoveChrome()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MoveChrome), new FrameworkPropertyMetadata(typeof(MoveChrome)));
        }
    }
}

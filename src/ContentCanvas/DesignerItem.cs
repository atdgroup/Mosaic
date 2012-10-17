using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DiagramDesigner
{
    public class DesignerItem : ContentControl
    {
        public static readonly RoutedEvent SelectedEvent = EventManager.RegisterRoutedEvent(
            "Selected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DesignerItem));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set 
            { 
                SetValue(IsSelectedProperty, value);

                // Raise the event
                if (value)
                {
                    RaiseEvent(new RoutedEventArgs(DesignerItem.SelectedEvent, this));
                }
            }
        }

        // Provide CLR accessors for the event
        public event RoutedEventHandler Selected
        {
            add { AddHandler(SelectedEvent, value); }
            remove { RemoveHandler(SelectedEvent, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
          DependencyProperty.Register("IsSelected", typeof(bool),
                                      typeof(DesignerItem),
                                      new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty MoveThumbTemplateProperty =
            DependencyProperty.RegisterAttached("MoveThumbTemplate", typeof(ControlTemplate), typeof(DesignerItem));

        public static ControlTemplate GetMoveThumbTemplate(UIElement element)
        {
            return (ControlTemplate)element.GetValue(MoveThumbTemplateProperty);
        }

        public static void SetMoveThumbTemplate(UIElement element, ControlTemplate value)
        {
            element.SetValue(MoveThumbTemplateProperty, value);
        }

        static DesignerItem()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignerItem), new FrameworkPropertyMetadata(typeof(DesignerItem)));
        }

        public DesignerItem()
        {
            this.Loaded += new RoutedEventHandler(this.DesignerItem_Loaded);
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            DesignerCanvas designer = VisualTreeHelper.GetParent(this) as DesignerCanvas;

            if (designer != null)
            {
                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                {
                    this.IsSelected = !this.IsSelected;
                }
                else
                {
                    if (!this.IsSelected)
                    {
                        designer.DeselectAll();
                        this.IsSelected = true;
                    }
                }
            }

            e.Handled = false;
        }

        private void DesignerItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Template != null)
            {
                ContentPresenter contentPresenter =
                    this.Template.FindName("PART_ContentPresenter", this) as ContentPresenter;

                MoveThumb thumb =
                    this.Template.FindName("PART_MoveThumb", this) as MoveThumb;

                if (contentPresenter != null && thumb != null)
                {
                    UIElement contentVisual =
                        VisualTreeHelper.GetChild(contentPresenter, 0) as UIElement;

                    if (contentVisual != null)
                    {
                        ControlTemplate template =
                            DesignerItem.GetMoveThumbTemplate(contentVisual) as ControlTemplate;

                        if (template != null)
                        {
                            thumb.Template = template;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Assigns the element a z-index which will ensure that 
        /// it is in front of every other element in the Canvas.
        /// The z-index of every element whose z-index is between 
        /// the element's old and new z-index will have its z-index 
        /// decremented by one.
        /// </summary>
        /// <param name="targetElement">
        /// The element to be sent to the front of the z-order.
        /// </param>
        public void BringToFront()
        {
            this.UpdateZOrder(true);
        }

        /// <summary>
        /// Assigns the element a z-index which will ensure that 
        /// it is behind every other element in the Canvas.
        /// The z-index of every element whose z-index is between 
        /// the element's old and new z-index will have its z-index 
        /// incremented by one.
        /// </summary>
        /// <param name="targetElement">
        /// The element to be sent to the back of the z-order.
        /// </param>
        public void SendToBack()
        {
            this.UpdateZOrder(false);
        }

        #region Private Helpers

        /// <summary>
        /// Helper method used by the BringToFront and SendToBack methods.
        /// </summary>
        /// <param name="element">
        /// The element to bring to the front or send to the back.
        /// </param>
        /// <param name="bringToFront">
        /// Pass true if calling from BringToFront, else false.
        /// </param>
        private void UpdateZOrder(bool bringToFront)
        {
            DesignerCanvas designer = VisualTreeHelper.GetParent(this) as DesignerCanvas;

            #region Safety Check

            if (designer == null)
                throw new ArgumentException("Must be a child element of the Canvas.", "element");

            #endregion // Safety Check

            #region Calculate Z-Indici And Offset

            // Determine the Z-Index for the target UIElement.
            int elementNewZIndex = -1;
            if (bringToFront)
            {
                foreach (DesignerItem elem in designer.Children)
                    if (elem.Visibility != Visibility.Collapsed)
                        ++elementNewZIndex;
            }
            else
            {
                elementNewZIndex = 0;
            }

            // Determine if the other UIElements' Z-Index 
            // should be raised or lowered by one. 
            int offset = (elementNewZIndex == 0) ? +1 : -1;

            int elementCurrentZIndex = Canvas.GetZIndex(this);

            #endregion // Calculate Z-Indici And Offset

            #region Update Z-Indici

            // Update the Z-Index of every UIElement in the Canvas.
            foreach (DesignerItem childElement in designer.Children)
            {
                if (childElement == this)
                    Canvas.SetZIndex(this, elementNewZIndex);
                else
                {
                    int zIndex = Canvas.GetZIndex(childElement);

                    // Only modify the z-index of an element if it is  
                    // in between the target element's old and new z-index.
                    if (bringToFront && elementCurrentZIndex < zIndex ||
                        !bringToFront && zIndex < elementCurrentZIndex)
                    {
                        Canvas.SetZIndex(childElement, zIndex + offset);
                    }
                }
            }

            #endregion // Update Z-Indici
        }

        #endregion // Private Helpers
    }
}

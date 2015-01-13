using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace CustomControls
{
    public class ReorderableListBox : ListBox
    {
        private Point _startPoint;
        public bool IsDragging { get; private set; }
        public object DragItem { get; private set; }
        public AdornerLayer AdornerLayer { get; private set; }
        public DragAdorner Overlay { get; private set; }

        public int OriginalItemIndex { get; private set; }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this.PreviewMouseLeftButtonDown += ReorderableListBox_PreviewMouseLeftButtonDown;

            this.PreviewMouseLeftButtonUp += ReorderableListBox_PreviewMouseLeftButtonUp;

            this.PreviewMouseMove += ReorderableListBox_PreviewMouseMove;

            this.SelectionChanged += ReorderableListBox_SelectionChanged;

            IsDragging = false;
        }

        void ReorderableListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsDragging) return;
            DragItem = this.SelectedItem;
            OriginalItemIndex = this.SelectedIndex;
        }

        void ReorderableListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || !MovedEnoughDistance(e) || DragItem ==null ) { return; }
            IsDragging = true;
            if (Overlay != null)
            {
                Overlay.UpdatePosition(e.GetPosition(this));
                return;
            }
            ContentPresenter presenter = new ContentPresenter();
            presenter.Content = DragItem;
            presenter.ContentTemplate = this.ItemTemplate;
            presenter.Measure(new Size(double.MaxValue, double.MaxValue));
            Overlay = new DragAdorner(this, presenter, 0.8);

            AdornerLayer = AdornerLayer.GetAdornerLayer(this as Visual);
            AdornerLayer.Add(Overlay);
        }

        private bool MovedEnoughDistance(MouseEventArgs e)
        {
            Point currPoint = e.GetPosition(null);
            return Math.Abs(currPoint.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(currPoint.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance;
        }

        void ReorderableListBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsDragging) { return; }
            IsDragging = false;
            DragItem == null;
            AdornerLayer.Remove(Overlay);
            Overlay = null;
            base.RaiseEvent(new RoutedEventArgs(ItemsReorderedEvent, this));
        }

        private void ReorderableListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
        }

        public static readonly RoutedEvent ItemsReorderedEvent;
        public event RoutedEventHandler ItemsReordered
        {
            add { base.AddHandler(ItemsReorderedEvent, value); }
            remove { base.RemoveHandler(ItemsReorderedEvent, value); }
        }

        static ReorderableListBox()
        {
            ItemsReorderedEvent = EventManager.RegisterRoutedEvent("ItemsReordered", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ReorderableListBox));
        }
    }
}

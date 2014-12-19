using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FileNameOrganiser
{
    public class DragAdorner : Adorner
    {
        private VisualBrush _brush;
        private Size _size;
        private Point _location;

        public DragAdorner(UIElement owner, UIElement adornElement, double opacity)
            : base(owner)
        {
            _brush = new VisualBrush(adornElement);
            _brush.Opacity = opacity;
            _size = adornElement.DesiredSize;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            var p = _location;
            p.Offset(3, 3);

            drawingContext.DrawRectangle(_brush, null, new Rect(p, _size));
        }

        public void UpdatePosition(Point point)
        {
            _location = point;
            InvalidateVisual();
        }
    }
}

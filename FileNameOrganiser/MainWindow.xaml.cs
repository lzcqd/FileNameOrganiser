using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileNameOrganiser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point _dragStartPos;
        private DragAdorner _adorner;
        private AdornerLayer _layer;
        private ScrollViewer _scrollViewer;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            IsDragging = false;
            
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            FilesList.PreviewMouseLeftButtonDown += FilesList_PreviewMouseLeftButtonDown;
            FilesList.PreviewMouseMove += FilesList_PreviewMouseMove;
            FilesList.PreviewMouseLeftButtonUp += FilesList_PreviewMouseLeftButtonUp;
            _scrollViewer = FilesList.FindVisualChild<ScrollViewer>();
            _scrollViewer.MouseLeftButtonDown += _scrollViewer_MouseLeftButtonDown;
        }

        void _scrollViewer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FilesList.SelectedItem = null;
        }

        void FilesList_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsDragging) IsDragging = false;
        }

        void FilesList_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !IsDragging && FilesList.SelectedItem != null)
            {
                Point dragCurrPos = e.GetPosition(null);

                if ((Math.Abs(dragCurrPos.X - _dragStartPos.X) > SystemParameters.MinimumHorizontalDragDistance) &&
                    (Math.Abs(dragCurrPos.Y - _dragStartPos.Y) > SystemParameters.MinimumVerticalDragDistance))
                {
                    StartDrag(e);
                }
            }
        }

        private void StartDrag(MouseEventArgs e)
        {
            IsDragging = true;
            FilesList.DragOver += FilesList_DragOver;
            IDataObject data = new DataObject(FilesList.SelectedItem);

            _adorner = new DragAdorner(FilesList, (UIElement)e.OriginalSource, 0.8);
            _layer = AdornerLayer.GetAdornerLayer(FilesList as Visual);
            _layer.Add(_adorner);

            DragDropEffects de = DragDrop.DoDragDrop(this.FilesList, data, DragDropEffects.Move);
            FilesList.DragOver -= FilesList_DragOver;
            _layer.Remove(_adorner);
            _adorner = null;
            IsDragging = false;
        }

        private void FilesList_DragOver(object sender, DragEventArgs e)
        {
            _adorner.UpdatePosition(e.GetPosition(FilesList));
        }

        void FilesList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPos = e.GetPosition(null);
        }

        public bool IsDragging { get; private set; }

        
    }

    public static class Extensions
    {
        public static ChildItem FindVisualChild<ChildItem>(this DependencyObject root) where ChildItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                if (child != null && child is ChildItem)
                {
                    return child as ChildItem;
                }
                else
                {
                    var childOfChild = FindVisualChild<ChildItem>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }
    }
}

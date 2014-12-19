using System;
using System.Collections.Generic;
using System.IO;
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
using FileNameOrganiser.ViewModel;

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
        private MainViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            IsDragging = false;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = (MainViewModel)this.DataContext;
            FilesList.PreviewMouseLeftButtonDown += FilesList_PreviewMouseLeftButtonDown;
            FilesList.PreviewMouseMove += FilesList_PreviewMouseMove;
            FilesList.AllowDrop = true;
            _scrollViewer = FilesList.FindVisualChild<ScrollViewer>();
            _scrollViewer.MouseLeftButtonDown += _scrollViewer_MouseLeftButtonDown;
        }

        void _scrollViewer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FilesList.SelectedItem = null;
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
            FilesList.Drop += FilesList_Drop;
            IDataObject data = new DataObject(FilesList.SelectedItem);

            _adorner = new DragAdorner(FilesList, (UIElement)e.OriginalSource, 0.8);
            _layer = AdornerLayer.GetAdornerLayer(FilesList as Visual);
            _layer.Add(_adorner);

            DragDropEffects de = DragDrop.DoDragDrop(this.FilesList, data, DragDropEffects.Move);
            _adorner.UpdatePosition(e.GetPosition(FilesList));
            FilesList.DragOver -= FilesList_DragOver;
            FilesList.Drop -= FilesList_Drop;
            _layer.Remove(_adorner);
            _adorner = null;
            IsDragging = false;
        }

        void FilesList_Drop(object sender, DragEventArgs e)
        {
            if (!(sender is ListBoxItem)) return;
            var target = ((ListBoxItem)sender).DataContext;
            var source = e.Data.GetData(typeof(FileInfo));
            var startIndex = FilesList.Items.IndexOf(source);
            var dropIndex = FilesList.Items.IndexOf(target);
            _vm.Files.RemoveAt(startIndex);
            _vm.Files.Insert(dropIndex, source as FileInfo);
 
        }

        private void FilesList_DragOver(object sender, DragEventArgs e)
        {
            _adorner.UpdatePosition(e.GetPosition(FilesList));
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
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

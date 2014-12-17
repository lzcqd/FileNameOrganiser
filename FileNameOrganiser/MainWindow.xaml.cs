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
            IDataObject data = new DataObject(FilesList.SelectedItem);
            DragDropEffects de = DragDrop.DoDragDrop(this.FilesList, data, DragDropEffects.Move);
            IsDragging = false;
        }

        void FilesList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPos = e.GetPosition(null);
        }

        public bool IsDragging { get; private set; }
    }
}

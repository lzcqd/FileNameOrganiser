using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void FilesList_ItemsReordered(object sender, RoutedEventArgs e)
        {
            ((ObservableCollection<FileInfo>)FilesList.ItemsSource).Move(FilesList.OriginalItemIndex, FilesList.SelectedIndex);
            FilesList.SelectedItem = null;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (FilesList.SelectedItem == null) return;
            ((ObservableCollection<FileInfo>)FilesList.ItemsSource).Remove(FilesList.SelectedItem as FileInfo);
        }

    }
}

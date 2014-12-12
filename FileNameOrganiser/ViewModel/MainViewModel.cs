using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Ookii.Dialogs.Wpf;
using System.Linq;

namespace FileNameOrganiser.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private int _capacity = 5;
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            FolderOpenClickCommand = new RelayCommand(OnFolderOpenClick);
            CachedPaths = new ObservableCollection<string>();
        }

        public RelayCommand FolderOpenClickCommand { get; private set; }

        protected void OnFolderOpenClick()
        {
            VistaFolderBrowserDialog folderDialog = new VistaFolderBrowserDialog();
            folderDialog.ShowDialog();
            AddToSelectedPaths(folderDialog.SelectedPath);
            SelectedPath = folderDialog.SelectedPath;
        }

        public ObservableCollection<string> CachedPaths { get; private set; }

        private void AddToSelectedPaths(string selectedPath)
        {
            if (CachedPaths.Contains(selectedPath)) return;
            
            if (CachedPaths.Count >= _capacity)
            {
                CachedPaths.RemoveAt(CachedPaths.Count - 1);
            }
            
            CachedPaths.Insert(0, selectedPath);
        }

        private string _selectedPath;
        public string SelectedPath
        {
            get
            {
                return _selectedPath;
            }
            set
            {
                if (_selectedPath == value) return;

                _selectedPath = value;
                RaisePropertyChanged(() => SelectedPath);
            }
        }

    }
}
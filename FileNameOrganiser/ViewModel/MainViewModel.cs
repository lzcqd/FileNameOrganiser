using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Ookii.Dialogs.Wpf;
using System.Linq;
using System.Windows;
using System.IO;
using System.Collections.Generic;

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
            FilesDropCommand = new RelayCommand<IDataObject>(OnFilesDrop);
            CachedPaths = new ObservableCollection<string>();
            Files = new ObservableCollection<FileInfo>();
        }

        public RelayCommand FolderOpenClickCommand { get; private set; }

        protected void OnFolderOpenClick()
        {
            VistaFolderBrowserDialog folderDialog = new VistaFolderBrowserDialog();
            folderDialog.ShowDialog();
            AddToSelectedPaths(folderDialog.SelectedPath);
            SelectedPath = folderDialog.SelectedPath;
            GetFiles(SelectedPath);
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


        public ObservableCollection<FileInfo> Files { get; private set; }

        protected void GetFiles(string selectedPath)
        {
            Files.Clear();
            
            DirectoryInfo dir = new DirectoryInfo(selectedPath);

            foreach (var file in dir.GetFiles())
            {
                Files.Add(file);
            }
        }

        public RelayCommand<IDataObject> FilesDropCommand { get; private set; }

        protected void OnFilesDrop(IDataObject data)
        {
            FileInfo[] droppedFiles = GetDropFiles(data);

            if (droppedFiles == null || droppedFiles.Count() <= 0) return;

            foreach (var droppedFile in droppedFiles)
            {
                if (droppedFile.Exists && !Files.Any(f => f.FullName == droppedFile.FullName))
                {
                    Files.Add(droppedFile);
                }
            }

            if (Files.Count() <= 0) return;
            var dirPath = Files[0].Directory.FullName;
            AddToSelectedPaths(dirPath);
            SelectedPath = dirPath;
        }

        private FileInfo[] GetDropFiles(IDataObject data)
        {
            List<FileInfo> ret = new List<FileInfo>();
            string[] droppedFiles = data.GetData(DataFormats.FileDrop, false) as string[];

            if (droppedFiles == null || droppedFiles.Count() <= 0) return null;

            foreach (var droppedFile in droppedFiles)
            {
                ret.Add(new FileInfo(droppedFile));
            }

            return ret.ToArray();
        }

    } 
}
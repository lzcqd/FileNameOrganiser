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
        private DragDropEffects _currEffect;
        private string _replaceStart = "$[", _replaceEnd = "]$";
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
            CachedPaths = new ObservableCollection<string>();
            Files = new ObservableCollection<FileInfo>();
            Logs = new ObservableCollection<string>();
            FolderOpenClickCommand = new RelayCommand(OnFolderOpenClick);
            FilesDropCommand = new RelayCommand<IDataObject>(OnFilesDrop);
            DragOverCommand = new RelayCommand<DragEventArgs>(e =>
            {
                if (!IsFileDrop(e.Data)) return;
                e.Effects = _currEffect;
                e.Handled = true;
            });
            DragEnterCommand = new RelayCommand<IDataObject>(OnDragEnter);
            RenameFileCommand = new RelayCommand(OnRenameFileClick, 
                () => 
                    !string.IsNullOrWhiteSpace(SequenceFileName) && Files.Any());
            Logs.Add("Initialized!");
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


        private string _sequenceFileName;
        public string SequenceFileName
        {
            get
            {
                return _sequenceFileName;
            }
            set
            {
                if (_sequenceFileName == value) return;
                _sequenceFileName = value;
                RaisePropertyChanged(() => SequenceFileName);
                RenameFileCommand.RaiseCanExecuteChanged();
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
            
            Logs.Add(String.Format("{0} files added.", Files.Count()));
        }

        public RelayCommand<IDataObject> FilesDropCommand { get; private set; }

        protected void OnFilesDrop(IDataObject data)
        {
            if (!IsFileDrop(data)) return;

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

            Logs.Add(String.Format("{0} files added.", Files.Count()));
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

        public RelayCommand<DragEventArgs> DragOverCommand { get; private set; }

        public RelayCommand<IDataObject> DragEnterCommand { get; private set; }

        private void OnDragEnter(IDataObject data)
        {
            if (!IsFileDrop(data)) return;

            var draggedFiles = GetDropFiles(data);

            if (draggedFiles != null && draggedFiles.Any(f => !f.Exists))
            {
                _currEffect = DragDropEffects.None;
            }
            else
            {
                _currEffect = DragDropEffects.Link;
            }
        }

        public RelayCommand RenameFileCommand { get; private set; }

        public void OnRenameFileClick()
        {
            Logs.Add("Start renaming...");
            int currentCount = 1;
            FileInfo preivousFile = null;
            foreach (var file in Files.ToList())
            {
                int startIndex = SequenceFileName.IndexOf(_replaceStart) + _replaceStart.Count();
                int endIndex = SequenceFileName.IndexOf(_replaceEnd);
                
                string dynamicString = SequenceFileName.Substring(startIndex, endIndex - startIndex);
                int decimalLength = dynamicString.Count();

                if (preivousFile != null && string.Equals(Path.GetFileNameWithoutExtension(preivousFile.Name),
                    Path.GetFileNameWithoutExtension(file.Name), StringComparison.OrdinalIgnoreCase))
                {
                    currentCount--;
                }

                string newName = SequenceFileName.Replace(_replaceStart + dynamicString + _replaceEnd,
                    currentCount.ToString("D" + decimalLength.ToString()));

                Logs.Add(String.Format("Renaming {0} to {1}", file.Name, newName + file.Extension));

                string newFilePath = Path.Combine(file.DirectoryName, newName + file.Extension);
                File.Move(file.FullName, newFilePath);

                Files.Remove(file);
                Files.Add(new FileInfo(newFilePath));

                preivousFile = file;
                currentCount++;
            }

            Logs.Add("Done!");
        }

        public ObservableCollection<string> Logs { get; private set; }

        private bool IsFileDrop(IDataObject data)
        {
            return data.GetFormats().Contains(DataFormats.FileDrop);
        }

    }
}
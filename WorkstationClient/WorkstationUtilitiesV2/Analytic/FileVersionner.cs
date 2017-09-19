using System;
using System.Collections.Generic;
using System.IO;

namespace WorkstationUtilities.Analytic{
    public enum FileAction{
        Keep = 0,
        Overwrite,
        Merge,
        SelectChanges
    }

    public class FileVersionner : IDisposable
    {

        private FileItem _Original;
        private FileItem _Modified;
        private List<FileComparisonItem> _Changes;

        private FileItem Original {
            get => _Original;
            set => _Original = value;
        }

        private FileItem Modified
        {
            get => _Modified;
            set => _Modified = value;
        }

        public List<FileComparisonItem> Changes
        {
            get => _Changes;
            private set => _Changes = value;
        }

        private string FilePath { get; set; }

        public FileVersionner(string OriginalFile, string NewFile, string Path, string OtherRepository = ""){
            FilePath = Path;
            Original = new FileItem(Path, OriginalFile);
            Modified = new FileItem(Path + OtherRepository, NewFile);
            Changes = new List<FileComparisonItem>();
        }

        public List<FileComparisonItem> AnalyzeFile(){
            Changes.Clear();
            Original.DiscoverFile();
            Modified.DiscoverFile();
            Original.CompareTo_2(ref _Modified, ref _Changes);

            return Changes;
        }

        public void CreateBackup(int index)
        {
            string From = Original.Absolute, To = FilePath + @"backup\" + Original.Filename + "_" + index;
            Directory.CreateDirectory(FilePath + @"backup\");
            File.Copy(From, To);
        }

        public void Dispose()
        {
            _Original?.Dispose();
            _Modified?.Dispose();
            
        }
    }
}

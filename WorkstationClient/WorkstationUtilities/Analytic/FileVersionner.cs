using System;
using System.Collections.Generic;

namespace WorkstationUtilities.Analytic{
    public enum FileAction{
        Keep = 0,
        Overwrite,
        Merge,
        SelectChanges
    }

    public class FileVersionner
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

        private String FilePath { get; set; }

        public FileVersionner(String OriginalFile, String NewFile, String Path){
            FilePath = Path;
            Original = new FileItem(Path, OriginalFile);
            Modified = new FileItem(Path, NewFile);
        }

        public void AnalyzeFile(){
            Changes.Clear();
            Original.DiscoverFile();
            Modified.DiscoverFile();
            Original.CompareTo(ref _Modified, ref _Changes);
        }


        // TODO FINISH COPY
    }
}

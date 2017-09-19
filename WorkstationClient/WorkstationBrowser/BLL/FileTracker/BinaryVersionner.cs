using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using WorkstationClrVersionner;
using WorkstationUtilities.Analytic;

namespace WorkstationBrowser.BLL.FileTracker
{
    public class BinaryVersionner : IDisposable
    {
        //private AnalyzerWrapper wrapper;
        private FileVersionner Versionner; 
        public BinaryVersionner(String Original, String Modified, String SourcePath, String Additional = ""){
            //wrapper = new AnalyzerWrapper(Original, Modified, SourcePath);
            Versionner = new FileVersionner(Original, Modified, SourcePath, Additional);
        }

        public BinaryVersionner(){
          
        }


        public void OpenFiles(){
            //if(!wrapper.IsOpen())
            //    wrapper?.OpenFile();
        }

        public void CloseFiles(){
           //if (wrapper.IsOpen())
           //     wrapper?.CloseFile();
        }

        public void Dispose()
        {
            Versionner.Dispose();
            //wrapper?.Dispose();
        }

        public List<FileComparisonItem> CheckDifferences(){
            return Versionner.AnalyzeFile();
        }

        // public AnalyzeResultList CheckDifferences(){

        //if (!wrapper.IsOpen())
        //    wrapper.OpenFile();
        //return wrapper.AnalyzeFile();
        // }
    }
}
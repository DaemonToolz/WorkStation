using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using WorkstationClrVersionner;

//using WorkstationClrVersionner;

namespace WorkstationBrowser.BLL.FileTracker
{
    public class BinaryVersionner : IDisposable
    {
        //private AnalyzerWrapper wrapper;
        public BinaryVersionner(String Original, String Modified, String SourcePath){
            //wrapper = new AnalyzerWrapper(Original, Modified, SourcePath);
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

        public void Dispose(){
            //wrapper?.Dispose();
        }

       // public AnalyzeResultList CheckDifferences(){
 
            //if (!wrapper.IsOpen())
            //    wrapper.OpenFile();
            //return wrapper.AnalyzeFile();
       // }
    }
}
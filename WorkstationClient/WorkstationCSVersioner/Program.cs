using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkstationClrVersionner;

namespace WorkstationCSVersioner{
    class Program{
        static void Main(string[] args)
        {
            // Testing Values
            AnalyzerWrapper wrapper = new AnalyzerWrapper("a.txt", "b.txt",
                @"C:\Users\macie\Source\Repos\WorkStation\WorkstationClient\Debug\");

            
            wrapper.OpenFile();
            var Result = wrapper.AnalyzeFile();
            wrapper.CloseFile();
            wrapper.Dispose();


            var LineChanges = Result.Results.Where(rec => rec.ChangeType == 0);
            var Addition = Result.Results.Where(rec => rec.ChangeType == 1);
            var Deletion = Result.Results.Where(rec => rec.ChangeType == 2);
            
            foreach (var change in LineChanges){
                for (int i = 0; i < change.Results.Count; ++i){
                    Console.WriteLine($"[~] {change.BeginLine + i} | " + change.Results[i]);
                }
            }


            foreach (var change in Addition)
            {
                for (int i = 0; i < change.Results.Count; ++i)
                {
                    Console.WriteLine($"[+] {change.BeginLine + i} | " + change.Results[i]);
                }
            }

            foreach (var change in Deletion)
            {
                for (int i = 0; i < change.Results.Count; ++i)
                {
                    Console.WriteLine($"[-] {change.BeginLine + i} | " + change.Results[i]);
                }
            }


            Console.ReadKey();
        }
    }
}

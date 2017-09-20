using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkstationUtilities.Analytic;

namespace WorkstationLiveTester{
    class Program{
        static void Main(string[] args){
            /*
            List<FileComparisonItem> results;
            using (var fv = new FileVersionner("Index.html", @"Index.html",
                @"C:\Users\macie\Source\Repos\WorkStation\WorkstationClient\WorkstationBrowser\UserContent\ProjectFiles\Workstation\",
                @"\backup\"))
            {
                results = fv.AnalyzeFile();

            }

            foreach (var result in results)
            {
                Console.WriteLine(result.changeType.ToString());
                Console.WriteLine($"From {result.StartingLine} to {result.EndLine}");
                foreach (var change in result.ChangeSet)
                    Console.WriteLine(change);
            }

    */
            String ProjectName = @"C:\users\macie\documents\workstation\test\2";
            String nameToSave = ProjectName.Substring(ProjectName.IndexOf("workstation"));
            Console.WriteLine(nameToSave);

            Console.ReadKey();
        }
    }
}

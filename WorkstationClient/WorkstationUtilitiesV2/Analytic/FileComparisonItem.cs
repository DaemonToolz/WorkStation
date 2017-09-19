using System;
using System.Collections.Generic;
using System.Text;

namespace WorkstationUtilities.Analytic {
    public enum ChangeType{
        LineChange = 0,
        NewContent,
        RemovedContent
    }

    public class FileComparisonItem {
        public int StartingLine, EndLine, AdditionalLines;
        public ChangeType changeType;

        // Single line change
        public string OriginalLineContent, ModifiedContent;

        public List<string> ChangeSet;

    }
}

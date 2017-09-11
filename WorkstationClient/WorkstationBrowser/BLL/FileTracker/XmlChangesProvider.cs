using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkstationBrowser.BLL.FileTracker
{
    public class XmlChangesProvider : XmlProvider {
        public XmlChangesProvider(string src) : base(src, null) {
        }

        public XmlChangesProvider(string src, string parserType) : base(src, parserType, null) {
        }


    }
}
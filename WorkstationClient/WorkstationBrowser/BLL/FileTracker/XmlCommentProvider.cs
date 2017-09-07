using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkstationBrowser.BLL.FileTracker
{
    public class XmlCommentProvider : XmlProvider {
        public XmlCommentProvider(string src) : base(src){
        }

        public XmlCommentProvider(string src, string parserType) : base(src, parserType){
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using WorkstationBrowser.BLL.FileTracker.Exception;

namespace WorkstationBrowser.BLL.FileTracker {
    public class XmlProvider : IDisposable{
        // REFERENCE https://lennilobel.wordpress.com/2009/09/02/streaming-into-linq-to-xml-using-c-custom-iterators-and-xmlreader/
        // Designed namings: _ft{file}.xml
        #region Reader
        protected XmlReader Reader { get; set; }
        public String SourcePath { get; protected set; }
        protected String FileParser { get; set; }
        protected String TrackedFile { get; set; }

        public readonly static short XDocumentLimit = 10; // Higher than 10Mo, reading as a stream
        #endregion 

        #region InMemory File
        public XDocument OpenedDocument { get; protected set; }
        protected XDocument Rollback { get; set; }
        #endregion

        #region Tags
        protected HashSet<String> CachedTags { get; set; }
        protected Dictionary<int, String[]> KnownNodes { get; set; }
        #endregion

        public XmlProvider(String src) {
            SourcePath = src;
            CachedTags = new HashSet<String>();
        }

        public XmlProvider(String src, String parserType) : this(src){
            FileParser = parserType;
        }

        public XmlProvider(String src, String parserType, String trackedFile) : this(src, parserType)
        {
            FileParser = parserType;
            TrackedFile = trackedFile;
        }


        public XDocument OpenFile(String trackedFile) {
            CloseFile(); // Force disposal here

            Reader = XmlReader.Create(SourcePath + String.Format(FileParser, trackedFile));
            return OpenedDocument = XDocument.Load(Reader);
        }

        public XmlReader OpenFileNoMemory(String trackedFile)
        {
            CloseFile(); // Force disposal here
          
            return Reader = XmlReader.Create(SourcePath + String.Format(FileParser, trackedFile));
        }



        public virtual void CloseFile(){
            Reader?.Close();
            Reader?.Dispose();
            OpenedDocument = Rollback = null;
        }

        public void SaveFile()
        {
            
            OpenedDocument.Save(SourcePath + String.Format(FileParser, TrackedFile));
            Rollback = OpenedDocument;
        }

        public virtual void DeleteNode(String key, String tag, String value){
            try{
                OpenedDocument.Descendants(key)
                    .Where(n => ((string)n.Element(tag)).Equals(value))
                    .Remove();

                SaveFile();
            }
            catch {
                OpenedDocument = Rollback;
            }
        }

        public virtual void StreamedDeleteNode(String key, String tag, String value){
            throw new NotImplementedException();
        }

        public virtual void StreamedAddNode()
        {
            throw new NotImplementedException();
        }

        public virtual void CheckTag(String tag)
        {
            if(!CachedTags.Contains(tag))
                if (!KnownNodes.Values.Any(tagArr => tagArr.Contains(tag)))
                    throw new UnsupportedTagException();

            CachedTags.Add(tag);

        }

        public virtual void AddNode(){
            throw new NotImplementedException();
        }

        public virtual void InsertNode(){
            throw new NotImplementedException();
        }

        public virtual void UpdateNode(){
            throw new NotImplementedException();
        }

        private  IEnumerable<XElement> StreamElements(String tag)
        {
            if (Reader == null) throw new NotSupportedException(); // Atm
            Reader.MoveToContent();
            while (Reader.Read()) 
                if ((Reader.NodeType == XmlNodeType.Element) && (Reader.Name == tag)) 
                    yield return XElement.ReadFrom(Reader) as XElement;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing){
            if (!disposedValue){
                if (disposing)
                {
                    SaveFile();

                    CloseFile();

                    CachedTags.Clear();
                    KnownNodes.Clear();

                    CachedTags = null;
                    KnownNodes = null;
                    TrackedFile = null;
                    FileParser = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~XmlProvider() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion


    }
}
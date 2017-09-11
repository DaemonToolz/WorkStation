using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace WorkstationBrowser.BLL.FileTracker {
    public class XmlProvider : IDisposable{
        // REFERENCE https://lennilobel.wordpress.com/2009/09/02/streaming-into-linq-to-xml-using-c-custom-iterators-and-xmlreader/
        // Designed namings: _ft{file}.xml
        #region Reader
        //protected XmlReader Reader { get; set; }
        public String SourcePath { get; protected set; }
        protected String FileParser { get; set; }
        protected String TrackedFile { get; set; }
        protected String AbsoluteFile => SourcePath + String.Format(FileParser, TrackedFile) + ".xml";


        public readonly static short XDocumentLimit = 10; // Higher than 10Mo, reading as a stream
        private int _Index = 0;
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
            TrackedFile = trackedFile;
        }

        public XmlProvider(String src, String parserType, String trackedFile, Dictionary<int, String[]> tags) : this(src, parserType, trackedFile)
        {
            KnownNodes = tags;
        }

        public XDocument OpenFile(String trackedFile = null) {
            CloseFile(); // Force disposal here

            if (trackedFile != null) //Reader = XmlReader.Create(SourcePath + String.Format(FileParser, trackedFile));
                TrackedFile = trackedFile;
          
            if (!File.Exists(AbsoluteFile))
                return OpenedDocument = new XDocument();
            return OpenedDocument = XDocument.Load(AbsoluteFile);//OpenedDocument = XDocument.Load(Reader);    
        }
        /*
        public XmlReader OpenFileNoMemory(String trackedFile)
        {
            CloseFile(); // Force disposal here
          
            return Reader = XmlReader.Create(SourcePath + String.Format(FileParser, trackedFile));
        }
        */


        public virtual void CloseFile(){
            //Reader?.Close();
            //Reader?.Dispose();
            OpenedDocument = Rollback = null;
        }

        public void SaveFile()
        {
            
            OpenedDocument.Save(SourcePath + String.Format(FileParser, TrackedFile) + ".xml");
            Rollback = OpenedDocument;
        }

        public virtual bool DeleteNode(String key, String tag, String value){
            try{
                OpenedDocument.Descendants(key)
                    .Where(n => ((string)n.Element(tag)).Equals(value))
                    .Remove();

                SaveFile();
                return true;
            }
            catch {
                OpenedDocument = Rollback;
                return false;
            }
        }

        public virtual bool AddNode(String key, params XmlElementProvider[] data)
        {
            try{

                Console.WriteLine("Preparing the data");
                List<XElement> xmlData = new List<XElement>();


                xmlData.AddRange(
                    data.Select(XmlElementProvider.Recursive_ConvertToXElement)
                );


                Console.WriteLine("Adding to the xml doc");
                if (key == null)
                {
                    Console.WriteLine("Key is null");
                    if (OpenedDocument.Root == null)
                    {

                        Console.WriteLine("Adding a root");
                        OpenedDocument.Add(new XElement("root", OpenedDocument.Root));
                    }
                    Console.WriteLine("Adding the data");
                    OpenedDocument.Root.Add(xmlData);
                }
                else
                {
                    if (OpenedDocument.Root == null)
                        OpenedDocument.Add(new XElement("root", OpenedDocument.Root));
                    

                    if (!OpenedDocument.Descendants(key).Any())
                        OpenedDocument.Root.Add(new XElement(key, xmlData));
                    else 
                        OpenedDocument.Descendants(key).FirstOrDefault().Add(xmlData);
                }

                SaveFile();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                OpenedDocument = Rollback;
                return false;
            }
        }

        public virtual bool InsertNode(String Id, String key, params XmlElementProvider[] data)
        {
            try
            {
                
                (OpenedDocument.Descendants(key))
                    .SingleOrDefault(idAttr => ((String)idAttr.Attribute("id")).Equals(Id))?
                    .AddAfterSelf(data.Select(XmlElementProvider.Recursive_ConvertToXElement));

                SaveFile();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                OpenedDocument = Rollback;
                return false;
            }
        }

        public virtual bool DeleteFile()
        {
            try
            {
                CloseFile();
                File.Delete(AbsoluteFile);
                return true;
            }
            catch {
                return false;
            }
        }

        public virtual IEnumerable<bool> UpdateMultipleNodes(String Id, String Key, Dictionary<String,String> data, bool LineChange = false)
        {
            var result = data.Select(pair => UpdateNode(Id, Key, pair.Key, pair.Value, LineChange));
            if(!LineChange)
                SaveFile();
            return result;
        }

        public virtual bool UpdateNode(String Id, String Key, String Name, String NewValue, bool SaveChanges = true)
        {
            try
            {

                var MyNode = (OpenedDocument.Descendants(Key)).SingleOrDefault(idAttr => ((String)idAttr.Attribute("id")).Equals(Id));
                MyNode.Element(Name).Value = NewValue;

                if(SaveChanges)
                    SaveFile();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                OpenedDocument = Rollback;
                return false;
            }
        }

        public virtual bool DeleteNode(String Id, String Key)
        {
            try
            {
                (OpenedDocument.Descendants(Key)).Where(idAttr => ((String)idAttr.Attribute("id")).Equals(Id)).Remove();
                
                SaveFile();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                OpenedDocument = Rollback;
                return false;
            }
        }

        public virtual void CheckTag(String tag)
        {
            if(!CachedTags.Contains(tag))
                if (!KnownNodes.Values.Any(tagArr => tagArr.Contains(tag)))
                    throw new Exception("Unsupported");

            CachedTags.Add(tag);

        }

       

        /*
        private  IEnumerable<XElement> StreamElements(String tag)
        {
            if (Reader == null) throw new NotSupportedException(); // Atm
            Reader.MoveToContent();
            while (Reader.Read()) 
                if ((Reader.NodeType == XmlNodeType.Element) && (Reader.Name == tag)) 
                    yield return XElement.ReadFrom(Reader) as XElement;
        }
        */
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
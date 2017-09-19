using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using WorkstationBrowser.BLL.FileTracker.Exception;
using WorkstationBrowser.SessionReference;

namespace WorkstationBrowser.BLL.FileTracker
{

    // Model possibly deprecated
    // https://stackoverflow.com/questions/31692333/multiple-users-writing-at-the-same-file
    // For real-time multiple I/O
    public class XmlProvider : IDisposable
    {
        // REFERENCE https://lennilobel.wordpress.com/2009/09/02/streaming-into-linq-to-xml-using-c-custom-iterators-and-xmlreader/
        // Designed namings: _ft{file}.xml
        #region Reader
        //protected XmlReader Reader { get; set; }
        public String SourcePath { get; protected set; }
        protected String FileParser { get; set; }
        protected String TrackedFile { get; set; }
        protected String AbsoluteFile => SourcePath + String.Format(FileParser, TrackedFile) + ".xml";

        // NOT IMPLEMENTED YET
        public readonly static short XDocumentLimit = 10; // Higher than 10Mo, reading as a stream

        private static readonly 
            ConcurrentDictionary<String, XmlSlimLock> _Locks =  new ConcurrentDictionary<String, XmlSlimLock>();
        #endregion 

        #region InMemory File
        public XDocument OpenedDocument { get; protected set; }
        protected XDocument Rollback { get; set; }
        protected UsersModel CurrentUser { get; set; }
        #endregion

        #region Tags
        protected HashSet<String> CachedTags { get; set; }
        protected Dictionary<int, String[]> KnownNodes { get; set; }
        #endregion

        public XmlProvider(String src, UsersModel user)
        {
            CurrentUser = user;
            SourcePath = src;
            CachedTags = new HashSet<String>();
        }

        public XmlProvider(String src, String parserType, UsersModel user) : this(src, user)
        {
            FileParser = parserType;
        }

        public XmlProvider(String src, String parserType, String trackedFile, UsersModel user) : this(src, parserType, user)
        {
            TrackedFile = trackedFile;
        }

        public XmlProvider(String src, String parserType, String trackedFile, Dictionary<int, String[]> tags, UsersModel user) : this(src, parserType, trackedFile, user)
        {
            KnownNodes = tags;
        }

        public XDocument OpenFile(String trackedFile = null)
        {
            CloseFile(); // Force disposal here

            if (trackedFile != null) //Reader = XmlReader.Create(SourcePath + String.Format(FileParser, trackedFile));
                TrackedFile = trackedFile;

            if (!_Locks.ContainsKey(AbsoluteFile))
                _Locks.TryAdd(AbsoluteFile, new XmlSlimLock());
            

            if(_Locks.ContainsKey(AbsoluteFile))
                _Locks[AbsoluteFile].AddUser(CurrentUser);

            if (!File.Exists(AbsoluteFile))
            {
                OpenedDocument = new XDocument();
                AddNode(null);
                return OpenedDocument;
            }
            return OpenedDocument = XDocument.Load(AbsoluteFile);//OpenedDocument = XDocument.Load(Reader);    
        }
        /*
        public XmlReader OpenFileNoMemory(String trackedFile)
        {
            CloseFile(); // Force disposal here
          
            return Reader = XmlReader.Create(SourcePath + String.Format(FileParser, trackedFile));
        }
        */

        public virtual void CloseFile() {
            //Reader?.Close();
            //Reader?.Dispose();
            if (_Locks.ContainsKey(AbsoluteFile))
            {
                _Locks[AbsoluteFile].RemoveUser((int) CurrentUser.id);
                if (_Locks[AbsoluteFile].ConcurrentAccess == 0)
                {
                    var xmlSlimLock = new XmlSlimLock();
                    _Locks.TryRemove(AbsoluteFile, out xmlSlimLock);

                }
            }

            OpenedDocument = Rollback = null;
        }

        public void SaveFile()
        {

            OpenedDocument.Save(AbsoluteFile);
            Rollback = OpenedDocument;
        }

        public virtual bool DeleteNode(String key, String tag, String value)
        {
            try
            {
                OpenedDocument.Descendants(key)
                    .Where(n => ((string)n.Element(tag)).Equals(value))
                    .Remove();

                SaveFile();
                return true;
            }
            catch
            {
                OpenedDocument = Rollback;
                return false;
            }
        }

        public virtual bool AddNode(String key, params XmlElementProvider[] data)
        {
            try
            {
                List<XElement> xmlData = new List<XElement>();

                if(data.Any())
                    xmlData.AddRange(
                        data.Select(XmlElementProvider.Recursive_ConvertToXElement)
                    );

                if (OpenedDocument.Root == null)
                    OpenedDocument.Add(new XElement("root", OpenedDocument.Root));

                SaveFile();

                if (key == null)
                {
                    OpenedDocument.Root.Add(xmlData);
                }
                else
                {
                 
                    if (!OpenedDocument.Descendants(key).Any())
                        OpenedDocument.Root.Add(new XElement(key, xmlData));
                    else
                        OpenedDocument.Descendants(key).FirstOrDefault().Add(xmlData);
                }

                SaveFile();
                return true;
            }
            catch 
            {
                OpenedDocument = Rollback;
                return false;
            }
        }

        public virtual List<UsersModel> ActiveUsers()
        {
            XmlSlimLock lockedData;
            _Locks.TryGetValue(AbsoluteFile, out lockedData);
            return lockedData.Users.ToList();
        }

        public virtual IEnumerable<XmlElementProvider> ReadNode(String key)
        {
            if (OpenedDocument?.Root == null) return new List<XmlElementProvider>();
            var comments = OpenedDocument.Descendants(key).Elements();
            

            return !comments.Any() ? 
                new List<XmlElementProvider>() :
                comments.Select(node => XmlElementProvider.Recursive_XmlElementProvider(node));
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
            catch
            {
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
            catch
            {
                return false;
            }
        }

        public virtual IEnumerable<XContainer> GetNodes(String key){
            try {
                return OpenedDocument.Elements(key);
            }
            catch
            {
                return new List<XContainer>();
            }
        }

        public virtual IEnumerable<bool> UpdateMultipleNodes(String Id, String Key, Dictionary<String, String> data, bool LineChange = false)
        {
            var result = data.Select(pair => UpdateNode(Id, Key, pair.Key, pair.Value, LineChange));
            if (!LineChange)
                SaveFile();
            return result;
        }

        public virtual bool UpdateNode(String Id, String Key, String Name, String NewValue, bool SaveChanges = true)
        {
            try
            {

                var MyNode = (OpenedDocument.Descendants(Key)).SingleOrDefault(idAttr => ((String)idAttr.Attribute("id")).Equals(Id));
                MyNode.Element(Name).Value = NewValue;

                if (SaveChanges)
                    SaveFile();
                return true;
            }
            catch 
            {
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
            catch {
                OpenedDocument = Rollback;
                return false;
            }
        }

        public virtual void CheckTag(String tag)
        {
            if (!CachedTags.Contains(tag))
                if (!KnownNodes.Values.Any(tagArr => tagArr.Contains(tag)))
                    throw new UnsupportedTagException();

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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
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
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion


        public bool Equals(XmlProvider provider){

            try
            {
                return CurrentUser.id == provider.CurrentUser.id &&
                       AbsoluteFile.Equals(provider.AbsoluteFile) &&
                       DictionaryEqual(KnownNodes, provider.KnownNodes);
            }
            catch
            {
                return false;
            }
        }

        protected static bool DictionaryEqual<TKey, TValue>( 
            IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second)
        {
            return DictionaryEqual(first, second, null);
        }

        protected static bool DictionaryEqual<TKey, TValue>(
            IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second,
            IEqualityComparer<TValue> valueComparer)
        {
            if (first == second) return true;
            if ((first == null) || (second == null)) return false;
            if (first.Count != second.Count) return false;

            valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;

            foreach (var kvp in first)
            {
                TValue secondValue;

                if (!second.TryGetValue(kvp.Key, out secondValue)) return false;
                if (!secondValue.GetType().IsArray && !valueComparer.Equals(kvp.Value, secondValue)) return false;
            }
            return true;
        }
    }
}
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkstationBrowser.BLL.FileTracker;
using System.Xml.Linq;

namespace WorkstationBrowser.Tests {
    [TestClass]
    public class XmlModifierUnitTest {
        XmlProvider xmlProvider;

        [TestInitialize]
        public void Initialize()
        {
            xmlProvider = new XmlProvider(@"C:\Users\macie\Source\Repos\WorkStation\WorkstationClient\WorkstationBrowser.Tests\bin\Debug\", 
                "_ft{0}", "test", 
                new System.Collections.Generic.Dictionary<int, string[]>()
                {
                    {0,new string[]{"root"}},
                    { 1, new String[]{"comments", "version"}},
                    { 2, new String[]{"comment"}},
                    { 3, new String[]{"author", "date", "content"}}
                });
            xmlProvider.OpenFile();
        }

        [TestMethod]
        public void TestXmlWrite()
        {
            List<XmlElementProvider> elements = new List<XmlElementProvider>()
            {
                new XmlElementProvider(){
                    Key = "comment",
                    Value = "",
                    Attributes = new List<XAttribute>(){ new XAttribute("id", "415sd89")},

                    Children =  new List<XmlElementProvider>(){
                                    new XmlElementProvider(){
                                        Key = "author", Value = "Pierre",
                                    },

                                    new XmlElementProvider()
                                    {
                                        Key = "date", Value = DateTime.Now
                                    },
                                    new XmlElementProvider(){
                                        Key = "content", Value = "Salut Paul!",
                                    }
                                }
                    
                },
                new XmlElementProvider(){
                    Key = "comment",
                    Value = "",
                    Attributes = new List<XAttribute>(){ new XAttribute("id", "415sdsc")},

                    Children =  new List<XmlElementProvider>(){
                        new XmlElementProvider(){
                            Key = "author", Value = "Paul",
                        },
                        new XmlElementProvider()
                        {
                            Key = "date", Value = DateTime.Now.Add(new TimeSpan(0,0,20)).ToString()
                        },
                        new XmlElementProvider(){
                            Key = "content", Value = "Salut Pierre!",
                        }
                    }

                }

            };


            Assert.IsTrue(xmlProvider.AddNode("comments", elements.ToArray()));
            var MultipleUpdates = xmlProvider.UpdateMultipleNodes(
                "415sd89",
                "comment",
                new Dictionary<string, string>()
                {
                    {"content", "Oh Hi Mark!"},
                    {"date", DateTime.Now.Add(new TimeSpan(1, 50, 30)).ToString()}
                }, true);

            foreach (var b in MultipleUpdates)
                Assert.IsTrue(b);

            Assert.IsTrue(xmlProvider.InsertNode("415sd89", "comment", elements.ToArray()));
            Assert.IsTrue(xmlProvider.DeleteNode("415sd89", "comment"));
            
            //Assert.IsTrue(xmlProvider.DeleteFile());
        }

    }
}

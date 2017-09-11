using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using WorkstationBrowser.Controllers.Generic;
using WorkstationBrowser.Models;
using WorkstationBrowser.SessionReference;

namespace WorkstationBrowser.BLL.FileTracker
{
    public class XmlCommentProvider : XmlProvider {
        public XmlCommentProvider(string src, UsersModel user) : base(src, user)
        {
        }


        public XmlCommentProvider(String src, String trackedFile, UsersModel user) : base(src, user)
        {
            FileParser = "_comtrack{0}";
            TrackedFile = trackedFile;
            KnownNodes = new Dictionary<int, string[]>() {
                    { 0, new string[]{"root"}},
                    { 1, new String[]{"comments", "version"}},
                    { 2, new String[]{"comment", "version_data"}},
                    { 3, new String[]{"author", "date", "content", "updatetime", "changeauthor"}}
            };
        }

        public bool AddNode(CommentModel model)
        {

            var newComment = new XmlElementProvider()
            {
                Key = "comment",
                Value = "",
                Attributes = new List<XAttribute>() {new XAttribute("id", Guid.NewGuid().ToString())},

                Children = new List<XmlElementProvider>()
                {
                    new XmlElementProvider()
                    {
                        Key = "author",
                        Value = model.Author.username,
                    },

                    new XmlElementProvider()
                    {
                        Key = "date",
                        Value = model.Date.ToString()
                    },
                    new XmlElementProvider()
                    {
                        Key = "content",
                        Value = model.Content,
                    }
                }

            };

            return AddNode("comments", newComment);
        }

        public IEnumerable<CommentModel> ReadComments(){
      
            if(ReadNode("comments").Any())
                return ReadNode("comments").Reverse().Select(comment => new CommentModel(){
                        Id = comment.Attributes.Single(attr => attr.Name.ToString() == "id").Value,
                        AuthorName = comment.Children[0].Value.ToString(),
                        Date = Convert.ToDateTime(comment.Children[1].Value.ToString()),
                        Content = comment.Children[2].Value.ToString()

                    });
          
            return new List<CommentModel>();
            
        }

    }
}
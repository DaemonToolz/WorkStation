using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using WorkstationBrowser.SessionReference;

namespace WorkstationBrowser.BLL.FileTracker {
    public class XmlSlimLock {
        public ReaderWriterLockSlim Lock { get; set; }
        private int _ConcurrentAccess;

        public XmlConcurrentList<UsersModel> Users { get; set; }

        public int ConcurrentAccess => _ConcurrentAccess;

        public XmlSlimLock()
        {
            Lock = new ReaderWriterLockSlim();
            Users = new XmlConcurrentList<UsersModel>();
            _ConcurrentAccess = 0;
        }


        public void AddUser(UsersModel newUser){
            if (Users.Find(usr => usr.id == newUser.id) != null) return;
            Users.Add(newUser);
            Interlocked.Increment(ref _ConcurrentAccess);
        }

        public void RemoveUser(int id)
        {
            if (Users.Find(usr => usr.id == id) == null) return;
            Users.Remove(model => model.id == id);
            Interlocked.Decrement(ref _ConcurrentAccess);
        }

    }
}
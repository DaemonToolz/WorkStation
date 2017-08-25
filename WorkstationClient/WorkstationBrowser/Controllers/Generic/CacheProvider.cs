using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading;

namespace WorkstationBrowser.Controllers.Generic {
    public enum CachePriority
    {
        Default,
        NotRemovable
    }
    public class CacheProvider {
        /*
         * No Double-lock check intended in first place.
         * Might come in the future
         */

        private static readonly ReaderWriterLockSlim _Lock = new ReaderWriterLockSlim();
        private static readonly ObjectCache _Cache = MemoryCache.Default;

      

        #region Generic Caching Model
        public void Set(String Key, Object Item, CachePriority cacheItemPriority) {
                // 
            var _Policy = new CacheItemPolicy{
                Priority = (cacheItemPriority == CachePriority.Default)
                    ? CacheItemPriority.Default
                    : CacheItemPriority.NotRemovable,
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(10.00)
            };

            _Lock.EnterWriteLock();
            try {
                _Cache.Set(Key, Item, _Policy);
            }
            finally {
                _Lock.ExitWriteLock();
            }
        }

        public Object Get(String Key) {
            Object retrieved;
            _Lock.EnterReadLock();
            try {
                retrieved = _Cache.Get(Key, null) ;
            } finally {
                _Lock.ExitReadLock();
            }
            return retrieved;
        }

        public void Remove(String Key){
            _Lock.EnterWriteLock();
            try {
                if (_Cache.Contains(Key)) 
                    _Cache.Remove(Key);
            } finally {
                _Lock.ExitWriteLock();
            }
        }

        #endregion
        
    }
    
}
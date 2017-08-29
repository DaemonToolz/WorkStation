using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using WorkstationBrowser.SessionReference;

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

        
        public IEnumerable<T> GetAll<T>(String Key, Func<T[]> alternative, CachePriority priority = CachePriority.Default){
            var cached = Get(Key);
            if (cached == null)
                Set(Key, alternative.Invoke(), priority);
            return ((T[])Get(Key)).ToList();
        }

        public bool Edit<T>(String Key, Func<IEnumerable<T>> fetcher, Func<T, bool> editFunc, T instance) where T:GenericModel
        {
            if (!editFunc.Invoke(instance)) return false;
            var all = fetcher.Invoke();
            var allModels = all as IList<T> ?? all.ToList();
            allModels.Remove(allModels.Single(model => model.id == instance.id));
            allModels.Add(instance);
            Set(Key, allModels.ToArray(), CachePriority.Default);
            return true;
        }
      

        public bool Delete<T>(String Key, Func<T, bool> updater, Func<IEnumerable<T>> fetcher, T instance,
            bool otherStep = false, Action<T> nextStep = null) where T:GenericModel
        {
            if (!updater.Invoke(instance)) return false;
            var allModels = fetcher.Invoke().ToList();
            allModels.Remove(allModels.Single(model => model.id == instance.id));
            Set(Key, allModels.ToArray(), CachePriority.Default);
            
            if (otherStep){
                nextStep?.Invoke(instance);
            }

            return true;
        }

        public bool CrossDelete<T>(String Key, Func<T, bool> updater, 
            Func<object, IEnumerable<T>> fetcher, 
            Type paramCastType, String fieldName, T instance,
            bool otherStep = false, Action<object> nextStep = null, 
            Type stepParamCast = null, String stepFieldName = null) where T:GenericModel
        {

            if (!updater.Invoke(instance)) return false;

            var parameter = typeof(T).GetField(fieldName).GetValue(instance);
            var value = Convert.ChangeType(parameter, paramCastType);

            var allModels = fetcher.Invoke(value).ToList();
            allModels.Remove(allModels.Single(model => model.id == instance.id));
            Set(Key, allModels.ToArray(), CachePriority.Default);
            
            if (otherStep && nextStep != null && stepParamCast != null && stepFieldName != null){

                var sparameter = typeof(T).GetField(stepFieldName).GetValue(instance);
                var svalue = Convert.ChangeType(sparameter, stepParamCast);

                nextStep.Invoke(svalue);
            }

            return true;
        }

        public bool CrossDelete<T, TP1>(String Key, 
            Func<TP1, IEnumerable<T>> fetcher,
            String fieldName, T instance,
            Func<T, bool> updater = null,
            bool otherStep = false, Func<TP1, bool> nextStep = null,
            String stepFieldName = null) where T : GenericModel
        {

            if(updater != null)
                if (!updater.Invoke(instance)) return false;
            var parameter = typeof(T).GetField(fieldName).GetValue(instance);

            var allModels = fetcher.Invoke((TP1)parameter).ToList();
            allModels.Remove(allModels.Single(model => model.id == instance.id));
            Set(Key, allModels.ToArray(), CachePriority.Default);

            if (otherStep && nextStep != null  && stepFieldName != null)
            {
                var sparameter = typeof(T).GetField(stepFieldName).GetValue(instance);
                nextStep.Invoke((TP1)sparameter);
            }

            return true;
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
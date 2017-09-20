using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using System.Web.Caching;
using WorkstationBrowser.SessionReference;
using CacheItemPriority = System.Runtime.Caching.CacheItemPriority;

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

        public void Set(String Key, Object Item, CachePriority cacheItemPriority, Action<CacheEntryRemovedArguments> callback = null, double Minutes = 10.00) {
                // 
            var _Policy = new CacheItemPolicy{
               
                Priority = (cacheItemPriority == CachePriority.Default)
                    ? CacheItemPriority.Default
                    : CacheItemPriority.NotRemovable,
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(Minutes)
            };

            if (callback != null)
                _Policy.RemovedCallback = new CacheEntryRemovedCallback(callback);

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

        public bool HasKey(String key)
        {
            return _Cache.Contains(key);
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


        public bool EditReflection<T, TInput>(String Key, Func<TInput, IEnumerable<T>> fetcher, Func<T, bool> editFunc, T instance, TInput Input, String FieldName)
        {
            if (!editFunc.Invoke(instance)) return false;
            var all = fetcher.Invoke(Input);
            var allModels = all as IList<T> ?? all.ToList();


            var field = instance.GetType().GetProperty(FieldName);

            allModels.Remove(allModels.Single(model => field.GetValue(model).Equals(field.GetValue(instance))));
            allModels.Add(instance);
            Set(Key, allModels.ToArray(), CachePriority.Default);
            return true;
        }

        public bool Add<T>(String Key, Func<T,T> baseUpdater, Func<IEnumerable<T>> fetcher, T instance) 
        {
            var result = baseUpdater.Invoke(instance);
            if (result == null) return false;

            var all = fetcher.Invoke();
            var allModels = all as IList<T> ?? all.ToList();
            allModels.Add(result);
            Set(Key, allModels.ToArray(), CachePriority.Default);

            return true;
        }

        public bool Add<T,Input>(String Key, Func<T, T> baseUpdater, Func<Input, IEnumerable<T>> fetcher, T instance, Input id)
        {
            var result = baseUpdater.Invoke(instance);
            if (result == null) return false;

            var all = fetcher.Invoke(id);
            var allModels = all as IList<T> ?? all.ToList();
            allModels.Add(result);
            Set(Key, allModels.ToArray(), CachePriority.Default);

            return true;
        }



        public bool CrossAdd<T>(String Key1, String Key2, Func<T, T> baseUpdater, 
            Func<int, IEnumerable<T>> fetcher1, Func<int, IEnumerable<T>> fetcher2, T instance, int param1, int param2, bool crossCondition) where T:GenericModel
        {
            var result = baseUpdater.Invoke(instance);
            if (result == null) return false;

            var all = fetcher1.Invoke(param1);
            var allModels = all as IList<T> ?? all.ToList();
            allModels.Add(result);
            Set(Key1, allModels.ToArray(), CachePriority.Default);

            if (crossCondition)
            {
                all = fetcher2.Invoke(param2);
                allModels = all as IList<T> ?? all.ToList();
                allModels.Add(result);
                Set(Key2, allModels.ToArray(), CachePriority.Default);
            }

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

        public bool DeleteReflection<T, TInput>(String Key, Func<T, bool> updater, Func<TInput, IEnumerable<T>> fetcher, T instance, TInput input, String FieldName){
            if (!updater.Invoke(instance)) return false;
            var allModels = fetcher.Invoke(input).ToList();

            var field = instance.GetType().GetProperty(FieldName);

            allModels.Remove(allModels.Single(model => field.GetValue(model).Equals(field.GetValue(instance))));
            Set(Key, allModels.ToArray(), CachePriority.Default);

            return true;
        }


        #region TODO
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
        #endregion

        public Object Remove(String Key)
        {
            Object retrieved = null;
            _Lock.EnterWriteLock();
            try {
                if (_Cache.Contains(Key))
                    retrieved = _Cache.Remove(Key);
            } finally {
                _Lock.ExitWriteLock();
            }
            return retrieved;
        }

        #endregion
        
    }
    
}
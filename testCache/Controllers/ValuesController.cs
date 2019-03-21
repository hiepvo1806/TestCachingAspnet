using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading;
using System.Web.Http;

namespace testCache.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            var callTime = DateTime.Now;
            Thread.Sleep(5000);
            return new [] { $" call time : {callTime}, afterTime: {DateTime.Now}" };
        }

        public string Get(string key)
        {
            var callTime = DateTime.Now;
            var cachedValue = GetValueTypeCache<int?>(key);
            if (cachedValue != null)
            {
                return $"Cached with key {key} found with value : {cachedValue}";
            }

            Thread.Sleep(5000);
            var saveVal = DateTime.Now.Second;
            SetCache(key, saveVal);
            return $" call time : {callTime}, afterTime: {DateTime.Now}, Key: {key},value: {saveVal}";
        }

        private T GetCache<T>(string cacheKey) where T: class, new() 
        {
            ObjectCache cache = MemoryCache.Default;
            T obj = cache[cacheKey] as T;
            return obj;
        }

        private T GetValueTypeCache<T>(string cacheKey)  
        {
            ObjectCache cache = MemoryCache.Default;
            T obj = (T) cache[cacheKey];
            return obj;
        }

        private void SetCache<T>(string cacheKey, T cacheObj)
        {
            ObjectCache cache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(1);
            cache.Set(cacheKey, cacheObj, policy);
        }

    }
}

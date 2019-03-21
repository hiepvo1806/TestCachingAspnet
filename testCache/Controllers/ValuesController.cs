using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading;
using System.Web.Http;
using Newtonsoft.Json;

namespace testCache.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            var callTime = DateTime.Now;
            return new [] { $" call time : {callTime}, afterTime: {DateTime.Now}" };
        }

        [Route("GetValueType")]
        public string GetValueType(string key)
        {
            var callTime = DateTime.Now;
            var cachedValue = CacheService.GetValueTypeCache<int?>(key);
            if (cachedValue != null)
            {
                return $"Cached Value with key {key} found with value : {cachedValue}";
            }

            Thread.Sleep(5000);
            var saveVal = DateTime.Now.Second;
            CacheService.SetCache(key, saveVal);
            return $" call time : {callTime}, afterTime: {DateTime.Now}, Key: {key},value: {saveVal}";
        }

        [Route("GetObjectType")]
        public string GetObjectType(string key)
        {
            var callTime = DateTime.Now;
            var cachedValue = CacheService.GetObjectCache< TestSaveCacheObject>(key);
            if (cachedValue != null)
            {
                return $"Cached Object with key {key} found with value : { JsonConvert.SerializeObject(cachedValue) }";
            }

            Thread.Sleep(5000);
            var saveObj = new TestSaveCacheObject()
            {
                EndDate = DateTime.Now,
                StartDate = callTime,
                Value = DateTime.Now.Second,
                Name = "cached obj Jeff"
            };

            CacheService.SetCache(key, saveObj);
            return $" call time : {callTime}, afterTime: {DateTime.Now}, Key: {key},value: {JsonConvert.SerializeObject(saveObj)}";
        }
    }

    public static class CacheService {
        public static T GetObjectCache<T>(string cacheKey) where T : class
        {
            ObjectCache cache = MemoryCache.Default;
            T obj = cache[cacheKey] as T;
            return obj;
        }

        public static T GetValueTypeCache<T>(string cacheKey)
        {
            ObjectCache cache = MemoryCache.Default;
            T obj = (T)cache[cacheKey];
            return obj;
        }

        public static void SetCache<T>(string cacheKey, T cacheObj)
        {
            ObjectCache cache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(1);
            cache.Set(cacheKey, cacheObj, policy);
        }
    }

    public class TestSaveCacheObject
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Value { get; set; }
    }
}

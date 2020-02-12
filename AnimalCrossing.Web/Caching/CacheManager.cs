using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace AnimalCrossing.Web.Caching
{
    public class CacheManager
    {
        private IDistributedCache _cache;

        public CacheManager(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T> Get<T>(string key)
        {
            var json = await _cache.GetStringAsync(key);

            if (!String.IsNullOrWhiteSpace(json))
                return JsonSerializer.Deserialize<T>(json);

            return default(T);
        }

        public async Task Set<T>(string key, T value, TimeSpan? slidingExpiration = null, DateTimeOffset? absoluteExpiration = null)
        {
            if (value is null)
                throw new ArgumentNullException("value");

            var json = JsonSerializer.Serialize(value);

            await _cache.SetStringAsync(key, json, new DistributedCacheEntryOptions() { SlidingExpiration = slidingExpiration, AbsoluteExpiration = absoluteExpiration });
        }

        public async Task Remove(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace AOPDemo.Caching
{
    public class MemoryCacheProvider : ICacheProvider
    {
        private readonly ConcurrentDictionary<string, Task<Tuple<DateTime, object>>> _cache;
        public MemoryCacheProvider()
        {
            _cache = new ConcurrentDictionary<string, Task<Tuple<DateTime, object>>>();
        }
        public async Task<T> Get<T>(string key)
        {
            Task<Tuple<DateTime, object>> outValue;
            if (_cache.TryGetValue(key, out outValue))
            {
                return (T)(await outValue).Item2;
            }

            return default(T);            
        }

        public async Task<T> Set<T>(string key, Func<Task<T>> cacheable, TimeSpan duration)
        {
            var result = await _cache.AddOrUpdate(key,
                async (k) =>
                {
                    var cacheableItem = await cacheable();
                    return Tuple.Create(DateTime.UtcNow.AddMilliseconds(duration.Milliseconds), (object)cacheableItem);
                },
                async (k, cache) =>
                {
                    var cachedItem = await cache;
                    if (cachedItem.Item1 >= DateTime.Now)
                    {
                        return cachedItem;
                    }
                    var cacheableItem = await cacheable();
                    return Tuple.Create(DateTime.UtcNow.AddMilliseconds(duration.Milliseconds), (object)cacheableItem);
                }
                );

            return (T)result.Item2;
        }
    }
}
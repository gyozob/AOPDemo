using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace AOPDemo.Caching
{
    public class MemoryCacheProvider : ICacheProvider
    {
        private static readonly ConcurrentDictionary<string, Tuple<DateTime, object>> _cache = new ConcurrentDictionary<string, Tuple<DateTime, object>>();
        public MemoryCacheProvider()
        {
        }
        public object Get(string key)
        {
            Tuple<DateTime, object> outValue;
            if (_cache.TryGetValue(key, out outValue))
            {
                return outValue.Item2;
            }

            return null;            
        }

        public object Set(string key, object cacheable, TimeSpan duration)
        {
            var result = _cache.AddOrUpdate(key,
                (k) =>
                {
                    var cacheableItem = cacheable;
                    return Tuple.Create(DateTime.UtcNow.AddMilliseconds(duration.Milliseconds), cacheableItem);
                },
                (k, cache) =>
                {
                    var cachedItem = cache;
                    if (cachedItem.Item1 >= DateTime.Now)
                    {
                        return cachedItem;
                    }
                    var cacheableItem = cacheable;
                    return Tuple.Create(DateTime.UtcNow.AddMilliseconds(duration.Milliseconds), cacheableItem);
                }
                );

            return result.Item2;
        }
    }
}
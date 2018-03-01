using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace AOPDemo.Caching
{
    public class MemoryCacheProvider : ICacheProvider
    {
        private readonly ConcurrentDictionary<string, Task<object>> _cache;
        public MemoryCacheProvider()
        {
            _cache = new ConcurrentDictionary<string, Task<object>>();
        }
        public async Task<T> GetOrSet<T>(string key, Func<Task<T>> cacheable)
        {
            return (T)await _cache.AddOrUpdate(key,
                async (k) =>
                {
                    return await cacheable();
                },
                async (k, r) =>
                {
                    return await cacheable();
                }
                );
        }
    }
}
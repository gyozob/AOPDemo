using System;
using System.Threading.Tasks;

namespace AOPDemo.Caching
{
    public interface ICacheProvider
    {
        Task<T> GetOrSet<T>(string key, Func<Task<T>> cacheable);
    }
}
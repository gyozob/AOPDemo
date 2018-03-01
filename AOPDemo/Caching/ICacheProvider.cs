using System;
using System.Threading.Tasks;

namespace AOPDemo.Caching
{
    public interface ICacheProvider
    {
        Task<T>  Get<T>(string key);
        Task<T> Set<T>(string key, Func<Task<T>> cacheable, TimeSpan duration);
    }
}
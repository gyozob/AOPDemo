using System;

namespace AOPDemo.Caching
{
    public interface ICacheProvider
    {
        object  Get(string key);
        object Set(string key, object cacheable, TimeSpan duration);
    }
}
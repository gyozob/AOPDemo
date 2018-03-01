using System;
using Unity.Interception.PolicyInjection.Pipeline;

namespace AOPDemo.Attributes
{
    public interface ICacheAttributeCallHandler : ICallHandler
    {
        ICacheAttributeCallHandler SetCacheKeyPattern(string cacheKey);
        ICacheAttributeCallHandler SetDuration(TimeSpan duration);
    }
}
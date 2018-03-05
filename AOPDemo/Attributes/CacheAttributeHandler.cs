using System;
using Unity;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.PolicyInjection.Policies;

namespace AOPDemo.Attributes
{
    public class CacheAttribute : HandlerAttribute
    {
        private readonly string _cacheKeyPattern;
        private readonly TimeSpan _duration;
        public CacheAttribute(string cacheKeyPattern, int durationMinutes)
        {
            _cacheKeyPattern = cacheKeyPattern;
            _duration = new TimeSpan(0, durationMinutes, 0);
        }
        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return 
                container
                    .Resolve<ICacheAttributeCallHandler>()
                    .SetCacheKeyPattern(_cacheKeyPattern)
                    .SetDuration(_duration);
        }
    }
}
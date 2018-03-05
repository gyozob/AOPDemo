using AOPDemo.Caching;
using System;
using Unity.Interception.PolicyInjection.Pipeline;

namespace AOPDemo.Attributes
{
    public class CacheAttributeCallHandler : ICacheAttributeCallHandler
    {
        private readonly ICacheProvider _cache;
        private string _cacheKeyPattern;
        private TimeSpan _duration;
        public CacheAttributeCallHandler(ICacheProvider cache)
        {
            _cache = cache;
        }
        public int Order
        {
            get
            {
                return 1;
            }

            set
            {
            }
        }
        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            var key = CacheHelper.BuildCacheKey(input, _cacheKeyPattern);
            
            var cached = _cache.Get(key);
            if (cached != null)
            {
                return input.CreateMethodReturn(cached);
            }
            IMethodReturn methodReturn = getNext()(input, getNext);
            _cache.Set(key, methodReturn.ReturnValue, _duration);
            return methodReturn;
        }

        public ICacheAttributeCallHandler SetCacheKeyPattern(string cacheKeyPattern)
        {
            _cacheKeyPattern = cacheKeyPattern;
            return this;
        }
        public ICacheAttributeCallHandler SetDuration(TimeSpan duration)
        {
            _duration = duration;
            return this;
        }
    }
}
using Unity;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.PolicyInjection.Policies;

namespace AOPDemo.Attributes
{
    public class CacheAttribute : HandlerAttribute
    {
        private readonly string _cacheKeyPattern;
        public CacheAttribute(string cacheKeyPatter)
        {
            _cacheKeyPattern = cacheKeyPatter;
        }
        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return container.Resolve<ICacheAttributeCallHandler>().SetCacheKeyPattern(_cacheKeyPattern);
        }
    }
}
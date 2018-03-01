using AOPDemo.Attributes;
using AOPDemo.Caching;
using AOPDemo.Controllers;
using AOPDemo.Repositories;
using Unity;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.Interceptors.InstanceInterceptors.TransparentProxyInterception;

namespace AOPDemo.App_Start
{
    public static class UnityConfig
    {
        public static IUnityContainer Register()
        {
            var container = new UnityContainer();
            container.AddNewExtension<Interception>()
                .Configure<Interception>()
                .SetDefaultInterceptorFor<IValuesRepository>(new TransparentProxyInterceptor());
            container.RegisterType<ICacheAttributeCallHandler, CacheAttributeCallHandler>();
            container.RegisterType<IValuesRepository, ValuesRepository>();
            container.RegisterType<ICacheProvider, MemoryCacheProvider>();            
            container.RegisterType<ValuesController>();
            return container;
        }
    }
}
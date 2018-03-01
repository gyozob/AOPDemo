using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using Unity;
using Unity.Exceptions;

namespace AOPDemo.App_Start
{
    public class UnityResolver : IDependencyResolver
    {
        private IUnityContainer container;
        private IDependencyResolver _fallbackResolver;

        public UnityResolver(IUnityContainer container, IDependencyResolver fallbackResolver)
        {
            _fallbackResolver = fallbackResolver;
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            try
            {
                if (container.IsRegistered(serviceType))
                {
                    return container.Resolve(serviceType);
                }
                else
                {
                    return _fallbackResolver.GetService(serviceType);
                }
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return new List<object>();
            }
        }

        public IDependencyScope BeginScope()
        {
            var child = container.CreateChildContainer();
            return new UnityResolver(child, _fallbackResolver);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            container.Dispose();
        }
    }
}
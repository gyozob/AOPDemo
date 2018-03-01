using AOPDemo.App_Start;
using System.Web.Http;
using Unity;

namespace AOPDemo
{
    public class WebApiConfig : HttpConfiguration
    {
        public WebApiConfig(IUnityContainer container)
        {
            DependencyResolver = new UnityResolver(container, DependencyResolver);

            // Web API routes
            Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            Formatters.Remove(Formatters.XmlFormatter);
            this.MapHttpAttributeRoutes();
        }
    }
}

using AOPDemo.App_Start;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(AOPDemo.Startup))]

namespace AOPDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = UnityConfig.Register();
            app.UseWebApi(new WebApiConfig(container));
        }
    }
}

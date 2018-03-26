using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using boottrader_worker.App_Start;
using Steeltoe.CloudFoundry.ConnectorAutofac;

namespace boottrader_worker
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ServerConfig.RegisterConfig("development");
            var builder = new ContainerBuilder();

            // Register all the controllers with Autofac
            builder.RegisterControllers(typeof(WebApiApplication).Assembly);

            builder.RegisterRabbitMQConnection(App_Start.ServerConfig.Configuration);

            // Create the Autofac container
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}

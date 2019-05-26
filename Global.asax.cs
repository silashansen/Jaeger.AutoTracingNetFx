using Autofac;
using Autofac.Integration.WebApi;
using Jaeger;
using Jaeger.Samplers;
using OpenTracing;
using OpenTracing.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WebApplication1
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

            //DI
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            var tracer = BuildTracer();
            builder.Register(c => tracer).As<ITracer>().SingleInstance();
            GlobalTracer.Register(tracer);


            var container = builder.Build();
            var config = GlobalConfiguration.Configuration;
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

        }

        private ITracer BuildTracer()
        {
            var tracer = new Tracer.Builder("Api")
               .WithSampler(new ConstSampler(true))
               .Build();

            return tracer;
        }

    }
}

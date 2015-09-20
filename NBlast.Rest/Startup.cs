﻿using System.Linq;
using System.Web.Http;
using NBlast.Rest.Configuration;
using Ninject.WebApi.DependencyResolver;
using Owin;
using WebApiContrib.Formatting.Jsonp;

namespace NBlast.Rest
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            config.AddJsonpFormatter();
            config.EnableCors();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            config.DependencyResolver = new NinjectDependencyResolver(NinjectKernelSupplier.Supply());
            config.EnsureInitialized();

            appBuilder.UseWebApi(config);
        }
    }
}
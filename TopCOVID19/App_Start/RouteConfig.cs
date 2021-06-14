using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TopCOVID19
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

          

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
              name: "Provincia",
              url: "{controller}/{action}/{id}",
              new { controller = "Home", action = "Provincia", id = UrlParameter.Optional }
          );

            routes.MapRoute(
              name: "xml",
              url: "{controller}/{action}/{id}",
              new { controller = "Home", action = "getXML", id = UrlParameter.Optional }
          );

            routes.MapRoute(
              name: "json",
              url: "{controller}/{action}/{id}",
              new { controller = "Home", action = "getJSON", id = UrlParameter.Optional }
          );

            routes.MapRoute(
              name: "csv",
              url: "{controller}/{action}/{id}",
              new { controller = "Home", action = "getCSV", id = UrlParameter.Optional }
          );
        }
    }
}

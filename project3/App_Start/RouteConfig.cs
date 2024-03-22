using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace project3
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "",
                defaults: new { section = "User", controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "project3.User.Controllers" }
            );
            routes.MapRoute(
                name: "User",
                url: "{section}/{controller}/{action}/{id}",
                defaults: new { section = "User", controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "project3.User.Controllers" }
            );
            routes.MapRoute(
                name: "Admin",
                url: "{section}/{controller}/{action}/{id}",
                defaults: new { section = "Admin", controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "project3.Admin.Controllers" }
            );
            routes.MapRoute(
                name: "LoginUser",
                url: "{section}/{controller}/{action}/{id}",
                defaults: new { section = "User", controller = "Home", action = "Login", id = UrlParameter.Optional },
                namespaces: new string[] { "project3.User.Controllers" }
            );
            routes.MapRoute(
                name: "RegisterUser",
                url: "{section}/{controller}/{action}/{id}",
                defaults: new { section = "User", controller = "Home", action = "Register", id = UrlParameter.Optional },
                namespaces: new string[] { "project3.User.Controllers" }
            );
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace project3
{
    public class CustomViewEngine : RazorViewEngine
    {
        public CustomViewEngine() : base()
        {
            ViewLocationFormats = new string[] {
                "~/Views/%1/{1}/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml",
            };

            MasterLocationFormats = new string[] {
                "~/Views/%1/{1}/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml",
                "~/Views/%1",
                "~/Views",
            };

            PartialViewLocationFormats = new string[] {
                "~/Views/%1/{1}/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml",
                "~/Views/%1",
                "~/Views",
            };
        }
        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            var section = controllerContext.RouteData.Values["section"]?.ToString() ?? string.Empty;
            return base.CreatePartialView(controllerContext, partialPath.Replace("%1", section));
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            var section = controllerContext.RouteData.Values["section"]?.ToString() ?? string.Empty;
            return base.CreateView(controllerContext, viewPath.Replace("%1", section), masterPath.Replace("%1", section));
        }

        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            var section = controllerContext.RouteData.Values["section"]?.ToString() ?? string.Empty;
            return base.FileExists(controllerContext, virtualPath.Replace("%1", section));
        }
    }
}
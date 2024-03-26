﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace project3.User.Controllers
{
    public class BaseController : Controller
    {
        // GET: CheckLoginSessionAttribute

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["cus"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(new { Controller = "Home", Action = "Index" })
                );
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
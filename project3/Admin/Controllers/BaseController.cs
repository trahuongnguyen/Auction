using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace project3.Admin.Controllers
{
    public class BaseController : Controller
    {
        // GET: CheckLoginSessionAttribute

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["uName"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(new { Controller = "Account", Action="Login" })
                );
            }
            base.OnActionExecuting(filterContext);
        }
    }

}
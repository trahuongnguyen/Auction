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
            if (HttpContext.Request.Params["section"].ToString().Equals("Admin"))
            {
                if (Session["uName"] == null)
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new System.Web.Routing.RouteValueDictionary(new { section = "Admin",  Controller = "Account", Action = "Login" })
                    );
                }
            }

            if (HttpContext.Request.Params["section"].ToString().Equals("User"))
            {
                if (Session["cus"] == null)
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new System.Web.Routing.RouteValueDictionary(new { section = "User", Controller = "Home", Action = "Index" })
                    );
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }

}
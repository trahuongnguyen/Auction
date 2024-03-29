using Newtonsoft.Json.Linq;
using project3.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static System.Collections.Specialized.BitVector32;

namespace project3.Admin.Controllers
{
    public class BaseController : Controller
    {
        // GET: CheckLoginSessionAttribute
        private DAOController dataAccess = new DAOController();
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["uName"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(new { Controller = "Account", Action = "Login" })
                );
            }
            else
            {
                int id = Int32.Parse(Session["id"].ToString());
                Console.WriteLine(id);
                List<relGr> rel = new List<relGr>();
                string selectQuery = "select * from REL_Em_Gru where REL_Em_Gru.em_ID = " + id + "";
                DataTable result = dataAccess.ExecuteQuery(selectQuery);
                foreach (DataRow row in result.Rows)
                {
                    relGr rel1 = new relGr();
                    rel1.em_id = Convert.ToInt32(row["em_ID"]);
                    rel1.gr_id = Convert.ToInt32(row["gru_ID"]);
                    rel.Add(rel1);
                }
                bool check1 = rel.Any(g => g.gr_id == 1 || g.gr_id == 2);
                string action = RouteData.Values["action"].ToString();
                string controller = RouteData.Values["controller"].ToString();
                if (controller.Equals("Employees") || controller.Equals("CustomersStar"))
                {
                    if (check1)
                    {
                        base.OnActionExecuting(filterContext);
                    }
                    else
                    {
                        filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(new { Section = "Admin", Controller = "Home", Action = "Index" })
                );
                    }
                }
                bool check2 = rel.Any(g => g.gr_id == 1 || g.gr_id == 3);
                if (controller.Equals("Products") || controller.Equals("Category") || controller.Equals("Auctions"))
                {
                    if (check2)
                    {
                        base.OnActionExecuting(filterContext);
                    }
                    else
                    {
                        filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(new { Section = "Admin", Controller = "Home", Action = "Index" })
                );
                    }
                }
                bool check3 = rel.Any(g => g.gr_id == 1);
                if (controller.Equals("Employees") && action.Equals("Decentralization"))
                {
                    if (check3)
                    {
                        base.OnActionExecuting(filterContext);
                    }
                    else
                    {
                        filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(new {Section = "Admin", Controller = "Home", Action = "Index" })
                );
                    }
                }

            }
            base.OnActionExecuting(filterContext);
        }
    }

}



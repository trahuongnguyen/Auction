using project3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace project3.User.Controllers
{
    public class HomeController : Controller
    {
        private dbauctionsystemEntities db = new dbauctionsystemEntities();
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.Categories = db.Categories.ToList();
            ViewBag.Products = db.Products.ToList();
            ViewBag.Autions = db.Auctions.ToList();

            /*Product product = new Product();
            product.Images.ElementAt(0);*/
            //ViewData["Categories"]
           /* Auction auction = new Auction();
            foreach (var item in auction.REL_Pro_Au) 
            { 
                var img = item.Product.Images.ElementAt(0).Img;
            }*/
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection form)
        {
            string name = form["name"];
            if (!string.IsNullOrEmpty(name))
            {
                Customer customer = db.Customers.FirstOrDefault(c=>c.UserName.Equals(name));
                if (customer != null)
                {
                    string password = form["password"];
                    if(!string.IsNullOrEmpty(password) && customer.Password.Equals(password))
                    {
                        HttpContext.Session.Add("user", customer);
                        return View();
                    }
                }
            }
            return RedirectToAction("~/");
        }
    }
}
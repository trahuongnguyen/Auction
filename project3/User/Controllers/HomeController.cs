using project3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
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
        public ActionResult Login(Customer customer)
        {
            if (ModelState.IsValid)
            {
                var cus = db.Customers.FirstOrDefault(c => c.UserName.Equals(customer.UserName));
                if (cus == null)
                {
                    ModelState.AddModelError("UserName", "UserName is not exist");
                    return View(customer);
                } 
                if(customer.Password.Length > 0)
                {
                    if(!Crypto.VerifyHashedPassword(cus.Password, customer.Password))
                    {
                        ModelState.AddModelError("Password", "Password is incorrect");
                        return View(customer);
                    }
                }
                HttpContext.Session.Add("cus", customer);
            }
            return RedirectToAction("~/");
        } 

        [HttpPost]
        public ActionResult Register(Customer customer)
        {
            if (customer == null)
            {
                return View(customer);
            }
            if (!ModelState.IsValid)
            {
                return View(customer);
            }
            if (DupplicatedEmail(customer))
            {
                ModelState.AddModelError("Duplicated", "Duplicated email");
            }
            if (DupplicatedUsername(customer))
            {
                ModelState.AddModelError("Duplicated", "Duplicated username");
            }
            customer.Status = 1;
            customer.Password = PassHash(customer.Password);
            db.Customers.Add(customer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        private bool DupplicatedEmail(Customer customer)
        {
            bool Flag = false;
            Customer customer1 = db.Customers.Where(c => c.Email.Equals(customer.Email)).FirstOrDefault();
            if (customer1 != null)
            {
                Flag = true;
            }
            return Flag;
        } 

        private bool DupplicatedUsername(Customer customer)
        {
            bool Flag = false;
            Customer customer1 = db.Customers.Where(c => c.UserName.Equals(customer.UserName)).FirstOrDefault();
            if (customer1 != null)
            {
                Flag = true;
            }
            return Flag;
        }
        static string PassHash(string password)
        {
            return Crypto.HashPassword(password);
        }

    }
}
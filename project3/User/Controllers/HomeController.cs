using project3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Script.Services;
using System.Web.UI.HtmlControls;

namespace project3.User.Controllers
{
    public class HomeController : Controller
    {
        private dbauctionsystemEntities db = new dbauctionsystemEntities();
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.Categories = db.Categories.Where(c=>c.Status==1).ToList();
            ViewBag.Products = db.Products.Where(p => p.sta_ID == 4).ToList();
            ViewBag.Autions = db.Auctions.Where(a=>a.EndTime > DateTime.Now).ToList();

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
        [ValidateAntiForgeryToken]
        public ActionResult Login(FormCollection form)
        {
            ViewBag.Categories = db.Categories.ToList();
            ViewBag.Products = db.Products.Where(p => p.sta_ID == 4).ToList();
            ViewBag.Autions = db.Auctions.ToList();
            if (!string.IsNullOrEmpty(form["UserName"].ToString()) && !string.IsNullOrEmpty(form["Password"].ToString()))
            {
                string username = form["UserName"].ToString();
                string password = form["Password"].ToString();
                var cus = db.Customers.FirstOrDefault(c => c.UserName.Equals(username));
                if (cus == null)
                {
                    ModelState.AddModelError("UserName", "UserName is not exist");
                    return View("Index", form);
                } 
                if(password.Length > 0)
                {
                    if(!Crypto.VerifyHashedPassword(cus.Password, password))
                    {
                        ModelState.AddModelError("password", "Password is incorrect");

                        return View("Index", form);
                    }
                }
                Session["cus"] = cus.cus_ID;
            }
            return RedirectToAction("Index", "Shop");
        }
        /*[HttpGet]
        public ActionResult Register()
        {
            return View();
        }*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(FormCollection form)
        {
            ViewBag.Categories = db.Categories.ToList();
            ViewBag.Products = db.Products.Where(p => p.sta_ID == 4).ToList();
            ViewBag.Autions = db.Auctions.ToList();
            if (string.IsNullOrEmpty(form["FirstName"].ToString()) || string.IsNullOrEmpty(form["LastName"].ToString()) || string.IsNullOrEmpty(form["UserName"].ToString()) || string.IsNullOrEmpty(form["Password"].ToString()) || string.IsNullOrEmpty(form["Email"].ToString()) || string.IsNullOrEmpty(form["PhoneNumber"].ToString()) || string.IsNullOrEmpty(form["Address"].ToString()) || string.IsNullOrEmpty(form["Sex"].ToString()))
            {
                ModelState.AddModelError("", "Please enter full of information");
                return View("Index", form);
            }
            if (!ModelState.IsValid)
            {
                return View("Index", form);
            }
            if (DupplicatedEmail(form["Email"].ToString()))
            {
                ModelState.AddModelError("Duplicated", "Duplicated email");
                return View("Index", form);
            }
            if (DupplicatedUsername(form["UserName"].ToString()))
            {
                ModelState.AddModelError("Duplicated", "Duplicated username");
                return View("Index", form);
            }
            Customer customer = new Customer();
            customer.UserName = form["UserName"].ToString();
            customer.Email = form["Email"].ToString();
            customer.Address = form["Address"].ToString();
            customer.PhoneNumber = form["PhoneNumber"].ToString();
            customer.Sex = form["Sex"].ToString();
            customer.Status = 1;
            customer.Surplus = 0;
            customer.Password = PassHash(form["Password"].ToString());
            db.Customers.Add(customer);
            db.SaveChanges();
            return RedirectToAction("Index", "Shop");
        }
        private bool DupplicatedEmail(string str)
        {
            bool Flag = false;
            Customer customer1 = db.Customers.Where(c => c.Email.Equals(str)).FirstOrDefault();
            if (customer1 != null)
            {
                Flag = true;
            }
            return Flag;
        } 

        private bool DupplicatedUsername(string str)
        {
            bool Flag = false;
            Customer customer1 = db.Customers.Where(c => c.UserName.Equals(str)).FirstOrDefault();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(FormCollection form)
        {
            if (string.IsNullOrEmpty(form["name"].ToString()) || string.IsNullOrEmpty(form["email"].ToString()) || string.IsNullOrEmpty(form["subject"].ToString()) || string.IsNullOrEmpty(form["Message1"].ToString()))
            {
                ModelState.AddModelError("", "Please enter full of information");
                return View("Contact", form);
            }
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Please enter full of information");
                return View("Contact", form);
            }
            Message mess = db.Messages.FirstOrDefault(m => m.Message1.Equals(form["Message1"], StringComparison.OrdinalIgnoreCase));
            if (mess != null)
            {
                ModelState.AddModelError("Message1", "Do not spam content message");
                return View("Contact", form);
            }
            Message message = new Message();
            message.Message1 = form["Message1"].ToString();
            mess.Time = DateTime.Now;
            db.Messages.Add(message);
            db.SaveChanges();
            return RedirectToAction("Index", "Shop");
        }

    }
}
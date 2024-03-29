using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using PagedList;
using project3.Models;

namespace project3.User.Controllers
{
    public class CustomersController : BaseController
    {
        private dbauctionsystemEntities db = new dbauctionsystemEntities();
        static Customer cus;
        // GET: Customers
        public ActionResult Index()
        {
            cus = db.Customers.Find(int.Parse(Session["cus"].ToString()));
            ViewBag.Products_Buying = db.Products.Where(p=>p.REL_Or_Pro.ElementAt(0).Order.cus_ID == cus.cus_ID).ToList();
            ViewBag.Products_No = db.Notifications.Where(n=>n.BuyCus == cus.cus_ID || n.SellCus == cus.cus_ID).ToList();
            return View(cus);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePass(FormCollection form)
        {
            if (string.IsNullOrEmpty(form["old_pass"].ToString()) || string.IsNullOrEmpty(form["new_pass"].ToString()) || string.IsNullOrEmpty(form["confirm_pass"].ToString()))
            {
                ModelState.AddModelError("summaryChange", "Please enter full of information");
                return View("Index", form);
            }
            if (!ModelState.IsValid)
            {
                return View("Index", form);
            }
            if (!Crypto.VerifyHashedPassword(cus.Password, form["old_pass"].ToString()))
            {
                ModelState.AddModelError("old_pass", "Old password is incorrect");
                return View("Index", form);
            }

            if (!form["new_pass"].ToString().Equals(form["confirm_pass"].ToString()))
            {
                ModelState.AddModelError("confirm_pass", "Password is incorrect");
                return View("Index", form);
            }
            cus.Password = Crypto.HashPassword(form["new_pass"].ToString());
            db.Entry(cus).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult History()
        {
            cus = db.Customers.Find(int.Parse(Session["cus"].ToString()));
            return View(db.Bids.Where(b=>b.cus_ID == cus.cus_ID));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUser(FormCollection form)
        {
            if (string.IsNullOrEmpty(form["usernameUpdate"].ToString()) || string.IsNullOrEmpty(form["password"].ToString())  || string.IsNullOrEmpty(form["phoneUpdate"].ToString()) || string.IsNullOrEmpty(form["addressUpdate"].ToString()) || string.IsNullOrEmpty(form["sexUpdate"].ToString()))
            {
                ModelState.AddModelError("summaryUpdate", "Please enter full of information");
                return View("Index", form);
            }
            if (!ModelState.IsValid)
            {
                return View("Index", form);
            }
            if (DupplicatedUsername(form["usernameUpdate"].ToString()))
            {
                ModelState.AddModelError("Duplicated", "Duplicated username");
                return View("Index", form);
            }
            if (!Crypto.VerifyHashedPassword(cus.Password, form["password"].ToString()))
            {
                ModelState.AddModelError("password", "Password is incorrect");
                return View("Index", form);
            }
            cus.UserName = form["usernameUpdate"].ToString();
            cus.Address = form["addressUpdate"].ToString();
            cus.PhoneNumber = form["phoneUpdate"].ToString();
            cus.Sex = form["sexUpdate"].ToString();
            db.Entry(cus).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Your_buying(int?pi)
        {
            cus = db.Customers.Find(int.Parse(Session["cus"].ToString()));
            int pageNumber = pi ?? 1;
            int pageSize = 12;
            List<Product> products = db.Products.Where(p => p.cus_ID == cus.cus_ID).ToList();
            ViewBag.Shop = products;
            return View(products.ToPagedList(pageNumber,pageSize));
        }

        // GET: Customers/Create
        public ActionResult Selling()
        {
            cus = db.Customers.Find(int.Parse(Session["cus"].ToString()));
            ViewBag.cat_ID = new SelectList(db.Categories, "cat_ID", "NameCat");
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Selling([Bind(Include = "pro_ID,NamePro,StartingPrice,StepPrice,StartTime,EndTime,ReceivedDate,Description,cus_ID,MoreInformation,sta_ID")] Product product, IEnumerable<HttpPostedFileBase> Images, List<int> cat_IDs)
        {
            ViewBag.cat_ID = new SelectList(db.Categories, "cat_ID", "NameCat", product.Categories);
            if (ModelState.IsValid)
            {
                if (Images != null)
                {
                    foreach (var item in Images)
                    {
                        if (item != null && item.ContentLength > 0)
                        {
                            if (!Path.GetExtension(item.FileName).ToLower().Equals(".png") && !Path.GetExtension(item.FileName).ToLower().Equals(".jpg"))
                            {
                                ModelState.AddModelError("Images", "Please choose file type .png or .jpg");
                                return View(product);
                            }
                        }
                    }
                    if(product.StartTime>=product.EndTime)
                    {
                        ModelState.AddModelError("", "End time must be greater than Start time");
                        return View(product);
                    }
                    foreach (var item in cat_IDs)
                    {
                        Category category = db.Categories.FirstOrDefault(c => c.cat_ID == item);
                        product.Categories.Add(category);
                    }
                    product.ReceivedDate = DateTime.Now;
                    product.cus_ID = cus.cus_ID;
                    product.sta_ID = 2;
                    db.Products.Add(product);
                    db.SaveChanges();
                    Product product1 = db.Products.FirstOrDefault(p => p.pro_ID == db.Products.Max(pro => pro.pro_ID));
                    foreach (var item in Images)
                    {
                        if(item != null && item.ContentLength > 0)
                        {
                            try
                            {
                                using (var binaryReader = new BinaryReader(item.InputStream))
                                {
                                    Image imgs = new Image();
                                    imgs.Img = binaryReader.ReadBytes(item.ContentLength);
                                    imgs.pro_ID = product1.pro_ID;
                                    db.Images.Add(imgs);
                                    db.SaveChanges();
                                }
                            }
                            catch (Exception)
                            {

                                throw;
                            }
                        }
                    }
                }

                return RedirectToAction("Sell_products", "Shop");
            }

            return View(product);
        }

        private bool DupplicatedUsername(string str)
        {
            bool Flag = false;
            Customer customer1 = db.Customers.Where(c => c.UserName.Equals(str) && c.cus_ID != cus.cus_ID).FirstOrDefault();
            if (customer1 != null)
            {
                Flag = true;
            }
            return Flag;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

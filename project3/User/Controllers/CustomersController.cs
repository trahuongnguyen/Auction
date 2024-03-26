using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using project3.Models;

namespace project3.User.Controllers
{
    public class CustomersController : Controller
    {
        private dbauctionsystemEntities db = new dbauctionsystemEntities();

        // GET: Customers
        public ActionResult Index()
        {
            return View(db.Customers.FirstOrDefault(c=>c.cus_ID == Int32.Parse(Session["cus"].ToString())));
        }

        public ActionResult History()
        {
            return View(db.Bids.Where(b=>b.cus_ID == Int32.Parse(Session["cus"].ToString())));
        }

        public ActionResult Your_buying()
        {
            return View(db.Products.Where(p=>p.cus_ID == Int32.Parse(Session["cus"].ToString())));
        }

        public ActionResult Reviews()
        {
            return View();
        }

        // GET: Customers/Create
        public ActionResult Selling()
        {
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
                                ModelState.AddModelError("Image", "Please choose file type .png or .jpg");
                                return View(product);
                            }
                        }
                    }
                    foreach (var item in cat_IDs)
                    {
                        Category category = db.Categories.FirstOrDefault(c => c.cat_ID == item);
                        product.Categories.Add(category);
                    }
                    product.ReceivedDate = DateTime.Now;
                    product.sta_ID = 1;
                    db.Products.Add(product);
                    db.SaveChanges();
                    Product product1 = db.Products.FirstOrDefault(p => p.pro_ID == db.Products.Max(pro => pro.pro_ID));
                    foreach (var item in Images)
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

                return RedirectToAction("Index");
            }

            ViewBag.cat_ID = new SelectList(db.Categories, "cat_ID", "NameCat", product.Categories);
            return View(product);
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

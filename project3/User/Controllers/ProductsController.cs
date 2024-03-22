using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using project3.Models;

namespace project3.User.Controllers.User
{
    public class ProductsController : Controller
    {
        private dbauctionsystemEntities db = new dbauctionsystemEntities();

        // GET: Products
        public ActionResult Index(int? pi)
        {
            int PageNumber = pi ?? 1;
            int PageSize = 12;
            var products = db.Products.Include(p => p.Customer).Include(p => p.Status);
            return View(products.ToPagedList(PageNumber, PageSize));
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.cat_ID = new SelectList(db.Categories, "cat_ID", "NameCat");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "pro_ID,NamePro,StartingPrice,StepPrice,StartTime,EndTime,ReceivedDate,Description,cus_ID,MoreInformation,sta_ID")] Product product, IEnumerable<HttpPostedFileBase> Images, List<int> cat_IDs)
        {
            if (ModelState.IsValid)
            {
                if(Images != null)
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
                        Category category = db.Categories.FirstOrDefault(c=> c.cat_ID == item);
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

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.cus_ID = new SelectList(db.Customers, "cus_ID", "FirstName", product.cus_ID);
            ViewBag.sta_ID = new SelectList(db.Status, "sta_ID", "StatusDetails", product.sta_ID);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "pro_ID,NamePro,StartingPrice,StepPrice,StartTime,EndTime,ReceivedDate,Description,cus_ID,MoreInformation,sta_ID")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.cus_ID = new SelectList(db.Customers, "cus_ID", "FirstName", product.cus_ID);
            ViewBag.sta_ID = new SelectList(db.Status, "sta_ID", "StatusDetails", product.sta_ID);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using project3.Models;

namespace project3.User.Controllers
{
    public class ShopController : Controller
    {
        private dbauctionsystemEntities db = new dbauctionsystemEntities();

        // GET: Shop
        [Route("[section]/[contronller]/[action]/{cat_ID}")]
        public ActionResult Index()
        {
            if (HttpContext.Request.QueryString["cat_ID"].ToString() != null)
            {
                int catID = Int32.Parse(RouteData.Values["cat_ID"].ToString());
                Category ca = db.Categories.Where(c => c.cat_ID == catID).FirstOrDefault();
                var products = ca.Products.ToList();
                return View(products);
            }
            
            //  var products = db.Products.Include(p => p.Customer).Include(p => p.Status);
            return View();
        }

        // GET: Shop/Details/5
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

        // GET: Shop/Create
        public ActionResult Create()
        {
            ViewBag.cus_ID = new SelectList(db.Customers, "cus_ID", "FirstName");
            ViewBag.sta_ID = new SelectList(db.Status, "sta_ID", "StatusDetails");
            return View();
        }

        // POST: Shop/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "pro_ID,NamePro,StartingPrice,StepPrice,StartTime,EndTime,ReceivedDate,Description,cus_ID,MoreInformation,sta_ID")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.cus_ID = new SelectList(db.Customers, "cus_ID", "FirstName", product.cus_ID);
            ViewBag.sta_ID = new SelectList(db.Status, "sta_ID", "StatusDetails", product.sta_ID);
            return View(product);
        }

        // GET: Shop/Edit/5
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

        // POST: Shop/Edit/5
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

        // GET: Shop/Delete/5
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

        // POST: Shop/Delete/5
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

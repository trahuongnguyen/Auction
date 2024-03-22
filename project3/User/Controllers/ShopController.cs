﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using project3.Models;

namespace project3.User.Controllers
{
    public class ShopController : Controller
    {
        private dbauctionsystemEntities db = new dbauctionsystemEntities();

        // GET: Shop
        //[Route("[section]/[contronller]/[action]/{cat_ID}")]
        public ActionResult Index(int? pi)
        {
            int PageNumber = pi ?? 1;
            int PageSize = 12;
            List<Product> products = db.Products.Include(p => p.Customer).Include(p => p.Status).ToList();
            if (HttpContext.Request.Params !=null)
            {
                if (Request.Params["cat_ID"] != null)
                {
                    int cat_ID = Request.Params["cat_ID"].ToString() != null ? Int32.Parse(Request.Params["cat_ID"].ToString()) : 0;
                    if (cat_ID > 0)
                    {
                        Category ca = db.Categories.Where(c => c.cat_ID == cat_ID).FirstOrDefault();
                        if (ca != null)
                        {
                            products = ca.Products.ToList();
                        }
                    }
                }

                if (Request.Params["au_ID"] != null)
                {
                    int au_ID = Request.Params["au_ID"].ToString() != null ? Int32.Parse(Request.Params["au_ID"].ToString()) : 0;
                    if (au_ID > 0)
                    {
                        Auction auction = db.Auctions.Where(a => a.au_ID == au_ID).FirstOrDefault();
                        if (auction != null)
                        {
                            products = new List<Product>();
                            foreach (var item in auction.REL_Pro_Au)
                            {
                                products.Add(item.Product);
                            }
                        }
                    }
                }
            }
            return View(products.ToPagedList(PageNumber, PageSize));
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
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using project3.Admin.Controllers;
using project3.Models;

namespace project3.User.Controllers
{
    public class BidsController : BaseController
    {
        private dbauctionsystemEntities db = new dbauctionsystemEntities();

        // GET: Bids
        public ActionResult Index()
        {
            var bids = db.Bids.Include(b => b.REL_Pro_Au).Include(b => b.Customer);
            return View(bids.ToList());
        }

        // GET: Bids/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bid bid = db.Bids.Find(id);
            if (bid == null)
            {
                return HttpNotFound();
            }
            return View(bid);
        }

        // GET: Bids/Create
        public ActionResult Create()
        {
            ViewBag.pro_ID = new SelectList(db.REL_Pro_Au, "pro_ID", "pro_ID");
            ViewBag.cus_ID = new SelectList(db.Customers, "cus_ID", "FirstName");
            return View();
        }

        // POST: Bids/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "bid_ID,Time,Price,cus_ID,pro_ID,au_ID")] Bid bid)
        {
            if (ModelState.IsValid)
            {
                db.Bids.Add(bid);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.pro_ID = new SelectList(db.REL_Pro_Au, "pro_ID", "pro_ID", bid.pro_ID);
            ViewBag.cus_ID = new SelectList(db.Customers, "cus_ID", "FirstName", bid.cus_ID);
            return View(bid);
        }

        // GET: Bids/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bid bid = db.Bids.Find(id);
            if (bid == null)
            {
                return HttpNotFound();
            }
            ViewBag.pro_ID = new SelectList(db.REL_Pro_Au, "pro_ID", "pro_ID", bid.pro_ID);
            ViewBag.cus_ID = new SelectList(db.Customers, "cus_ID", "FirstName", bid.cus_ID);
            return View(bid);
        }

        // POST: Bids/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "bid_ID,Time,Price,cus_ID,pro_ID,au_ID")] Bid bid)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bid).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.pro_ID = new SelectList(db.REL_Pro_Au, "pro_ID", "pro_ID", bid.pro_ID);
            ViewBag.cus_ID = new SelectList(db.Customers, "cus_ID", "FirstName", bid.cus_ID);
            return View(bid);
        }

        // GET: Bids/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bid bid = db.Bids.Find(id);
            if (bid == null)
            {
                return HttpNotFound();
            }
            return View(bid);
        }

        // POST: Bids/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Bid bid = db.Bids.Find(id);
            db.Bids.Remove(bid);
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

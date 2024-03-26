using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json.Linq;
using project3.Models;
using WebGrease.Css.Extensions;

namespace project3.Admin.Controllers
{
    public class AuctionsController : BaseController
    {
        private dbauctionsystemEntities db = new dbauctionsystemEntities();

        // GET: Auctions
        public ActionResult Index()
        {
            var auction = db.Auctions.Include(a => a.Employee);
            return View(auction.ToList());
        }

        // GET: Auctions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auction auction = db.Auctions.Find(id);
            ViewBag.rel = db.REL_Pro_Au.Where(r => r.au_ID == auction.au_ID).Include(r => r.Product).ToList();
            if (auction == null)
            {
                return HttpNotFound();
            }
            return View(auction);
        }

        // GET: Auctions/Create
        public ActionResult Create()
        {

            ViewBag.ListPro = db.Products.Include(p => p.Customer).Include(p => p.Status).Where(p => p.sta_ID == 2).ToList();
            return View();
        }

        // POST: Auctions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "au_ID,NameAu,StartTime,EndTime")] Auction auction, List<int> proID)
        {
            if (ModelState.IsValid)
            {
                REL_Pro_Au rEL_Pro_Au = new REL_Pro_Au();
                if (proID != null)
                {
                    foreach (var value in proID.ToList())
                    {
                        rEL_Pro_Au.pro_ID = value;
                        rEL_Pro_Au.au_ID = auction.au_ID;
                        proID.Remove(value);
                        db.REL_Pro_Au.Add(rEL_Pro_Au);
                        db.SaveChanges();
                    }
                }
                db.Auctions.Add(auction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.em_ID = new SelectList(db.Employees, "em_ID", "FirstName", auction.em_ID);
            return View(auction);
        }

        // GET: Auctions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auction auction = db.Auctions.Find(id);
            List<REL_Pro_Au> rEL_Pro_Au = db.REL_Pro_Au.Where(r => r.au_ID == auction.au_ID).ToList();
            foreach (REL_Pro_Au rEL_Pro_ in rEL_Pro_Au)
            {
                ViewBag.ListProAu.Add(db.Products.Include(P => P.Categories).Include(p => p.Status).Where(p => p.pro_ID == rEL_Pro_.pro_ID).ToList());
            }
            ViewBag.ListPro = db.Products.Include(p => p.Customer).Include(p => p.Status).Where(p => p.sta_ID == 2).ToList();

            if (auction == null)
            {
                return HttpNotFound();
            }
            return View(auction);
        }

        // POST: Auctions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "au_ID,NameAu,StartTime,EndTime")] Auction auction, List<int> proID)
        {
            if (ModelState.IsValid)
            {
                REL_Pro_Au rEL_Pro_ = new REL_Pro_Au();
                List<REL_Pro_Au> rEL_Pro_Au = db.REL_Pro_Au.Where(r => r.au_ID == auction.au_ID).ToList();
                if (proID != null)
                {
                    foreach(var r in rEL_Pro_Au)
                    {
                        db.REL_Pro_Au.Remove(r);
                    }
                    foreach(int p in proID)
                    {
                        rEL_Pro_.pro_ID = p;
                        rEL_Pro_.au_ID = auction.au_ID;
                        db.REL_Pro_Au.Add(rEL_Pro_);
                    }
                }
                db.Entry(auction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.em_ID = new SelectList(db.Employees, "em_ID", "FirstName", auction.em_ID);
            return View(auction);
        }

        // GET: Auctions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auction auction = db.Auctions.Find(id);
            if (auction == null)
            {
                return HttpNotFound();
            }
            return View(auction);
        }

        // POST: Auctions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Auction auction = db.Auctions.Find(id);
            db.Auctions.Remove(auction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}

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
using PagedList;

namespace project3.Admin.Controllers
{
    public class AuctionsController : BaseController
    {
        private dbauctionsystemEntities db = new dbauctionsystemEntities();

        // GET: Auctions
        public ActionResult Index(int? pi)
        {
            var auction = db.Auctions.Include(a => a.Employee).ToList();
            
            int PageNumber = pi ?? 1;
            int PageSize = 5;
            return View(auction.ToPagedList(PageNumber, PageSize));
        }

        // GET: Auctions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auction auction = db.Auctions.Find(id);
            ViewBag.rel = db.REL_Pro_Au.Where(r => r.au_ID == auction.au_ID).ToList();
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
                db.Auctions.Add(auction);
                db.SaveChanges();     
                if (proID != null)
                {
                    foreach (var value in proID.ToList())
                    {
                        REL_Pro_Au rEL_Pro_Au = new REL_Pro_Au();
                        rEL_Pro_Au.pro_ID = value;
                        rEL_Pro_Au.au_ID = auction.au_ID;   
                        db.REL_Pro_Au.Add(rEL_Pro_Au);
                        var product = db.Products.Where(p => p.pro_ID == value).FirstOrDefault();
                        product.sta_ID = 4;
                        db.Entry(product).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                
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
            /*List<Product> products = new List<Product>();
            foreach (REL_Pro_Au rEL_Pro_ in rEL_Pro_Au)
            {
                products.Add(db.Products.Where(p => p.pro_ID == rEL_Pro_.pro_ID).FirstOrDefault());
            }*/
            ViewBag.ListPro = db.Products.Include(p => p.Customer).Include(p => p.Status).Where(p => p.sta_ID != 1 && p.sta_ID != 5).ToList();
            ViewBag.ListProAu = rEL_Pro_Au;
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
                List<REL_Pro_Au> rEL_Pro_Au = db.REL_Pro_Au.Where(r => r.au_ID == auction.au_ID).ToList();
                if (proID != null)
                {
                    foreach(var r in rEL_Pro_Au)
                    {
                        db.REL_Pro_Au.Remove(r);
                        Product pro = db.Products.FirstOrDefault(p=>p.pro_ID==r.pro_ID);
                        pro.sta_ID = 2;
                        db.Entry(pro).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    foreach (var value in proID.ToList())
                    {
                        REL_Pro_Au rel = new REL_Pro_Au();
                        rel.pro_ID = value;
                        rel.au_ID = auction.au_ID;
                        db.REL_Pro_Au.Add(rel);
                        var product = db.Products.Where(p => p.pro_ID == value).FirstOrDefault();
                        product.sta_ID = 4;
                        db.Entry(product).State = EntityState.Modified;
                        db.SaveChanges();
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
            auction.Stastus = 0;
            db.Entry(auction).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}

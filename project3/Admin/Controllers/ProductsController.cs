﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using project3.Models;
using PagedList;

namespace project3.Admin.Controllers
{
    public class ProductsController : BaseController
    {
        private dbauctionsystemEntities db = new dbauctionsystemEntities();


        public ActionResult Index(int? pi)
        {
            int PageNumber = pi ?? 1;
            int PageSize = 5;
            ViewBag.ListPro = db.Products.Include(p => p.Customer).Include(p => p.Status).Include(p=>p.REL_Pro_Au).Where(p=>p.Status.sta_ID == 1);
            var products = db.Products.Include(p => p.Customer).Include(p => p.Status).Where(p => p.Status.sta_ID != 1);
            return View(products.ToPagedList(PageNumber, PageSize));
        }
        public ActionResult Inspect(List<int> proID)
        {
            if (proID != null)
            {
                foreach (var value in proID.ToList())
                {
                    Product product = db.Products.Where(p=>p.pro_ID == value).FirstOrDefault();
                    if(product != null)
                    {
                        product.sta_ID = 2;
                        proID.Remove(value);
                        db.Entry(product).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
            }
            return View();
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Include(p=>p.Status).Include(p=>p.Customer).Include(p=>p.Categories).Where(p=>p.pro_ID==id).FirstOrDefault();
            if (product == null)
            {
                return HttpNotFound();
            }
            
            return View(product);
        }
    }
}

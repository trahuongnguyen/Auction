using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using project3.Admin.Controllers;
using project3.Models;

namespace project3.User.Controllers
{
    public class ShopController : BaseController
    {
        private dbauctionsystemEntities db = new dbauctionsystemEntities();
        
        // GET: Shop
        //[Route("[section]/[contronller]/[action]/{cat_ID}")]
        public ActionResult Index(int? pi)
        {
            int PageNumber = pi ?? 1;
            int PageSize = 12;
            List<Product> products = db.Products.Include(p => p.Customer).Include(p => p.Status).Where(p => p.sta_ID == 4).ToList();
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
                            products = ca.Products.Where(p => p.sta_ID == 4).ToList();
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

        public ActionResult Sell_products(int? pi)
        {
            int PageNumber = pi ?? 1;
            int PageSize = 12;
            var products = db.Products.Include(p => p.Customer).Include(p => p.Status);
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

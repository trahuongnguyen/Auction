using System;
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
    public class ShopController : BaseController
    {
        private dbauctionsystemEntities db = new dbauctionsystemEntities();


        // GET: Shop
        //[Route("[section]/[contronller]/[action]/{cat_ID}")]
        public ActionResult Index(int? pi, int? pz, int? price, int? evented)
        {

            int PageNumber = pi ?? 1;
            int PageSize = pz??12;
            int Price = price ?? 0;
            int Even = evented ?? 1;
            List<Product> products;
            products = db.Products.Include(p => p.Customer).Include(p => p.Status).Where(p => p.sta_ID > 2).ToList();
            if (HttpContext.Request.Params != null)
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
                if (Request.Params["search"] != null)
                {
                    string[] keys = Request.Params["search"].ToString().Split(' ');
                    foreach (string key in keys)
                    {
                        products = products.Where(p => p.NamePro.Contains(key) && p.sta_ID == 4).ToList();
                    }
                    ViewBag.search = Request.Params["search"].ToString();
                }
            }
            if(Price > 0)
            {
                products = products.Where(p=>p.StartingPrice < Price && p.StartingPrice > Price-100).ToList();
            }
            switch (Even)
            {
                case 2: 
                    products = products.Where(p=>p.StartTime > DateTime.Now).ToList();
                    break;
                case 3:
                    products = products.Where(p => p.StartTime <= DateTime.Now && p.EndTime >= DateTime.Now).ToList();
                    break;
                case 4:
                    products = products.Where(p => p.EndTime < DateTime.Now).ToList();
                    break;
                default:
                    break;
            }
            ViewBag.Shop = products.ToList();
            return View(products.ToPagedList(PageNumber, PageSize));
        }

        public ActionResult Sell_products(int? pi)
        {
            int PageNumber = pi ?? 1;
            int PageSize = 12;
            int cus_id = int.Parse(Session["cus"].ToString());
            var products = db.Products.Where(p => p.cus_ID == cus_id);
            ViewBag.Shop = products;
            return View(products.ToPagedList(PageNumber, PageSize));
        }

        // GET: Shop/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product pro = db.Products.Find(id);
            if (pro == null)
            {
                return HttpNotFound();
            }
            return View(pro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Review(Product pro, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(form["review-star"].ToString()) || string.IsNullOrEmpty(form["review"].ToString()))
                {
                    return View("Details", pro.pro_ID);
                }
                Review review = new Review();
                review.Star = Int32.Parse(form["review-star"].ToString());
                review.Review1 = form["review"].ToString();
                review.REL_Or_Pro = pro.REL_Or_Pro.ElementAt(0);
                db.Reviews.Add(review);
                db.SaveChanges();
                return RedirectToAction("Details", pro.pro_ID);
            }
            return View("Details", pro.pro_ID);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Bid(Product pro, FormCollection form)
        {
            Bid myBid = new Bid();
            myBid.Time = DateTime.Now;
            int price;
            if (!int.TryParse(form["price-input"].ToString(), out price))
            {
                ModelState.AddModelError("price", "Auction is timeout");
                Bid lastBid = db.Bids.Where(b => b.REL_Pro_Au.pro_ID == pro.pro_ID).LastOrDefault();
                if (lastBid == null)
                {
                    Notification no = db.Notifications.FirstOrDefault(n => n.pro_ID == pro.pro_ID);
                    if (no == null)
                    {
                        Notification notification = new Notification();
                        notification.SellCus = pro.cus_ID;
                        notification.NameNo = "Notification for \"" + pro.NamePro + "\"";
                        notification.NoDetails = "Your product \"" + pro.NamePro + "\" does not have any bid";
                        notification.pro_ID = pro.pro_ID;
                        notification.Time = myBid.Time;
                        db.Notifications.Add(notification);
                        db.SaveChanges();
                    }
                    return View("Details", pro.pro_ID);
                }
                REL_Or_Pro rop = new REL_Or_Pro();
                Order orderToday = db.Orders.FirstOrDefault(o => o.MoreInformation.Equals(DateTime.Now.Date.ToString()) && o.cus_ID == lastBid.cus_ID);
                if (orderToday == null)
                {
                    Order order = new Order();
                    order.MoreInformation = DateTime.Now.Date.ToString();
                    order.cus_ID = lastBid.cus_ID;
                    db.Orders.Add(order);
                    db.SaveChanges();
                    order = db.Orders.LastOrDefault();
                    rop.pro_ID = pro.pro_ID;
                    rop.or_ID = order.or_ID;
                    db.REL_Or_Pro.Add(rop);
                    db.SaveChanges();
                }
                else
                {
                    REL_Or_Pro rEL_Or_Pro = db.REL_Or_Pro.FirstOrDefault(r => r.pro_ID == pro.pro_ID && r.or_ID == orderToday.or_ID);
                    if (rEL_Or_Pro == null)
                    {
                        rop.pro_ID = pro.pro_ID;
                        rop.or_ID = orderToday.or_ID;
                        db.REL_Or_Pro.Add(rop);
                        db.SaveChanges();
                    }
                }

                Notification no1 = db.Notifications.FirstOrDefault(n => n.pro_ID == pro.pro_ID);
                if (no1 == null)
                {
                    Notification notification = new Notification();
                    notification.SellCus = pro.cus_ID;
                    notification.BuyCus = lastBid.cus_ID;
                    notification.NameNo = "Notification for \"" + pro.NamePro + "\"";
                    notification.NoDetails = "The product \"" + pro.NamePro + "\" have been ordered";
                    notification.pro_ID = pro.pro_ID;
                    notification.Time = myBid.Time;
                    db.Notifications.Add(notification);
                    db.SaveChanges();
                }
                return View("Details", pro.pro_ID);
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("price", "Enter price to bid");
                return View("Details", pro.pro_ID);
            }

            Bid bid = db.Bids.LastOrDefault(b => b.au_ID == pro.REL_Pro_Au.ElementAt(0).au_ID && b.pro_ID == pro.pro_ID);

            if (bid != null && (bid.Price + pro.StepPrice) > price)
            {
                ModelState.AddModelError("price", "Your price must be greater than or equal to sum of current price and step price");
                return View("Details", pro.pro_ID);
            }

            if (price < (pro.StepPrice + pro.StartingPrice))
            {
                ModelState.AddModelError("price", "Your price must be greater than or equal to sum of current price and step price");
                return View("Details", pro.pro_ID);
            }


            myBid.Price = price;
            myBid.pro_ID = pro.pro_ID;
            myBid.au_ID = pro.REL_Pro_Au.ElementAt(0).au_ID;
            myBid.cus_ID = Int32.Parse(Session["cus"].ToString());
            db.Bids.Add(myBid);
            db.SaveChanges();
            return RedirectToAction("Detail", pro.pro_ID);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RatingCustom(Product pro, int rating_star)
        {
            int your_Id = Int32.Parse(Session["cus"].ToString());
            Rating rate = db.Ratings.FirstOrDefault(r=>r.REL_Or_Pro.pro_ID == pro.pro_ID && r.sent_cus == your_Id);
            if (rate == null && rating_star > 0)
            {
                Rating myRate = new Rating();
                myRate.Star = rating_star;
                myRate.pro_ID = pro.pro_ID;
                myRate.or_ID = pro.REL_Or_Pro.ElementAt(0).or_ID;
                myRate.sent_cus = your_Id;
                myRate.received_cus = your_Id==pro.cus_ID?pro.REL_Or_Pro.ElementAt(0).Order.cus_ID:pro.cus_ID;
                db.Ratings.Add(myRate);
                db.SaveChanges();
                return RedirectToAction("Details", pro.pro_ID);
            }
            return View("Details", pro.pro_ID);
        }

        public ActionResult Auction()
        {
            ViewBag.Auction = db.Auctions.Where(a => a.Stastus == 1 && a.StartTime > DateTime.Now).ToList();
            return View(db.Auctions.Where(a => a.Stastus == 1 && a.StartTime <= DateTime.Now && a.EndTime >= DateTime.Now));
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

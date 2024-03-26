using System;
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
    public class CustomersController : BaseController
    {
        private dbauctionsystemEntities db = new dbauctionsystemEntities();
        private DAOController dataAccess = new DAOController();
        // GET: Customers
        public ActionResult Index(int? pi)
        {
            List<CustomerStar> cus = new List<CustomerStar>();
            string selectQuery = "select Customer.cus_ID,Customer.UserName,Customer.Sex,Customer.Email,Customer.PhoneNumber,AVG(Rating.Star) as Star from Customer left join Orders on Orders.cus_ID = Customer.cus_ID join REL_Or_Pro on REL_Or_Pro.or_ID = Orders.or_ID join Rating on Rating.or_ID = REL_Or_Pro.or_ID group by Customer.cus_ID, Customer.UserName,Customer.Address,Customer.Email,Customer.PhoneNumber, Customer.Sex";
            DataTable result = dataAccess.ExecuteQuery(selectQuery);
            foreach (DataRow row in result.Rows)
            {

                CustomerStar customerStar = new CustomerStar();

                customerStar.cus_ID = Convert.ToInt32(row["cus_ID"]);
                customerStar.UserName = row["UserName"].ToString();
                customerStar.Sex = row["Sex"].ToString();
                customerStar.Email = row["Email"].ToString();
                customerStar.PhoneNumber = row["PhoneNumber"].ToString();
                customerStar.Star = Convert.ToDouble(row["Star"]);
                cus.Add(customerStar); 
            }

            int PageNumber = pi ?? 1;
            int PageSize = 5;
            return View(cus.ToPagedList(PageNumber, PageSize));
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
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

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

namespace project3.Admin.Controllers
{
    public class CustomersStarController : BaseController
    {
        private dbauctionsystemEntities db = new dbauctionsystemEntities();
        private DAOController dataAccess = new DAOController();
        // GET: Customers
        public ActionResult Index(int? pi)
        {
            List<CustomerStar> cus = new List<CustomerStar>();
            string selectQuery = "select Customer.cus_ID,Customer.Img,Customer.UserName,Customer.Address,Customer.PhoneNumber,Customer.Email,AVG(Rating.Star) as Star from Customer left join Rating on Customer.cus_ID = Rating.received_cus where Customer.Status = 1 group by Customer.cus_ID,Customer.Img,Customer.UserName,Customer.Address,Customer.PhoneNumber,Customer.Email";
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
        public ActionResult Block(int? id)
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
        [HttpPost, ActionName("Block")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            customer.Status = 0;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}

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
    public class EmployeesController : BaseController
    {
        private dbauctionsystemEntities db = new dbauctionsystemEntities();


        public ActionResult Index(int? pi)
        {
            ViewBag.em = db.Employees.Where(e => e.Status == 1).ToList();
            int PageNumber = pi ?? 1;
            int PageSize = 5;
            var employee = db.Employees.Where(e => e.Status != 1).ToList();
            return View(employee.ToPagedList(PageNumber, PageSize));
        }
        public ActionResult Inspect(List<int> emID)
        {
            if (emID != null)
            {
                foreach (var value in emID.ToList())
                {
                    Employee employee = db.Employees.Where(e => e.em_ID == value).FirstOrDefault();
                    if (employee != null)
                    {
                        employee.Status = 2;
                        db.Entry(employee).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }
                return RedirectToAction("Index");
            }
            return View("Index");
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Find(id);
            employee.Status = 0;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Decentralization(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.FirstOrDefault(e => e.em_ID == id);
            ViewBag.Group = db.Groups.ToList();
            if (employee == null)
            {
                return HttpNotFound();
            }

            return View(employee);
        }
        [HttpPost, ActionName("Decentralization")]
        [ValidateAntiForgeryToken]
        public ActionResult Decentralization(int id, List<int> gruID)
        {
            DAOController dataAccess = new DAOController();
            //int idd = Int32.Parse(RouteData.Values["id"] as string);

            if (gruID != null)
            {
                string deleteQuery = "DELETE FROM REL_Em_Gru WHERE REL_Em_Gru.em_ID = " + id + "";
                bool deleted = dataAccess.ExecuteDelete(deleteQuery);

                foreach (var value in gruID.ToList())
                {
                    string sqlQuery = "INSERT INTO REL_Em_Gru (REL_Em_Gru.em_ID,REL_Em_Gru.gru_ID) VALUES (" + id + "," + value + ")";
                    int rowsInserted = dataAccess.ExecuteInsert(sqlQuery);
                    if (rowsInserted > 0)
                    {

                    }
                }


                return RedirectToAction("Index");


            }
            return View();
        }
    }
}


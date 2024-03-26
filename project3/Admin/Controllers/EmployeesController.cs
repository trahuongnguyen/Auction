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

        // GET: Employees
       
        public ActionResult Index(int? pi)
        {
            ViewBag.em = db.Employees.Where(e=>e.Status==1).ToList();
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
                    Employee employee = db.Employees.Where(e=>e.em_ID==value).FirstOrDefault();
                    if(employee != null)
                    {
                        employee.Status = 2;
                        emID.Remove(value);
                        db.Entry(employee).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
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
    }
}

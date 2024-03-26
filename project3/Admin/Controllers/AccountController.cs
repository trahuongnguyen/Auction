using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using project3.Models;
using BCrypt.Net;
namespace project3.Admin.Controllers
{
    public class AccountController : Controller
    {
        private dbauctionsystemEntities db = new dbauctionsystemEntities();
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string uname, string pass)
        {
            Employee employee = db.Employees.Where(e => e.UserName.Equals(uname)).FirstOrDefault();


            if (employee != null)
            {
                int stat = employee.Status;
                string hashedPassword = employee.Password;
                if (stat != 1 && BCrypt.Net.BCrypt.Verify(pass, hashedPassword))
                {
                    Session["uName"] = employee.UserName;
                    Session["fName"] = employee.FirstName;
                    Session["id"] = employee.em_ID;
                    var employees = db.Employees.Where(e => e.Status == 1).ToList();
                    TempData["EmList"] = employees;
                    var products = db.Products.Include(p=>p.Status).Where(p => p.sta_ID == 1).ToList();
                    TempData["ProList"] = products;
                    return RedirectToAction("Index", "/Home");
                }
                else if (!BCrypt.Net.BCrypt.Verify(pass, hashedPassword))
                {
                    ModelState.AddModelError(nameof(employee.Password), "Wrong Password!!!");
                }
                else
                {
                    ViewBag.Message = String.Format("You need to wait for the administrator to approve");
                }

            }
            else
            {
                ModelState.AddModelError(nameof(employee.UserName), "Wrong UserName!!!");
            }
            return View();
        }
        public ActionResult Details()
        {
            var value = Session["uName"].ToString();
            var employee = db.Employees.Where(e => e.UserName == value).FirstOrDefault();
            return View(employee);
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "em_ID,FirstName,LastName,UserName,Password,Sex,Address,Email,PhoneNumber")] Employee employee)
        {
            if (!IsDupplicated(employee))
            {
                if (ModelState.IsValid)
                {
                    string salt = BCrypt.Net.BCrypt.GenerateSalt();
                    string hash = BCrypt.Net.BCrypt.HashPassword(employee.Password, salt);
                    employee.Password = hash;
                    employee.Status = 1;
                    db.Employees.Add(employee);
                    db.SaveChanges();
                    ViewBag.Message = String.Format("You need to wait for the administrator to approve");
                }
            }

            return View();
        }
       
        public ActionResult Logout()
        {
            Session.Remove("uName");
            Session.Remove("fName");
            Session.Remove("id");
            TempData.Remove("EmList");
            TempData.Remove("ProList");
            return RedirectToAction("Login");
        }
        // GET: Logins/Edit/5
        public ActionResult ChangePassword()
        {
            if(Session != null && Session["uName"] != null )
            {
                var value = Session["uName"].ToString();
                Employee employee = db.Employees.Where(e => e.UserName == value).FirstOrDefault();
                return View(employee);         
            }
            else
            {
                return RedirectToAction("Login");
            }
            

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword([Bind(Include = "em_ID,Password")] Employee employee, string passn)
        {
            var em = db.Employees.Where(e => e.em_ID == employee.em_ID).FirstOrDefault();
            if (BCrypt.Net.BCrypt.Verify(employee.Password, em.Password) && ModelState.IsValid)
            {
                string salt = BCrypt.Net.BCrypt.GenerateSalt().ToString();
                string hash = BCrypt.Net.BCrypt.HashPassword(passn, salt);
                Console.WriteLine(hash);
                em.Password = hash;
                db.Entry(em).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.Message = String.Format("Changed Password Successfully");
                return View();
            }
            else
            {
                ModelState.AddModelError(nameof(employee.Password), "Wrong Password!!!");
            }
            return View(employee);
        }
        private bool IsDupplicated(Employee employee)
        {
            bool Flag = false;
            var employeeUser = db.Employees.ToList().Where(p => p.UserName.Equals(employee.UserName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (employeeUser != null)
            {
                ModelState.AddModelError(nameof(employee.UserName), "User is Dupplicate");
                Flag = true;
            }
            var employeeEmail = db.Employees.ToList().Where(p => p.Email.Equals(employee.Email, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (employeeEmail != null)
            {
                ModelState.AddModelError(nameof(employee.Email), "Email is Dupplicate");
                Flag = true;
            }
            var employeePhone = db.Employees.ToList().Where(p => p.PhoneNumber.Equals(employee.PhoneNumber, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (employeePhone != null)
            {
                ModelState.AddModelError(nameof(employee.PhoneNumber), "Email is Dupplicate");
                Flag = true;
            }
            return Flag;
        }
    }
}

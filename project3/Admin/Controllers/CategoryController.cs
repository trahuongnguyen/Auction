using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using project3.Models;

namespace project3.Admin.Controllers
{
    public class CategoryController : Controller
    {
        private dbauctionsystemEntities2 db = new dbauctionsystemEntities2();

        // GET: Category
        public ActionResult Index()
        {
            var categories = db.Categories.Include(c => c.Category2);
            return View(categories.ToList());
        }

        // GET: Category/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: Category/Create
        public ActionResult Create()
        {
            ViewBag.Parent_ID = new SelectList(db.Categories.ToList(), "cat_ID", "NameCat");
            return View();
        }

        // POST: Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cat_ID,NameCat,Parent_ID,Characteristic,Status")] Category category, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                if (Image != null && Image.ContentLength > 0)
                {
                    if (!Path.GetExtension(Image.FileName).ToLower().Equals(".png") && !Path.GetExtension(Image.FileName).ToLower().Equals(".jpg"))
                    {
                        ModelState.AddModelError("Image", "Please choose file type .png or .jpg");
                        return View(category);
                    }
                    if (Image.ContentLength > 300000)
                    {
                        ModelState.AddModelError("Image", "This File is out of 300KB");
                        return View(category);
                    }
                    try
                    {
                        using (var binaryReader = new BinaryReader(Image.InputStream))
                        {
                            category.Image = binaryReader.ReadBytes(Image.ContentLength);
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Parent_ID = new SelectList(db.Categories.ToList(), "cat_ID", "NameCat", category.Parent_ID);
            return View(category);
        }

        // GET: Category/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            ViewBag.Parent_ID = new SelectList(db.Categories, "cat_ID", "NameCat", category.Parent_ID);
            return View(category);
        }

        // POST: Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cat_ID,NameCat,Parent_ID,Characteristic,Image,Status")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Parent_ID = new SelectList(db.Categories, "cat_ID", "NameCat", category.Parent_ID);
            return View(category);
        }

        // GET: Category/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
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

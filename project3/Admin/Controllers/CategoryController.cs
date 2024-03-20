using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
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
        private dbauctionsystemEntities db = new dbauctionsystemEntities();

        public ActionResult Index(int? pi)
        {
            int PageNumber = pi ?? 1;
            int PageSize = 5;
            var categories = db.Categories.Include(c => c.Category2);
            //List<Category> query = db.Categories.OrderBy(c => c.cat_ID).Include(c => c.Category2).Skip(10).Take(10).ToList();
            return View(categories.ToList());
        }

        public ActionResult Create()
        {
            ViewBag.Parent = new SelectList(db.Categories, "cat_ID", "NameCat");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cat_ID,NameCat,Parent_ID,Characteristic,Status")] Category category, HttpPostedFileBase Image)
        {
            if (!IsDupplicated(category))
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
                    category.Status = 1;
                    db.Categories.Add(category);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.Parent = new SelectList(db.Categories, "cat_ID", "NameCat", category.Parent_ID);
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
            ViewBag.Parent = new SelectList(db.Categories, "cat_ID", "NameCat", category.Parent_ID);
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cat_ID,NameCat,Parent_ID,Characteristic")] Category category, HttpPostedFileBase Image)
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
                category.Status = 1;
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Parent = new SelectList(db.Categories, "cat_ID", "NameCat", category.Parent_ID);
            return View(category);
        }

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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            category.Status = 0;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public bool IsDupplicated(Category cat)
        {
            bool Flag = false;
            var Cat = db.Categories.ToList().Where(c => c.NameCat.Equals(cat.NameCat, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (Cat != null)
            {
                ModelState.AddModelError(nameof(cat.NameCat), "Name is Dupplicate");
                Flag = true;
            }

            return Flag;
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

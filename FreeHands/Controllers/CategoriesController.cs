using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FreeHands.Models;

namespace FreeHands.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private DB db = new DB();

        // GET: Categories
        public ActionResult Index()
        {
            return View(db.Categories.OrderByDescending(x=>x.Products.Count).ToList());
        }


        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name")] Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    db.Categories.Add(category);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.error ="برجاء ادخال البيانات بطريقة صحيحة !!";
            }
            catch(Exception ex)
            {
                ViewBag.error = ex.Message;
            }

            return View("Index", db.Categories.OrderByDescending(x => x.Products.Count).ToList());
        }

        // GET: Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name")] Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(category).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.error = "برجاء ادخال البيانات بطريقة صحيحة !!";
            }
            catch(Exception ex)
            {
                ViewBag.error = ex.Message;
            }
            return View(category);
        }


        // POST: Categories/Delete/5
        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                Category category = db.Categories.Find(id);
                db.Categories.Remove(category);
                db.SaveChanges();
                return Json(id);
            }
            catch
            {
                return Json(0);
            }
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FreeHands.Models;
using System.IO;

namespace FreeHands.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private DB db = new DB();

        // GET: Products
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Category).Include(p => p.Provider);
            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }


        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Description,Price,Cat_ID,Provider_ID,TimeToFinish")] Product product,HttpPostedFileBase Photo)
        {
            try
            {
                if (ModelState.IsValid && Photo != null)
                {
                    db.Products.Add(product);
                    db.SaveChanges();
                    Photo.SaveAs(Server.MapPath("~/Uploads/Products/" + product.ID + ".jpg"));
                    return RedirectToAction("Index");
                }
                ViewBag.error = "برجاء ادخال البيانات بطريقة صحيحة !!";
            }
            catch(Exception ex)
            {
                ViewBag.error = ex.Message;
            }
            return View("Index", db.Products.Include(p => p.Category).Include(p => p.Provider).ToList());
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Description,Price,Cat_ID,Provider_ID,TimeToFinish")] Product product,HttpPostedFileBase Photo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                    if (Photo != null)
                    {
                        Photo.SaveAs(Server.MapPath("~/Uploads/Products/" + product.ID + ".jpg"));
                    }
                    return RedirectToAction("Index");
                }
                ViewBag.error = "برجاء ادخال البيانات بطريقة صحيحة !!";
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                Product product = db.Products.Find(id);

                db.Products.Remove(product);
                db.SaveChanges();
                FileInfo F = new FileInfo(Server.MapPath("~/Uploads/Products/" + id + ".jpg"));
                if (F.Exists)
                {
                    F.Delete();
                }
                return Json(id);
            }
            catch
            {
                return Json(0);
            }
        }
        [HttpPost]
        public JsonResult DeleteOffer(int id)
        {
            try
            {
                Offer offer = db.Offers.Find(id);
                db.Offers.Remove(offer);
                db.SaveChanges();
                return Json(id);
            }
            catch
            {
                return Json(0);
            }
        }
        [HttpPost]
        public JsonResult DeleteOption(int id)
        {
            try
            {
                Option option = db.Options.Find(id);
                db.Options.Remove(option);
                db.SaveChanges();
                return Json(id);
            }
            catch
            {
                return Json(0);
            }
        }
        [HttpPost]
        public JsonResult DeleteComment(int id)
        {
            try
            {
                Product_Rates productrate = db.Product_Rates.Find(id);
                db.Product_Rates.Remove(productrate);
                db.SaveChanges();
                return Json(id);
            }
            catch
            {
                return Json(0);
            }
        }
        [HttpPost]
        public JsonResult HideShowComment(int id,bool visible)
        {
            try
            {
                Product_Rates productrate = db.Product_Rates.Find(id);
                productrate.Avaiable = visible;
                db.Entry(productrate).State = EntityState.Modified;
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

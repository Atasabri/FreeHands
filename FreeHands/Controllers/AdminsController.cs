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
    public class AdminsController : Controller
    {
        private DB db = new DB();

        // GET: Admins
        public ActionResult Index()
        {
            return View(db.Admins.ToList());
        }

        // GET: Admins/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Admin admin = db.Admins.Find(id);
           
            if (admin == null)
            {
                return HttpNotFound();
            }
            int admin_id = int.Parse(User.Identity.Name);
            Admin _admin = db.Admins.Find(admin_id);
            if (_admin.IsManager || _admin.ID == admin.ID)
            {
                return View(admin);
            }
            return RedirectToAction("Index");
        }
        // POST: Admins/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserName,Password,IsManager")] Admin admin,HttpPostedFileBase Photo)
        {
            try
            {
                if (ModelState.IsValid && Photo != null)
                {
                    if (db.Admins.Any(x => x.UserName == admin.UserName))
                    {
                        ViewBag.error = "اسم المستخدم موجود !! برجاء استخدام اسم اخر .";
                        return View("Index", db.Admins.ToList());
                    }
                    db.Admins.Add(admin);
                    db.SaveChanges();
                    Photo.SaveAs(Server.MapPath("~/Uploads/Admins/" + admin.ID + ".jpg"));
                    return RedirectToAction("Index");
                }
                ViewBag.error = "البيانات المدخلة غير صحيحة !!.. من فضلك قم بادخال البيانات بشكل صحيح";
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
            }
            return View("Index", db.Admins.ToList());
        }

        // GET: Admins/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Admin admin = db.Admins.Find(id);            
            if (admin == null)
            {
                return HttpNotFound();
            }
            int admin_id = int.Parse(User.Identity.Name);
            Admin _admin = db.Admins.Find(admin_id);
            if (!_admin.IsManager)
            {
                return RedirectToAction("Index");
            }
            return View(admin);
        }

        // POST: Admins/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserName,Password,IsManager")] Admin admin,HttpPostedFileBase Photo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (db.Admins.Where(x=>x.ID!=admin.ID).Any(x => x.UserName == admin.UserName))
                    {
                        throw new Exception("اسم المستخدم موجود بالفعل .. برجاء استخدام اسم اخر .");
                    }
                    db.Entry(admin).State = EntityState.Modified;
                    db.SaveChanges();
                    if (Photo != null)
                    {
                        Photo.SaveAs(Server.MapPath("~/Uploads/Admins/" + admin.ID + ".jpg"));
                    }
                    int admin_id = int.Parse(User.Identity.Name);
                    if (admin_id == admin.ID)
                    {
                        return RedirectToAction("logout", "Auth");
                    }
                    return RedirectToAction("Index");
                }
                ViewBag.error = "البيانات المدخلة غير صحيحة !!.. من فضلك قم بادخال البيانات بشكل صحيح";
            }catch(Exception ex)
            {
                ViewBag.error = ex.Message;
            }
            return View(admin);
        }


        // POST: Admins/Delete/5
        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                Admin admin = db.Admins.Find(id);
                db.Admins.Remove(admin);
                db.SaveChanges();
                FileInfo F = new FileInfo(Server.MapPath("~/Uploads/Admins/" + id + ".jpg"));
                if (F.Exists)
                {
                    F.Delete();
                }
                return Json(id);
            }catch
            {
                return Json(0);
            }
        }



        // Tasks ____________________________________________________________________________________________
        public ActionResult Tasks()
        {
            int admin_id = int.Parse(User.Identity.Name);
            Admin _admin = db.Admins.Find(admin_id);
            if (_admin.IsManager)
            {
                return View(db.Tasks.Where(x => x.FromManager == admin_id).ToList());
            }
            else
            {
                return View(db.Tasks.Where(x => x.ToAdmin == admin_id).ToList());
            }
        }
        [HttpPost]
        public ActionResult AddTask(Task task)
        {
            try
            {
                int admin_id = int.Parse(User.Identity.Name);
                Admin _admin = db.Admins.Find(admin_id);
                if(_admin.IsManager==false)
                {
                    return HttpNotFound();
                }
                task.FromManager = admin_id;
                db.Tasks.Add(task);
                db.SaveChanges();
                return RedirectToAction("Tasks");
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
            }
             return RedirectToAction("Tasks");
        }
        [HttpPost]
        public JsonResult DeleteTask(int id)
        {
            int admin_id = int.Parse(User.Identity.Name);
            Admin _admin = db.Admins.Find(admin_id);
            if (!_admin.IsManager)
            {
                return Json(0);
            }
            Task task = db.Tasks.Find(id);
            db.Tasks.Remove(task);
            db.SaveChanges();
            return Json(id);
        }
        [HttpPost]
        public JsonResult FinishTask(int id)
        {
            int admin_id = int.Parse(User.Identity.Name);
            Admin _admin = db.Admins.Find(admin_id);
            if (_admin.IsManager)
            {
                return Json(0);
            }
            Task task = db.Tasks.Find(id);
            task.Finished = true;
            db.Entry(task).State = EntityState.Modified;
            db.SaveChanges();
            return Json(id);
        }




        //Dashboard & Edit Settings ________________________________________________________________________
        public ActionResult Dashboard()
        {
            return View(db.Data.Find(1));
        }
        public ActionResult EditSettings()
        {
            return View(db.Data.Find(1));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSettings(Datum data,FormCollection form)
        {
            try
            {
                if(form["AllowKnownNumber"]!=null)
                {
                    data.AllowKnownNumber = true;
                }
                if(form["AllowVisa"] != null)
                {
                    data.AllowVisa = true;
                }
                data.ID = 1;
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Dashboard");

            }catch(Exception ex)
            {
                ViewBag.error = ex.Message;
                return View(data);
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

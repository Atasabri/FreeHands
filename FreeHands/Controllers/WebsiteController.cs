using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FreeHands.Models;

namespace FreeHands.Controllers
{
    public class WebsiteController : Controller
    {
        DB db = new DB();
        // GET: Website
        public ActionResult Subscribers()
        {
            return View(db.Subscribers.ToList());
        }
        public ActionResult Contacts()
        {
            return View(db.Contacts.ToList());
        }
        [HttpPost]
        public ActionResult SendMail(string Subject, HttpPostedFileBase file, List<string> fooa)
        {
            if (fooa != null && fooa.Count() >= 1 && file != null)
            {
                List<string> Users = fooa.Distinct().ToList();
                DEL.Send_Mail(Subject, file, Users);
            }
            return Redirect(Request.UrlReferrer.AbsolutePath);
        }
        [HttpPost]
        public JsonResult DeleteEmail(int id)
        {
            try
            {
                Subscriber subscriber = db.Subscribers.Find(id);
                db.Subscribers.Remove(subscriber);
                db.SaveChanges();
                return Json(id);
            }
            catch
            {
                return Json(0);
            }
        }
        [HttpPost]
        public JsonResult DeleteContact(int id)
        {
            try
            {
                Contact contact = db.Contacts.Find(id);
                db.Contacts.Remove(contact);
                db.SaveChanges();
                return Json(id);
            }
            catch
            {
                return Json(0);
            }
        }
    }
}
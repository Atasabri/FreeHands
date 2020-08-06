using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FreeHands.Models;

namespace FreeHands.Controllers
{
    public class HomeController : Controller
    {
        DB db = new DB();
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Subscribe(string Email)
        {
            Subscriber sub = new Subscriber { Email = Email };
            db.Subscribers.Add(sub);
            db.SaveChanges();
            return Json("شكرا لك علي المتابعة");
        }
        [HttpPost]
        public JsonResult Contact(Contact contact)
        {
            try
            {
                db.Contacts.Add(contact);
                db.SaveChanges();
            }
            catch
            {
                return Json("حدث خطأ في الاضافة");
            }
            return Json("تم ارسال رسالتك بنجاح , شكرا لك");
        }
    }
}

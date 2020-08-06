using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FreeHands.Models;
using System.Web.Security;

namespace FreeHands.Controllers
{
    public class AuthController : Controller
    {
        DB db = new DB();
        // GET: Auth
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string Name, string Password)
        {
            Admin admin = db.Admins.SingleOrDefault(x => x.UserName == Name && x.Password == Password);
            if (admin != null)
            {
                FormsAuthentication.SetAuthCookie(admin.ID.ToString(), false);
                return RedirectToAction("Dashboard", "Admins");
            }
            ViewBag.error = "هذا المستخدم غير موجود , برجاء ادخال بيانات صحيحة !";
            return View();
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}
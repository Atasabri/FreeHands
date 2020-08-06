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
    public class UsersController : Controller
    {
        private DB db = new DB();

        // GET: Users
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }
        // POST: Users/Delete/5
        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                User user = db.Users.Find(id);
                db.Users.Remove(user);
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

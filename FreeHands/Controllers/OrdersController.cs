using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FreeHands.Models;

namespace FreeHands.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        DB db = new DB();
        // GET: Orders
        public ActionResult Index()
        {
            return View(db.Orders.OrderByDescending(x=>x.Date).Where(x=>x.IsOrderd).ToList());
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                Order order = db.Orders.Find(id);
                db.Orders.Remove(order);
                db.SaveChanges();
                return Json(id);
            }
            catch
            {
                return Json(0);
            }
        }
        public ActionResult Details(int id)
        {
            Order order = db.Orders.Find(id);
            if(order==null)
            {
                return RedirectToAction("Index");
            }
            return View(order);
        }


        //Statices ________________________________________________________________________________

        [HttpPost]
        public ActionResult GetOrders(DateTime FromDate,DateTime ToDate)
        {
            ToDate = ToDate.AddDays(1);
            var Data = db.Orders.Where(x => x.IsOrderd & x.Date.CompareTo(FromDate) > 0).Where(x=> ToDate.CompareTo(x.Date) > 0).ToList();
            ViewBag.Head = "من بداية يوم  : "+FromDate.ToString("d")+"الي نهاية يوم :" +ToDate.AddDays(-1).ToString("d");
            return View("Data",Data);
        }
        public ActionResult StillInprocess()
        {
            var Data = db.Orders.Where(x => x.IsOrderd && !x.ISArrivedToUser).ToList();
            ViewBag.Head = "طلبات لم تصل الي العميل بعد";
            return View("Data", Data);
        }
        public ActionResult NoDriver()
        {
            var Data = db.Orders.Where(x => x.IsOrderd && !x.Driver_ID.HasValue&& !x.ISArrivedToUser).ToList();
            ViewBag.Head = "طلبات لم تصل لا يوجد لها سائقين";
            return View("Data", Data);
        }
        public ActionResult Problem()
        {
            var Data = db.Orders.Where(x => x.IsOrderd && x.ProblemInArriveToUser).ToList();
            ViewBag.Head = "طلبات لم يستلمها العميل ";
            return View("Data", Data);
        }
    }
}
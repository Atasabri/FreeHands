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
    public class DriversController : Controller
    {
        private DB db = new DB();

        // GET: Drivers
        public ActionResult Index()
        {
            var drivers = db.Drivers.Include(d => d.Company);
            return View(drivers.ToList());
        }

        // GET: Drivers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Driver driver = db.Drivers.Find(id);
            if (driver == null)
            {
                return HttpNotFound();
            }
            return View(driver);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,FirstName,LastName,Email,Token,Password,Phone,Company_ID,Location,Visible,Country_ID,City_ID")] Driver driver, HttpPostedFileBase Photo, HttpPostedFileBase Driver_Car_License, HttpPostedFileBase Driver_License)
        {
            try
            {
                driver.Token = DEL.encrypt(driver.Email);
                if ( Photo != null && Driver_License != null && Driver_Car_License != null)
                {
                    db.Drivers.Add(driver);
                    db.SaveChanges();
                    Photo.SaveAs(Server.MapPath("~/Uploads/Driver_Photo/" + driver.ID + ".jpg"));
                    Driver_License.SaveAs(Server.MapPath("~/Uploads/Driver_License/" + driver.ID + ".jpg"));
                    Driver_Car_License.SaveAs(Server.MapPath("~/Uploads/Driver_Car_License/" + driver.ID + ".jpg"));
                    return RedirectToAction("Index");
                }
                ViewBag.error = "برجاء ادخال البيانات بطريقة صحيحة !!";
            }
            catch (Exception ex) {

                ViewBag.error = ex.Message;
            }
            return View("Index", db.Drivers.Include(d => d.Company).ToList());
        }

        // GET: Drivers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Driver driver = db.Drivers.Find(id);
            if (driver == null)
            {
                return HttpNotFound();
            }
            return View(driver);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FirstName,LastName,Email,Token,Password,Phone,Location,Company_ID,Visible,Country_ID,City_ID")] Driver driver, HttpPostedFileBase Photo, HttpPostedFileBase Driver_Car_License, HttpPostedFileBase Driver_License)
        {
            try
            {
                db.Entry(driver).State = EntityState.Modified;
                db.SaveChanges();
                if (Photo != null)
                {
                    Photo.SaveAs(Server.MapPath("~/Uploads/Driver_Photo/" + driver.ID + ".jpg"));
                }
                if (Driver_License != null)
                {
                    Photo.SaveAs(Server.MapPath("~/Uploads/Driver_License/" + driver.ID + ".jpg"));
                }
                if (Driver_Car_License != null)
                {
                    Photo.SaveAs(Server.MapPath("~/Uploads/Driver_Car_License/" + driver.ID + ".jpg"));
                }
                return RedirectToAction("Index");
            }

            catch (Exception ex){ ViewBag.error = ex.Message; }
            return View(driver);
        }

        // POST: Drivers/Delete/5
        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                Driver driver = db.Drivers.Find(id);
                db.Drivers.Remove(driver);
                db.SaveChanges();
                FileInfo F = new FileInfo(Server.MapPath("~/Uploads/Driver_Photo/"+id+".jpg"));
                if(F.Exists)
                {
                    F.Delete();
                }
                FileInfo F1 = new FileInfo(Server.MapPath("~/Uploads/Driver_License/" + id + ".jpg"));
                if (F1.Exists)
                {
                    F1.Delete();
                }
                FileInfo F2 = new FileInfo(Server.MapPath("~/Uploads/Driver_Car_License/" + id + ".jpg"));
                if (F2.Exists)
                {
                    F2.Delete();
                }
                return Json(id);
            }
            catch
            {
                return Json(0);
            }
        }
        [HttpPost]
        public JsonResult ChangeDriverVisible(int id,bool visible)
        {
            try
            {
                Driver driver = db.Drivers.Find(id);
                driver.Visible = visible;
                db.Entry(driver).State = EntityState.Modified;
                db.SaveChanges();
                return Json(id);
            }
            catch
            {
                return Json(0);
            }
        }







        // Money Data & Statices ____________________________________________________________________
        [HttpPost]
        public ActionResult Data(DateTime FromDate, DateTime ToDate)
        {
            ToDate = ToDate.AddDays(1);
            var orders = db.Drivers.Where(x=>!x.Company_ID.HasValue).Select(x => new
            {
                x.ID,
                x.FirstName,
                orders = x.Orders.Where(o => o.ISArrivedToUser && o.Date.CompareTo(FromDate) > 0&&ToDate.AddDays(1).CompareTo(o.Date) > 0).Select(N => new {
                    N.IsUseVisa,
                    N.Price,
                    N.Delivery,
                    N.Total
                }),
            });
            var Data = orders.Where(x => x.orders.Count() > 0).AsEnumerable().Select(x => new Data
            {
                ID = x.ID,
                TradeName = x.FirstName,
                OrdersCount = x.orders.Where(O => !O.IsUseVisa).Count(),
                OrdersPrice = x.orders.Where(O => !O.IsUseVisa && O.Total.HasValue).Sum(O => O.Total).Value,
                OrdersPriceT = x.orders.Where(O => !O.IsUseVisa && O.Total.HasValue).Sum(O => O.Total).Value - x.orders.Where(O => !O.IsUseVisa && O.Delivery.HasValue).Sum(O => O.Delivery).Value,

                OrdersCountV = x.orders.Where(O => O.IsUseVisa).Count(),
                OrdersPriceV = x.orders.Where(O => O.IsUseVisa && O.Total.HasValue).Sum(O => O.Total).Value,
                OrdersPriceTV = x.orders.Where(O => O.IsUseVisa && O.Delivery.HasValue).Sum(O => O.Delivery).Value,
            });
            ViewBag.Head = " بيانات التاريخ  للسائقين الغير مقيدين لشركات " + "من بداية يوم  " + FromDate.ToString("d") + "  الي نهاية يوم " + ToDate.AddDays(-1).ToString("d");
            return View("DriversData", Data);
        }
        public ActionResult UnVisibleDrivers()
        {
            var Data = db.Drivers.Where(x => !x.Visible).ToList();
            ViewBag.Head = "السائقين الغير متاحين";
            return View("Data", Data);
        }
        public ActionResult NotWorkDrivers()
        {
            var Data = db.Drivers.Where(x => x.Orders.Count<1).ToList();
            ViewBag.Head = "السائقين لم يصلوا اي طلب";
            return View("Data", Data);
        }
        public ActionResult FreeLancerDrivers()
        {
            var Data = db.Drivers.Where(x =>!x.Company_ID.HasValue).ToList();
            ViewBag.Head = "سائقين لا ينتموا لاي شركة";
            return View("Data", Data);
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

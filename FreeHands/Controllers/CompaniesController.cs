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
    public class CompaniesController : Controller
    {
        private DB db = new DB();

        // GET: Companies
        public ActionResult Index()
        {
            return View(db.Companies.ToList());
        }

        // GET: Companies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }
        // POST: Companies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,UserName,Password,Phone,Address")] Company company)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Companies.Add(company);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.error = "برجاء ادخال البيانات بطريقة صحيحة !!";
            }
            catch(Exception ex)
            {
                ViewBag.error = ex.Message;
            }
            return View("Index", db.Companies.ToList());
        }

        // GET: Companies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,UserName,Password,Phone,Address")] Company company)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(company).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.error = "برجاء ادخال البيانات بطريقة صحيحة !!";
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
            }
            return View(company);
        }
        // POST: Companies/Delete/5
        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                Company company = db.Companies.Find(id);
                db.Companies.Remove(company);
                db.SaveChanges();
                return Json(id);
            }
            catch
            {
                return Json(0);
            }
        }


        //Money Calculate ________________________________________________________________________________

        [HttpPost]
        public ActionResult Data(DateTime FromDate, DateTime ToDate)
        {
            ToDate = ToDate.AddDays(1);
            var orders = db.Companies.Select(x => new
            {
                x.ID,
                x.Name,
                orders = x.Drivers.SelectMany(O=>O.Orders.Where(o => o.ISArrivedToUser && o.Date.CompareTo(FromDate) > 0&&ToDate.CompareTo(o.Date) > 0).Select(N=>new {
                    N.IsUseVisa,
                    N.Price,
                    N.Delivery,
                    N.Total
                })),
            });
            var Data = orders.Where(x => x.orders.Count() > 0).AsEnumerable().Select(x => new Data
            {
                ID = x.ID,
                TradeName = x.Name,
                OrdersCount = x.orders.Where(O => !O.IsUseVisa).Count(),
                OrdersPrice = x.orders.Where(O => !O.IsUseVisa && O.Total.HasValue).Sum(O => O.Total).Value,
                OrdersPriceT = x.orders.Where(O => !O.IsUseVisa && O.Total.HasValue).Sum(O => O.Total).Value- x.orders.Where(O => !O.IsUseVisa && O.Delivery.HasValue).Sum(O => O.Delivery).Value,

                OrdersCountV = x.orders.Where(O => O.IsUseVisa).Count(),
                OrdersPriceV = x.orders.Where(O => O.IsUseVisa && O.Total.HasValue).Sum(O => O.Total).Value,
                OrdersPriceTV = x.orders.Where(O => O.IsUseVisa && O.Delivery.HasValue).Sum(O => O.Delivery).Value,
            });
            ViewBag.Head =" بيانات التاريخ لشركات التوصيل "+" من بداية يوم  "+FromDate.ToString("d")+"الي نهاية يوم"+ToDate.AddDays(-1).ToString("d");
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

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
    public class ProvidersController : Controller
    {
        private DB db = new DB();

        // GET: Providers
        public ActionResult Index()
        {
            return View(db.Providers.ToList());
        }

        // GET: Providers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Provider provider = db.Providers.Find(id);
            if (provider == null)
            {
                return HttpNotFound();
            }
            return View(provider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,FirstName,LastName,TradeName,Token,Email,Password,Phone,Lat,Log,Location,IsAvaiable,AllowVisa,IsExcelent,KnownNumber,Country_ID,City_ID,Currency")] Provider provider,HttpPostedFileBase Photo,HttpPostedFileBase IdentityPhoto)
        {
            try
            {
                if (ModelState.IsValid && Photo != null && IdentityPhoto != null)
                {
                    provider.Token = DEL.encrypt(provider.TradeName);
                    db.Providers.Add(provider);
                    db.SaveChanges();
                    Photo.SaveAs(Server.MapPath("~/Uploads/Provider_Photo/" + provider.ID + ".jpg"));
                    IdentityPhoto.SaveAs(Server.MapPath("~/Uploads/Provider_Identity/" + provider.ID + ".jpg"));
                    return RedirectToAction("Index");
                }
                ViewBag.error = "برجاء ادخال البيانات بطريقة صحيحة !!";
            }
            catch(Exception ex)
            {
                ViewBag.error = ex.Message;
            }

            return View("Index", db.Providers.ToList());
        }

        // GET: Providers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Provider provider = db.Providers.Find(id);
            if (provider == null)
            {
                return HttpNotFound();
            }
            return View(provider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FirstName,LastName,TradeName,Token,Email,Password,Phone,Lat,Log,Location,IsAvaiable,AllowVisa,IsExcelent,KnownNumber,Country_ID,City_ID,Currency")] Provider provider,HttpPostedFileBase Photo,HttpPostedFileBase IdentityPhoto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(provider).State = EntityState.Modified;
                    db.SaveChanges();
                    if (Photo != null)
                    {
                        Photo.SaveAs(Server.MapPath("~/Uploads/Provider_Photo/" + provider.ID + ".jpg"));
                    }
                    if (IdentityPhoto != null)
                    {
                        IdentityPhoto.SaveAs(Server.MapPath("~/Uploads/Provider_Identity/" + provider.ID + ".jpg"));
                    }
                    return RedirectToAction("Index");
                }
                ViewBag.error = "برجاء ادخال البيانات بطريقة صحيحة !!";
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
            }
            return View(provider);
        }
        // POST: Providers/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                Provider provider = db.Providers.Find(id);
                var products = db.Products.Where(x => x.Provider_ID == id);
                db.Providers.Remove(provider);
                db.Products.RemoveRange(products);
                db.SaveChanges();
                FileInfo F = new FileInfo(Server.MapPath("~/Uploads/Provider_Photo/" + id + ".jpg"));
                if (F.Exists)
                {
                    F.Delete();
                }
                FileInfo F1 = new FileInfo(Server.MapPath("~/Uploads/Provider_Identity/" + id + ".jpg"));
                if (F1.Exists)
                {
                    F1.Delete();
                }
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
                Provider_Rates provrate = db.Provider_Rates.Find(id);
                db.Provider_Rates.Remove(provrate);
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
                Provider_Rates provrate = db.Provider_Rates.Find(id);
                provrate.Avaiable = visible;
                db.Entry(provrate).State = EntityState.Modified;
                db.SaveChanges();
                return Json(id);
            }
            catch
            {
                return Json(0);
            }
        }
        [HttpPost]
        public JsonResult ChangeExcelent(int id,bool isexcelent)
        {
            try
            {
                Provider provider = db.Providers.Find(id);
                provider.IsExcelent = isexcelent;
                db.SaveChanges();
                return Json(id);
            }
            catch
            {
                return Json(0);
            }
        }
        
         [HttpPost]
        public JsonResult ChangeAccept(int id, bool isaccept)
        {
            try
            {
                Provider provider = db.Providers.Find(id);
                provider.Admin_Accept = isaccept;
                db.SaveChanges();
                return Json(id);
            }
            catch
            {
                return Json(0);
            }
        }



        //Money Data & Statices _____________________________________________________________________
        [HttpPost]
        public JsonResult ProviderData(int id, DateTime FromDate, DateTime ToDate)
        {
            ToDate = ToDate.AddDays(1);
            Provider provider = db.Providers.Find(id);
            Datum datum = db.Data.Find(1);
            var Orders = provider.Products.SelectMany(x => x.Orders.Where(y => y.IsOrderd && y.Date.CompareTo(FromDate) > 0 && y.ISArrivedToUser&&ToDate.CompareTo(y.Date) > 0).Select(y => new
            {
                y.Price,
                y.IsUseVisa
            }));
            if (Orders.Count() > 0)
            {
                var Data = new
                {
                    Count = Orders.Where(O => !O.IsUseVisa).Count(),
                    Total = Orders.Where(O => !O.IsUseVisa && O.Price.HasValue).Sum(O => O.Price).Value,
                    TotalF = DEL.GetMoneyAfterTax(Orders.Where(O => !O.IsUseVisa && O.Price.HasValue).Sum(O => O.Price).Value, datum.Tax),

                    CountV = Orders.Where(O => O.IsUseVisa).Count(),
                    TotalV = Orders.Where(O => O.IsUseVisa && O.Price.HasValue).Sum(O => O.Price).Value,
                    TotalVF = DEL.GetMoneyAfterTax(Orders.Where(O => O.IsUseVisa && O.Price.HasValue).Sum(O => O.Price).Value, datum.Tax),
                };
                return Json(Data);
            }
            else
            {
                var data = new
                {
                    Count = 0,
                    Total = 0,
                    TotalF = 0,
                    CountV = 0,
                    TotalV = 0,
                    TotalVF = 0
                };
                return Json(data);
            }
        }
        [HttpPost]
        public ActionResult Data(DateTime FromDate,DateTime ToDate)
        {
            ToDate = ToDate.AddDays(1);
            var ProvidersData = db.Providers.Select(N => new
            {
                N.ID,
                N.TradeName,
                orders = N.Products.SelectMany(x => x.Orders.Where(y => y.IsOrderd&&y.Date.CompareTo(FromDate)>0&&y.ISArrivedToUser&&ToDate.CompareTo(y.Date) > 0).Select(y => new
                {
                   y.Price,
                   y.IsUseVisa
                }))
            });
            Datum datum = db.Data.Find(1);
            var Data = ProvidersData.Where(x => x.orders.Count() > 0).AsEnumerable().Select(x => new Data
            {
                ID=x.ID,
                TradeName= x.TradeName,
                OrdersCount=x.orders.Where(O=>!O.IsUseVisa).Count(),
                OrdersPrice = x.orders.Where(O => !O.IsUseVisa&&O.Price.HasValue).Sum(O=>O.Price).Value,
                OrdersPriceT =DEL.GetMoneyAfterTax(x.orders.Where(O => !O.IsUseVisa && O.Price.HasValue).Sum(O => O.Price).Value,datum.Tax),

                OrdersCountV = x.orders.Where(O => O.IsUseVisa).Count(),
                OrdersPriceV = x.orders.Where(O => O.IsUseVisa && O.Price.HasValue).Sum(O => O.Price).Value,
                OrdersPriceTV = DEL.GetMoneyAfterTax(x.orders.Where(O => O.IsUseVisa && O.Price.HasValue).Sum(O => O.Price).Value,datum.Tax),
            });
            ViewBag.Head = " بيانات التاريخ لمقدمي الخدمات " + "من بداية يوم " + FromDate.ToString("d") + "الي نهاية يوم " + ToDate.AddDays(-1).ToString("d");
            return View(Data);
        }

        public ActionResult VisaProviders()
        {
            var data = db.Providers.Where(x => x.AllowVisa).ToList();
            ViewBag.Head = "مقدمين الخدمات المستخدمين للدفع الالكتروني";
            return View("Providers", data);
        }
        public ActionResult KnownNumberProviders()
        {
            var data = db.Providers.Where(x => !string.IsNullOrEmpty(x.KnownNumber)).ToList();
            ViewBag.Head = "مقدمين الخدمات المستخدمين  للرقم المعروف";
            return View("Providers", data);
        }
        public ActionResult NotWorkProviders()
        {
            var data = db.Providers.Where(x=>x.Products.SelectMany(y => y.Orders).Count()<1).ToList();
            ViewBag.Head = "مقدمين الخدمات  ليس لديهم اي طلبات";
            return View("Providers", data);
        }
        public ActionResult ExcelentProviders()
        {
            var data = db.Providers.Where(x => x.IsExcelent).ToList();
            ViewBag.Head = "مقدمين الخدمات المتميزين";
            return View("Providers", data);
        }
        public ActionResult NotVisibleProviders()
        {
            var data = db.Providers.Where(x => !x.IsAvaiable).ToList();
            ViewBag.Head = "مقدمين الخدمات الغير متاحين";
            return View("Providers", data);
        }
        public ActionResult NotGoodProviders()
        {
            var data = db.Providers.Where(x => x.Provider_Rates.Where(r => r.Rate.HasValue).Average(r => r.Rate) < 2.5).ToList();
            ViewBag.Head = "مقدمين الخدمات تقيمهم اقل من 2.5";
            return View("Providers", data);
        }       
        public ActionResult NotAcceptProviders()
        {
            var data = db.Providers.Where(x => !x.Admin_Accept).ToList();
            ViewBag.Head = "مقدمين الخدمات الغير مفعلين من قبل الادمن";
            return View("Providers", data);
        }
        public ActionResult TopOrders()
        {
            var data = db.Providers.OrderByDescending(x=>x.Products.Select(o=>o.Orders.Count)).Take(20).ToList();
            ViewBag.Head = "اعلي 20 مقدم في عدد الطلبات";
            return View("Providers", data);
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

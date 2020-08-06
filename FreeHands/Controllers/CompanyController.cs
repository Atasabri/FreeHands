using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FreeHands.Models;
using System.IO;

namespace FreeHands.Controllers
{
    public class CompanyController : Controller
    {
        DB db = new DB();
        
        
        // Authentication _________________________________________________________________________
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string Name,string Pass)
        {
            Company company = db.Companies.SingleOrDefault(x => x.UserName == Name && x.Password == Pass);
            if(company!=null)
            {
                Session["Login"] = company.ID;
                return RedirectToAction("Dashboard");
            }
            ViewBag.error = "Invalid Login !!!";
            return View();
        }

        public ActionResult LogOut()
        {
            Session.Remove("Login");
            return RedirectToAction("Login");
        }



        //Drivers __________________________________________________________________________________
        public ActionResult Drivers()
        {
            if(Session["Login"]!=null)
            {
                int id = int.Parse(Session["Login"].ToString());
                return View(db.Drivers.Where(x => x.Company_ID == id).ToList());
            }
            return RedirectToAction("Login");
        }
        [HttpPost]
        public ActionResult CreateDriver(Driver driver, HttpPostedFileBase Photo, HttpPostedFileBase Driver_Car_License, HttpPostedFileBase Driver_License)
        {
            try
            {
                if (Session["Login"] != null)
                {
                    if (Photo == null || Driver_License == null || Driver_Car_License == null)
                    {
                        return RedirectToAction("Drivers");
                    }
                    int Company_ID = int.Parse(Session["Login"].ToString());
                    driver.Company_ID = Company_ID;
                    driver.Token = DEL.encrypt(driver.Email);
                    driver.Visible = true;
                    db.Drivers.Add(driver);
                    db.SaveChanges();
                    Photo.SaveAs(Server.MapPath("~/Uploads/Driver_Photo/" + driver.ID + ".jpg"));
                    Driver_License.SaveAs(Server.MapPath("~/Uploads/Driver_License/" + driver.ID + ".jpg"));
                    Driver_Car_License.SaveAs(Server.MapPath("~/Uploads/Driver_Car_License/" + driver.ID + ".jpg"));
                    return RedirectToAction("Drivers");
                }
            }
            catch
            {
                return RedirectToAction("Drivers");
            }
            return RedirectToAction("Login");
       }
        public ActionResult Driver(int id)
        {
            if (Session["Login"] != null)
            {
                int Company_ID = int.Parse(Session["Login"].ToString());
                var driver = db.Drivers.Where(x=>x.ID==id).Select(x=>new {
                    x.ID
                    ,x.Token
                    ,x.FirstName
                    ,x.LastName
                    ,x.Phone
                    ,x.Password
                    ,x.Email
                    ,x.Location
                    ,x.Company_ID
                    ,x.Visible
                    ,x.Country_ID,
                    x.City_ID
                }).FirstOrDefault();
                if (driver.Company_ID == Company_ID)
                {
                    return Json(driver);
                }
            }
            return RedirectToAction("Login");
        }
        public ActionResult DriverEdit(Driver driver, HttpPostedFileBase Photo, HttpPostedFileBase Driver_Car_License, HttpPostedFileBase Driver_License)
        {
            if (Session["Login"] != null)
            {
                int Company_ID = int.Parse(Session["Login"].ToString());
                driver.Company_ID = Company_ID;
                db.Entry(driver).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                if(Photo!=null)
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
                return RedirectToAction("Drivers");
            }
            return RedirectToAction("Login");
        }
        [HttpPost]
        public ActionResult DeleteDriver(int id)
        {
            if (Session["Login"] != null)
            {
                Driver driver = db.Drivers.Find(id);
                db.Drivers.Remove(driver);
                db.SaveChanges();
                FileInfo F = new FileInfo(Server.MapPath("~/Uploads/Driver_Photo/" + id + ".jpg"));
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
            return RedirectToAction("Login");
        }

        [HttpPost]
        public JsonResult ChangeDriverVisible(int id, bool visible)
        {
            if (Session["Login"] != null)
            {
                try
                {
                    Driver driver = db.Drivers.Find(id);
                    int company_ID = int.Parse(Session["Login"].ToString());
                    if (driver.Company_ID != company_ID)
                    {
                        return Json(id);
                    }
                    driver.Visible = visible;
                    db.Entry(driver).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(id);
                }
                catch
                {
                    return Json(0);
                }
            }
            else
            {
                return Json(0);
            }
        }

        public JsonResult CheckEmail(string email)
        {
            if (db.Drivers.Any(x => x.Email == email))
            {
                return Json(0);
            }
            return Json(1);
        }
        public JsonResult CheckEmailEdit(string email, int id)
        {
            if (db.Drivers.Where(x => x.ID != id).Any(x => x.Email == email))
            {
                return Json(0);
            }
            return Json(1);
        }
      
        
        
        //Company Money Data _______________________________________________________________________
        public ActionResult Data()
        {
            if (Session["Login"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        [HttpPost]
        public ActionResult GetCompanyData(DateTime FromDate, DateTime ToDate)
        {
            if (Session["Login"] != null)
            {
                ToDate = ToDate.AddDays(1);
                int company_ID = int.Parse(Session["Login"].ToString());
                Company company = db.Companies.Find(company_ID);
                var Orders = company.Drivers.SelectMany(x => x.Orders.Where(y => y.IsOrderd && y.Date.CompareTo(FromDate) > 0 && ToDate.CompareTo(y.Date) > 0 && y.ISArrivedToUser).AsEnumerable().Select(y => new
                {
                    y.Total,
                    y.Delivery,
                    y.IsUseVisa
                }));
                if (Orders.Count() > 0)
                {
                    var Data = new
                    {
                        Count = Orders.Where(O => !O.IsUseVisa).Count(),
                        Total = Orders.Where(O => !O.IsUseVisa && O.Total.HasValue).Sum(O => O.Total).Value,
                        TotalF = Orders.Where(O => !O.IsUseVisa && O.Delivery.HasValue).Sum(O => O.Delivery).Value,

                        CountV = Orders.Where(O => O.IsUseVisa).Count(),
                        TotalV = Orders.Where(O => O.IsUseVisa && O.Total.HasValue).Sum(O => O.Total).Value,
                        TotalVF = Orders.Where(O => O.IsUseVisa && O.Delivery.HasValue).Sum(O => O.Delivery).Value,
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
            else
            {
                return Json(0);
            }
        }





        //Dashboard & Orders _________________________________________________________________________
        public ActionResult Dashboard()
        {
            if(Session["Login"]!=null)
            {
                int id = int.Parse(Session["Login"].ToString());
                return View(db.Companies.Find(id));
            }
            return RedirectToAction("Login");
        }

        public ActionResult Orders()
        {
            if (Session["Login"] != null)
            {
                int company_ID = int.Parse(Session["Login"].ToString());
                return View(db.Orders.Where(x=>x.Driver_ID.HasValue).Where(x=>x.Driver.Company_ID==company_ID).OrderByDescending(x=>x.Date).ToList());
            }
            return RedirectToAction("Login");
        }
        [HttpPost]
        public ActionResult GetOrders(DateTime FromDate,DateTime ToDate)
        {
            if (Session["Login"] != null)
            {
                int company_ID = int.Parse(Session["Login"].ToString());
                return View("Orders",db.Orders.Where(x => x.Driver_ID.HasValue).Where(x => x.Driver.Company_ID == company_ID&&x.Date.CompareTo(FromDate)>0&&ToDate.CompareTo(x.Date)>0).OrderByDescending(x => x.Date).ToList());
            }
            return RedirectToAction("Login");
        }
        public ActionResult Order(int id)
        {
            if (Session["Login"] != null)
            {
                var order = db.Orders.Where(x => x.ID == id).AsEnumerable().Select(x => new
                {
                    x.ID,
                    Date=x.Date.ToString("MM/dd/yyyy"),
                    x.Driver.FirstName,
                    x.Driver.LastName,
                    x.Accepted,
                    x.ISArrivedToUser,
                    x.Number,
                    x.User.Phone,
                    x.Product.Name,
                    x.Total,
                    x.Delivery
                }).FirstOrDefault();
                return Json(order);
            }
            return RedirectToAction("Login");
        }

        public ActionResult GetCities(int CountryID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<City> List = db.Cities.Where(x => x.Country_ID == CountryID).ToList();
            return Json(List, JsonRequestBehavior.AllowGet);
        }
    }
}
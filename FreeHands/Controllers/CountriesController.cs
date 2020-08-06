using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FreeHands.Models;

namespace FreeHands.Controllers
{
    [Authorize]
    public class CountriesController : Controller
    {
        DB db = new DB();
        // GET: Countries
        public ActionResult Index()
        {
            return View(db.Countries.OrderByDescending(x=>x.Users.Count).ToList());
        }
        public ActionResult Cities(int? id)
        {
            if(id==null)
            {
                return RedirectToAction("Index");
            }
            return View(db.Cities.Where(x=>x.Country_ID==id).OrderByDescending(x=>x.Users.Count).ToList());
        }
        public ActionResult Country(int ID)
        {
            Country country = db.Countries.Find(ID);
            if(country==null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Head = "المستخدمين الموجودين في دولة"+"  "+country.Name;
            return View(db.Users.Where(x => x.Country_ID == ID).ToList());
        }
    }
}
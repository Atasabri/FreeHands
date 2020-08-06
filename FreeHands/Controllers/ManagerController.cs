using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.Globalization;
using System.Threading;
using System.IO;
using System.Device.Location;
using FreeHands.Models;

#region Author
      /*
       *[Auth : Ata Sabri Ata Ahmed
       * Version : V_001
       * Info : Api Controller For Manage All Requests & Responses From FreeHands APPS 
       * E-Mail : ata.sabry@rooyadev.com]
       */
#endregion

namespace FreeHands.Controllers
{
    public class ManagerController : ApiController
    {
        // DataBase Context 
        DB db = new DB();

        //______________________________________[Providers]__________________________________________________

        [HttpPost]
        public IHttpActionResult Provider_Register()
        {
            try
            {
                #region Params
                string FirstName = HttpContext.Current.Request.Form["FirstName"];
                string LastName = HttpContext.Current.Request.Form["LastName"];
                string TradeName = HttpContext.Current.Request.Form["TradeName"];
                string Email = HttpContext.Current.Request.Form["Email"];
                string Password = HttpContext.Current.Request.Form["Password"];
                double Lat = Convert.ToDouble(HttpContext.Current.Request.Form["Lat"]);
                double Log = Convert.ToDouble(HttpContext.Current.Request.Form["Log"]);
                string Location = HttpContext.Current.Request.Form["Location"];
                string Phone = HttpContext.Current.Request.Form["Phone"];
                string KnownNumber = HttpContext.Current.Request.Form["KnownNumber"];
                string Country = HttpContext.Current.Request.Form["Country"];
                string City = HttpContext.Current.Request.Form["City"];
                string Currency = HttpContext.Current.Request.Form["Currency"];

                string Provider_Photo = HttpContext.Current.Request.Form["Provider_Photo"];
                string Provider_Identity = HttpContext.Current.Request.Form["Provider_Identity"];
                #endregion

                if (db.Providers.Any(x => x.TradeName == TradeName))
                {
                    return BadRequest("هذا الاسم موجود بالفعل برجاء ادخال اسم جديد !!");
                }

                DEL.Location location = DEL.CountryCityID(Country, City);
                Provider provider = new Provider
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    TradeName = TradeName,
                    Email = Email,
                    Password = Password,
                    Lat = Lat,
                    Log = Log,
                    Location = Location,
                    Phone = Phone,
                    IsAvaiable = true,
                    Currency=Currency,
                    Token= DEL.encrypt(TradeName),
                    Country_ID= location.Country_ID,
                    City_ID=location.City_ID
                };
                db.Providers.Add(provider);
                db.SaveChanges();

                byte[] PhotoBase64 = Convert.FromBase64String(Provider_Photo);
                File.WriteAllBytes(HttpContext.Current.Server.MapPath("~/Uploads/Provider_Photo/" + provider.ID + ".jpg"), PhotoBase64);
                byte[] IdentityPhotoBase64 = Convert.FromBase64String(Provider_Identity);
                File.WriteAllBytes(HttpContext.Current.Server.MapPath("~/Uploads/Provider_Identity/" + provider.ID + ".jpg"), IdentityPhotoBase64);

                var Data = new
                {
                    provider.ID,
                    provider.Token,
                    Photo = DEL.Domain + "/Uploads/Provider_Photo/" + provider.ID + ".jpg",
                    IdentityPhoto = DEL.Domain + "/Uploads/Provider_Identity/" + provider.ID + ".jpg",
                    provider.FirstName,
                    provider.LastName,
                    provider.TradeName,
                    provider.Email,
                    provider.Password,
                    provider.Lat,
                    provider.Log,
                    provider.Location,
                    provider.Phone,
                    provider.IsAvaiable,
                    provider.KnownNumber
                };
                return Ok(Data);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public IHttpActionResult Provider_Login()
        {
            try
            {
                string TradeName = HttpContext.Current.Request.Form["TradeName"];
                string Password = HttpContext.Current.Request.Form["Password"];

                var provider = db.Providers.Where(x => x.TradeName == TradeName && x.Password == Password);

                var Data = provider.Select(x => new
                {
                    x.ID,
                    x.Token,
                    Photo = DEL.Domain + "/Uploads/Provider_Photo/" + x.ID + ".jpg",
                    IdentityPhoto = DEL.Domain + "/Uploads/Provider_Identity/" + x.ID + ".jpg",
                    x.FirstName,
                    x.LastName,
                    x.TradeName,
                    x.Email,
                    x.Currency,
                    x.Password,
                    x.Lat,
                    x.Log,
                    x.Location,
                    x.Phone,
                    x.IsAvaiable,
                    x.KnownNumber,
                    x.IsExcelent,
                    x.AllowVisa,
                    x.Discount,
                    x.AccountNumber,
                    x.IPan,
                    x.SwiftCode,
                    x.BankName,
                    Rate = x.Provider_Rates.Where(r => r.Rate.HasValue).Count() > 0 ? x.Provider_Rates.Where(r => r.Rate.HasValue).Select(r => r.Rate.Value).Average() : 0,
                }).FirstOrDefault();
                if (Data != null)
                {
                    return Ok(Data);
                }
                if(db.Providers.Any(x=>x.TradeName==TradeName))
                {
                    return BadRequest("برجاء التأكد من كلمة المرور !!");
                }
                return BadRequest("هذا المستخدم غير موجود .. برجاء انشاء حساب");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public IHttpActionResult Provider_Products()
        {
            string Token = HttpContext.Current.Request.Form["Token"];
            var data = db.Products.Where(x => x.Provider.Token == Token).OrderByDescending(x=>x.ID).AsEnumerable().Select(x => new
            {
                x.ID,
                x.Name,
                x.Price,
                OptionsCount=x.Options.Count,
                x.Description,
                x.Provider.Currency,
                x.Visible,
                TimeToFinishH=TimeSpan.FromMinutes(x.TimeToFinish).ToString("hh"),
                TimeToFinishM = TimeSpan.FromMinutes(x.TimeToFinish).ToString("mm"),
                CatName = x.Category.Name,
                Photo = DEL.Domain + "/Uploads/Products/" + x.ID + ".jpg"
            });
            return Ok(data);
        }
        [HttpGet]
        public IHttpActionResult Provider_Product_Options(int Product_ID)
        {
            var data = db.Options.Where(x =>x.Product_ID==Product_ID).OrderByDescending(x=>x.ID).Select(x => new
            {
                x.ID,
                x.OptionName,
                x.Price,
                x.Product.Provider.Currency,
                x.Description
            });
            return Ok(data);
        }
        [HttpGet]
        public IHttpActionResult AllCategories()
        {
            var Data = db.Categories.Select(x => new { x.ID, x.Name });
            return Ok(Data);
        }
        [HttpPut]
        public IHttpActionResult EditProvider()
        {
            try
            {
                #region Params
                string Token = HttpContext.Current.Request.Form["Token"];
                string FirstName = HttpContext.Current.Request.Form["FirstName"];
                string LastName = HttpContext.Current.Request.Form["LastName"];
                string TradeName = HttpContext.Current.Request.Form["TradeName"];
                string Email = HttpContext.Current.Request.Form["Email"];
                string Password = HttpContext.Current.Request.Form["Password"];
                string Lat = HttpContext.Current.Request.Form["Lat"];
                string Log = HttpContext.Current.Request.Form["Log"];
                string Location = HttpContext.Current.Request.Form["Location"];
                string Phone = HttpContext.Current.Request.Form["Phone"];
                string KnownNumber = HttpContext.Current.Request.Form["KnownNumber"];
                string IsAvaiable = HttpContext.Current.Request.Form["IsAvaiable"];
                string Discount = HttpContext.Current.Request.Form["Discount"];
                string AllowVisa = HttpContext.Current.Request.Form["AllowVisa"];

                string Provider_Photo = HttpContext.Current.Request.Form["Provider_Photo"];
                string Provider_Identity = HttpContext.Current.Request.Form["Provider_Identity"];

                //Visa
                string AccountNumber = HttpContext.Current.Request.Form["AccountNumber"];
                string BankName = HttpContext.Current.Request.Form["BankName"];
                string IPan = HttpContext.Current.Request.Form["IPan"];
                string SwiftCode = HttpContext.Current.Request.Form["SwiftCode"];
                #endregion
                Provider provider = db.Providers.SingleOrDefault(x => x.Token == Token);
                if (provider != null)
                {
                    if (!string.IsNullOrEmpty(FirstName))
                        provider.FirstName = FirstName;
                    if (!string.IsNullOrEmpty(LastName))
                        provider.LastName = LastName;
                    if (!string.IsNullOrEmpty(TradeName))
                    {
                        if (db.Providers.Where(x => x.ID != provider.ID).Any(x => x.TradeName == TradeName))
                        {
                            return BadRequest("هذا الاسم مستخدم بالفعل برجاء ادخال اسم مختلف !!");
                        }
                        provider.TradeName = TradeName;
                        provider.Token = DEL.encrypt(TradeName);
                    }
                    if (!string.IsNullOrEmpty(Email))
                        provider.Email = Email;
                    if (!string.IsNullOrEmpty(Password))
                        provider.Password = Password;

                    if (!string.IsNullOrEmpty(Lat))
                        provider.Lat = Convert.ToDouble(Lat);
                    if (!string.IsNullOrEmpty(Log))
                        provider.Log = Convert.ToDouble(Log);
                    if (!string.IsNullOrEmpty(Location))
                        provider.Location = Location;
                    if (!string.IsNullOrEmpty(Phone))
                        provider.Phone = Phone;
                    if (!string.IsNullOrEmpty(KnownNumber))
                        provider.KnownNumber = KnownNumber;
                    if (!string.IsNullOrEmpty(IsAvaiable))
                        provider.IsAvaiable = Convert.ToBoolean(IsAvaiable);
                    if (Discount=="")
                    {
                        provider.Discount = 0;
                    }
                    else
                    {
                        provider.Discount = Convert.ToDouble(Discount);
                    }
                    provider.AccountNumber=AccountNumber;
                    provider.BankName=BankName;
                    provider.IPan=IPan;
                    provider.SwiftCode=SwiftCode;
                    if (!string.IsNullOrEmpty(AllowVisa))
                        provider.AllowVisa = Convert.ToBoolean(AllowVisa);

                    if (!string.IsNullOrEmpty(Provider_Photo))
                    {
                        byte[] Photo = Convert.FromBase64String(Provider_Photo);
                        File.WriteAllBytes(HttpContext.Current.Server.MapPath("~/Uploads/Provider_Photo/" + provider.ID + ".jpg"), Photo);
                    }
                    if (!string.IsNullOrEmpty(Provider_Identity))
                    {
                        byte[] Photo = Convert.FromBase64String(Provider_Identity);
                        File.WriteAllBytes(HttpContext.Current.Server.MapPath("~/Uploads/Provider_Identity/" + provider.ID + ".jpg"), Photo);
                    }
                    db.Entry(provider).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Ok(new { Message = "تم تعديل بيانات مقدم الخدمة ." });
                }
                return BadRequest("Provider Not Found !");
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public IHttpActionResult AddProduct()
        {
            try
            {
                #region Params
                string Token = HttpContext.Current.Request.Form["Token"];
                int Cat_ID = int.Parse(HttpContext.Current.Request.Form["Cat_ID"]);
                string Name = HttpContext.Current.Request.Form["Name"];
                string Description = HttpContext.Current.Request.Form["Description"];
                double Price = Convert.ToDouble(HttpContext.Current.Request.Form["Price"]);
                bool Visible = Convert.ToBoolean(HttpContext.Current.Request.Form["Visible"]);
                int Time = int.Parse(HttpContext.Current.Request.Form["Time"]);
                string Photo = HttpContext.Current.Request.Form["Photo"];


                #endregion
                if (string.IsNullOrEmpty(Photo))
                {
                    return BadRequest("من فضلك قم باختيار صورة !!");
                }
                Provider provider = db.Providers.SingleOrDefault(x => x.Token == Token);
                Product product = new Product
                {
                    Name = Name,
                    Description = Description,
                    Cat_ID = Cat_ID,
                    Price = Price,
                    Provider_ID = provider.ID ,
                    TimeToFinish=Time,
                    Visible=Visible                  
                };  
                db.Products.Add(product);
                db.SaveChanges();
                byte[] Base64Photo = Convert.FromBase64String(Photo);
                File.WriteAllBytes(HttpContext.Current.Server.MapPath("~/Uploads/Products/" + product.ID + ".jpg"), Base64Photo);
                return Ok(new {
                    Message = "تم اضافة الصورة بنجاح .",
                    product.ID,product.Name,product.Price,
                    Options = product.Options.Select(x=>new {x.ID,x.OptionName,x.Price,x.Description })
                });
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        public IHttpActionResult EditProduct()
        {
            try
            {
                #region Params
                string Token = HttpContext.Current.Request.Form["Token"];
                int ID = int.Parse(HttpContext.Current.Request.Form["ID"]);
                string Cat_ID = HttpContext.Current.Request.Form["Cat_ID"];
                string Name = HttpContext.Current.Request.Form["Name"];
                string Description = HttpContext.Current.Request.Form["Description"];
                string Price = HttpContext.Current.Request.Form["Price"];
                string Visible = HttpContext.Current.Request.Form["Visible"];
                string Time = HttpContext.Current.Request.Form["Time"];
                string Photo = HttpContext.Current.Request.Form["Photo"];
                #endregion

                Product product = db.Products.SingleOrDefault(x => x.ID == ID && x.Provider.Token == Token);
                if (!string.IsNullOrEmpty(Cat_ID))
                    product.Cat_ID = int.Parse(Cat_ID);
                if (!string.IsNullOrEmpty(Name))
                    product.Name = Name;
                if (!string.IsNullOrEmpty(Description))
                    product.Description = Description;
                if (!string.IsNullOrEmpty(Price))
                    product.Price = Convert.ToDouble(Price);
                if (!string.IsNullOrEmpty(Visible))
                    product.Visible = Convert.ToBoolean(Visible);
                if (!string.IsNullOrEmpty(Time))
                    product.TimeToFinish = int.Parse(Time);


                db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                if (!string.IsNullOrEmpty(Photo))
                {
                    byte[] Base64Photo = Convert.FromBase64String(Photo);
                    File.WriteAllBytes(HttpContext.Current.Server.MapPath("~/Uploads/Products/" + product.ID + ".jpg"), Base64Photo);
                }
                return Ok(new { Message = "تم تعديل المنتج بنجاح ." });
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        public IHttpActionResult EditProductVisiblity(int Product_ID)
        {
            try
            {
                string Token = HttpContext.Current.Request.Form["Token"];
                bool Visible = Convert.ToBoolean(HttpContext.Current.Request.Form["Visible"]);

                Product product = db.Products.SingleOrDefault(x => x.ID == Product_ID && x.Provider.Token == Token);
                product.Visible = Visible;
                db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Ok(new { Message = "تم تعديل ظهور المنتج للعملاء ." });
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public IHttpActionResult DeleteProduct()
        {
            try
            {
                string Token = HttpContext.Current.Request.Form["Token"];
                int ID = int.Parse(HttpContext.Current.Request.Form["ID"]);
                Product product = db.Products.SingleOrDefault(x => x.ID == ID && x.Provider.Token == Token);
                db.Products.Remove(product);
                db.SaveChanges();
                return Ok(new { Message = "تم مسح المنتج بنجاح ." });
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public IHttpActionResult AddOption()
        {
            try
            {
                int Product_ID = int.Parse(HttpContext.Current.Request.Form["Product_ID"]);
                string Name = HttpContext.Current.Request.Form["Name"];
                double Price = Convert.ToDouble(HttpContext.Current.Request.Form["Price"]);
                string Description = HttpContext.Current.Request.Form["Description"];
                Option option = new Option
                {
                    OptionName = Name,
                    Product_ID = Product_ID,
                    Price = Price,
                    Description=Description
                };
                db.Options.Add(option);
                db.SaveChanges();
                return Ok(new { Message = "تم اضافة الاضافة بنجاح ." });
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        public IHttpActionResult EditOption()
        {
            try
            {
                string Token= HttpContext.Current.Request.Form["Token"];
                int ID = int.Parse(HttpContext.Current.Request.Form["ID"]);
                string Name = HttpContext.Current.Request.Form["Name"];
                string Price = HttpContext.Current.Request.Form["Price"];
                string Description = HttpContext.Current.Request.Form["Description"];

                Option option = db.Options.Where(x=>x.Product.Provider.Token==Token&&x.ID==ID).FirstOrDefault();
                if (!string.IsNullOrEmpty(Name))
                    option.OptionName = Name;
                if (!string.IsNullOrEmpty(Price))
                    option.Price = Convert.ToDouble(Price);
                if(Description!=null)
                    option.Description = Description;

                db.Entry(option).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Ok(new { Message = "تم تعديل الاضافة بنجاح" });
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public IHttpActionResult DeleteOption()
        {
            try
            {
                string Token = HttpContext.Current.Request.Form["Token"];
                int ID = int.Parse(HttpContext.Current.Request.Form["ID"]);
                Option option = db.Options.Where(x => x.Product.Provider.Token == Token && x.ID == ID).FirstOrDefault();
                db.Options.Remove(option);
                db.SaveChanges();
                return Ok(new { Message = "تم حذف الاضافة بنجاح ." });
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Provider_Offers()
        {
            string Token = HttpContext.Current.Request.Form["Token"];
            var Data = db.Offers.Where(x => x.Provider.Token == Token).OrderByDescending(x=>x.ID).Select(x => new
            {
                x.ID,
                x.Name,
                x.Price,
                x.Product_ID,
                x.Provider.Currency,
                x.Description,
                ProductName = x.Product.Name,
                Photo = DEL.Domain + "/Uploads/Products/" + x.Product_ID + ".jpg",
            });
            return Ok(Data);
        }
        [HttpPost]
        [CacheFilter(TimeDuration = 10000)]
        public IHttpActionResult Provider_Orders()
        {
            string Token = HttpContext.Current.Request.Form["Token"];
            Provider provider = db.Providers.SingleOrDefault(x => x.Token == Token);
            var orders = provider.Products.SelectMany(x => x.Orders.Where(y => y.IsOrderd).AsEnumerable().OrderByDescending(y => y.Date).Select(y => new
            {
                y.ID,
                Date=y.Date.ToString("d"),
                Time = y.Date.ToString("T"),
                y.Number,
                y.IsUseVisa,
                y.Status,
                y.Ready,
                DateNeeded=y.DateTimeNeeded.HasValue? y.DateTimeNeeded.Value.ToString("d"):null,
                TimeNeeded = y.DateTimeNeeded.HasValue ? y.DateTimeNeeded.Value.ToString("T") : null,
                y.ProblemInArriveToUser,
                y.Reason,
                y.Discount,
                y.ISArrivedToUser,
                DriverFirstName=y.Driver_ID.HasValue?y.Driver.FirstName:null,
                DriverLastName = y.Driver_ID.HasValue ? y.Driver.LastName:null,
                DriverPhone = y.Driver_ID.HasValue ? y.Driver.Phone:null,
                DriverName = y.Driver_ID.HasValue ? y.Driver.FirstName : null,
                ProductName = y.Product.Name,
                Offer = y.Offer_ID.HasValue ? y.Offer.Name : null,
                y.Price,
                x.Provider.Currency
            }));
            return Ok(orders);
        }
        [HttpPost]
        public IHttpActionResult AddOffer(int Product_ID)
        {
            try
            {
                string Token = HttpContext.Current.Request.Form["Token"];
                string Name = HttpContext.Current.Request.Form["Name"];
                string Description = HttpContext.Current.Request.Form["Description"];
                double Price = Convert.ToDouble(HttpContext.Current.Request.Form["Price"]);
                bool Active = Convert.ToBoolean(HttpContext.Current.Request.Form["Active"]);

                Provider provider = db.Providers.SingleOrDefault(x => x.Token == Token);
                if (!provider.Products.Any(x=>x.ID==Product_ID))
                {
                    return BadRequest("هذا المنتج لا ينتمي لمقدم الخدمة!!");
                }
                Offer offer = new Offer
                {
                    Active = Active,
                    Product_ID = Product_ID,
                    Provider_ID = provider.ID,
                    Price = Price,
                    Name = Name,
                    Description=Description
                };
                db.Offers.Add(offer);
                db.SaveChanges();
                return Ok(new { Message = "تم اضافة العرض بنجاح .", Offer_ID = offer.ID });
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        public IHttpActionResult EditOffer(int Offer_ID)
        {
            try
            {
                string Token = HttpContext.Current.Request.Form["Token"];
                string Name = HttpContext.Current.Request.Form["Name"];
                string Description = HttpContext.Current.Request.Form["Description"];
                string Price = HttpContext.Current.Request.Form["Price"];
                string Active = HttpContext.Current.Request.Form["Active"];

                Offer offer = db.Offers.SingleOrDefault(x => x.Provider.Token == Token&&x.ID==Offer_ID);
                if (!string.IsNullOrEmpty(Name))
                    offer.Name = Name;
                if (!string.IsNullOrEmpty(Price))
                    offer.Price =Convert.ToDouble(Price);
                if (!string.IsNullOrEmpty(Active))
                    offer.Active = Convert.ToBoolean(Active);
                if (!string.IsNullOrEmpty(Description))
                    offer.Description = Description;



                db.Entry(offer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Ok(new { Message = "تم تعديل العرض بنجاح ."});
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public IHttpActionResult DeleteOffer(int Offer_ID)
        {
            try
            {
                string Token = HttpContext.Current.Request.Form["Token"];
                Offer offer = db.Offers.Where(x => x.Provider.Token == Token && x.ID == Offer_ID).FirstOrDefault();                
                db.Offers.Remove(offer);
                db.SaveChanges();
                return Ok(new { Message = "تم حذف العرض بنجاح ." });
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public IHttpActionResult AcceptOrder(int Order_ID)
        {
            try
            {
                string Token = HttpContext.Current.Request.Form["Token"];
                Order order = db.Orders.SingleOrDefault(x => x.ID == Order_ID && x.Product.Provider.Token == Token);
                if (order.IsOrderd)
                {
                    order.Accepted = true;
                    order.Status = "تم التأكيد";
                    db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Ok(new { Message = "تم الموافقة علي هذا الطلب ." });
                }
                else
                {
                    return BadRequest("الطلب لم يتم طلبه !!");
                }
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public IHttpActionResult ChangeStatus(int Order_ID)
        {
            try
            {
                string Token = HttpContext.Current.Request.Form["Token"];
                string Status = HttpContext.Current.Request.Form["Status"];
                Order order = db.Orders.SingleOrDefault(x => x.Product.Provider.Token == Token && x.ID == Order_ID);
                if(!order.Accepted)
                {
                    return BadRequest("الطلب لم يتم طلبه ما ذال تحت الطلب !!");
                }
                order.Status = Status;
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Ok(new { Message = "تم تغيير الحالة بنجاح ." });
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public IHttpActionResult IsReady(int Order_ID)
        {
            try
            {
                string Token = HttpContext.Current.Request.Form["Token"];
                Order order = db.Orders.SingleOrDefault(x => x.Product.Provider.Token == Token && x.ID == Order_ID);
                if (!order.Accepted)
                {
                    return BadRequest("من فضلم قم بالموافقة علي الطلب اولا !!");
                }
                order.Ready = true;
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Ok(new { Message = "تم جعل الطلب جاهز ." });
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public IHttpActionResult CheckKnownNumber()
        {
            Datum data = db.Data.Find(1);
            return Ok(new { IsActive = data.AllowKnownNumber });
        }
        [HttpPost]
        public IHttpActionResult ForgetPassword_Provider()
        {
            string Name = HttpContext.Current.Request.Form["Name"];
            Provider provider = db.Providers.SingleOrDefault(x => x.TradeName == Name);
            if (provider == null)
            {
                return BadRequest("اسم المستخدم غير موجود !!");
            }
            try
            {
                DEL.Send_Password(provider.Password, provider.Email);
                return Ok(new { Message = "يمكنك تصفح البريد الالكتروني ." });
            }
            catch
            {
                return BadRequest("حدث خطأ اثناء ارسال رسالة البريد !!");
            }
        }






        //_____________________________________[Drivers]__________________________________________________


        [HttpPost]
        public IHttpActionResult Driver_Register()
        {
            try
            {
                #region Params
                string FirstName = HttpContext.Current.Request.Form["FirstName"];
                string LastName = HttpContext.Current.Request.Form["LastName"];
                string Email = HttpContext.Current.Request.Form["Email"];
                string Password = HttpContext.Current.Request.Form["Password"];
                string City = HttpContext.Current.Request.Form["City"];
                string Phone = HttpContext.Current.Request.Form["Phone"];
                string Company_ID =HttpContext.Current.Request.Form["Company_ID"];
                string Country = HttpContext.Current.Request.Form["Country"];
                string Location = HttpContext.Current.Request.Form["Location"];

                string Driver_Photo = HttpContext.Current.Request.Form["Driver_Photo"];
                string Driver_License = HttpContext.Current.Request.Form["Driver_License"];
                string Driver_Car_License = HttpContext.Current.Request.Form["Driver_Car_License"];
                #endregion
                if (db.Drivers.Any(x => x.Email == Email))
                {
                    return BadRequest("هذا البريد الالكتروني مستخدم بالفعل برجاء ادخال بريد اخر !! ");
                }
                DEL.Location locate = DEL.CountryCityID(Country, City);
                Driver driver = new Driver
                {
                    Email = Email,
                    FirstName = FirstName,
                    LastName = LastName,
                    Password = Password,
                    Location = Location,
                    Phone = Phone,
                    Token = DEL.encrypt(Email),
                    Visible=false,
                    Country_ID=locate.Country_ID,
                    City_ID=locate.City_ID
                };
                if(!string.IsNullOrEmpty(Company_ID)&&Company_ID!="0")
                {
                    driver.Company_ID = int.Parse(Company_ID);
                }
                db.Drivers.Add(driver);
                db.SaveChanges();
                byte[] PhotoBase64 = Convert.FromBase64String(Driver_Photo);
                File.WriteAllBytes(HttpContext.Current.Server.MapPath("~/Uploads/Driver_Photo/" + driver.ID + ".jpg"), PhotoBase64);
                byte[] Driver_LicenseBase64 = Convert.FromBase64String(Driver_License);
                File.WriteAllBytes(HttpContext.Current.Server.MapPath("~/Uploads/Driver_License/" + driver.ID + ".jpg"), Driver_LicenseBase64);
                byte[] Driverr_Car_LicenseBase64 = Convert.FromBase64String(Driver_Car_License);
                File.WriteAllBytes(HttpContext.Current.Server.MapPath("~/Uploads/Driver_Car_License/" + driver.ID + ".jpg"), Driverr_Car_LicenseBase64);
                var Data = new
                {
                    driver.ID,
                    driver.Token,
                    Photo = DEL.Domain + "/Uploads/Driver_Photo/" + driver.ID + ".jpg",
                    LicensePhoto = DEL.Domain + "/Uploads/Driver_License/" + driver.ID + ".jpg",
                    CarLicensePhoto = DEL.Domain + "/Uploads/Driver_Car_License/" + driver.ID + ".jpg",
                    driver.FirstName,
                    driver.LastName,
                    driver.Email,
                    driver.Password,
                    driver.Location,
                    driver.Phone,
                };
                return Ok(Data);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public IHttpActionResult Driver_Login()
        {
            try
            {
                string Email = HttpContext.Current.Request.Form["Email"];
                string Password = HttpContext.Current.Request.Form["Password"];

                var driver = db.Drivers.Where(x => x.Email == Email && x.Password == Password);

                var Data = driver.Select(x => new
                {
                    x.ID,
                    x.Token,
                    Photo = DEL.Domain + "/Uploads/Driver_Photo/" + x.ID + ".jpg",
                    License = DEL.Domain + "/Uploads/Driver_License/" + x.ID + ".jpg",
                    CarLicense = DEL.Domain + "/Uploads/Driver_Car_License/" + x.ID + ".jpg",
                    x.FirstName,
                    x.LastName,
                    x.Email,
                    x.Password,
                    x.Location,
                    x.Phone,
                    x.Visible
                }).FirstOrDefault();
                if (Data != null)
                {
                    return Ok(Data);
                }
                if (db.Drivers.Any(x => x.Email==Email))
                {
                    return BadRequest("برجاء التأكد من كلمة المرور !!");
                }
                return BadRequest("هذا البريد الالكتروني غير موجود .. برجاء انشاء حساب");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        public IHttpActionResult Edit_Driver()
        {
            try
            {
                #region Params
                string Token = HttpContext.Current.Request.Form["Token"];
                string FirstName = HttpContext.Current.Request.Form["FirstName"];
                string LastName = HttpContext.Current.Request.Form["LastName"];
                string Email = HttpContext.Current.Request.Form["Email"];
                string Password = HttpContext.Current.Request.Form["Password"];
                string Location = HttpContext.Current.Request.Form["Location"];
                string Phone = HttpContext.Current.Request.Form["Phone"];

                string Driver_Photo = HttpContext.Current.Request.Form["Driver_Photo"];
                string Driver_License = HttpContext.Current.Request.Form["Driver_License"];
                string Driver_Car_License = HttpContext.Current.Request.Form["Driver_Car_License"];
                #endregion
                Driver driver = db.Drivers.SingleOrDefault(x => x.Token == Token);
                if (!string.IsNullOrEmpty(Email))
                {
                    if (db.Drivers.Where(x => x.ID != driver.ID).Any(x => x.Email == Email))
                    {
                        return BadRequest("البريد الالكتروني مستخدم بالفعل !!");
                    }
                    driver.Email = Email;
                }
                if (!string.IsNullOrEmpty(FirstName))
                    driver.FirstName = FirstName;
                if (!string.IsNullOrEmpty(LastName))
                    driver.LastName = LastName;
                if (!string.IsNullOrEmpty(Password))
                    driver.Password = Password;
                if (!string.IsNullOrEmpty(Location))
                    driver.Location = Location;
                if (!string.IsNullOrEmpty(Phone))
                    driver.Phone = Phone;
                db.Entry(driver).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                if (!string.IsNullOrEmpty(Driver_Photo))
                {
                    byte[] PhotoBase64 = Convert.FromBase64String(Driver_Photo);
                    File.WriteAllBytes(HttpContext.Current.Server.MapPath("~/Uploads/Driver_Photo/" + driver.ID + ".jpg"), PhotoBase64);
                }
                if (!string.IsNullOrEmpty(Driver_Photo))
                {
                    byte[] Driver_LicenseBase64 = Convert.FromBase64String(Driver_License);
                    File.WriteAllBytes(HttpContext.Current.Server.MapPath("~/Uploads/Driver_License/" + driver.ID + ".jpg"), Driver_LicenseBase64);
                }
                if (!string.IsNullOrEmpty(Driver_Photo))
                {
                    byte[] Driverr_Car_LicenseBase64 = Convert.FromBase64String(Driver_Car_License);
                    File.WriteAllBytes(HttpContext.Current.Server.MapPath("~/Uploads/Driver_Car_License/" + driver.ID + ".jpg"), Driverr_Car_LicenseBase64);
                }
                return Ok(new { Message = "تم تعديل بيانات السائق بنجاح ." });
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult NearOrders(double Lat,double Log)
        {
            var Location = new GeoCoordinate(Lat, Log);
            var Orders = db.Orders.Where(x => 
            x.IsOrderd &&x.Accepted &&
            !x.ISArrivedToUser &&
            !x.Driver_ID.HasValue)
            .AsEnumerable().
                Select(x => new
                {
                    x.ID,
                    Date=x.Date.ToString("d"),
                    Time = x.Date.ToString("T"),
                    DateNeeded = x.DateTimeNeeded.HasValue ? x.DateTimeNeeded.Value.ToString("d") : null,
                    TimeNeeded = x.DateTimeNeeded.HasValue ? x.DateTimeNeeded.Value.ToString("T") : null,
                    x.Status,
                    x.User.Phone,
                    ProviderName= x.Product.Provider.TradeName,
                    ProviderLocation= x.Product.Provider.Location,
                    ProviderPhone = x.Product.Provider.Phone,
                    x.Product.Provider.Lat,
                    x.Product.Provider.Log,
                    x.UserLat,
                    x.UserLog,
                    Distance =Convert.ToInt16(Location.GetDistanceTo(new GeoCoordinate(x.Product.Provider.Lat, x.Product.Provider.Log)) * 0.001)
                }).Where(x=>x.Distance<50);
            return Ok(Orders);
        }
        [HttpPost]
        public IHttpActionResult DriverAcceptOrder(int Order_ID)
        {
            try
            {
                string Token = HttpContext.Current.Request.Form["Token"];
                Driver driver = db.Drivers.SingleOrDefault(x => x.Token == Token);
                Order order = db.Orders.Find(Order_ID);
                if (order.Driver_ID.HasValue || !order.Accepted || order.ISArrivedToUser || !order.IsOrderd )
                {
                    return BadRequest("هذا الطلب غير متاح لك !!");
                }
                if(!driver.Visible)
                {
                    return BadRequest("انت غير متاح .. برجاء التواصل مع الشركة !!");
                }
                order.Driver_ID = driver.ID;
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Ok(new { Message = "تستطيع الان القيادة ." });
             }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public IHttpActionResult ArriveToUser(int Order_ID)
        {
            try
            {
                string Token = HttpContext.Current.Request.Form["Token"];
                Order order = db.Orders.SingleOrDefault(x => x.ID == Order_ID && x.Driver.Token == Token);
                order.ISArrivedToUser = true;
                order.Status = "تم التوصيل";
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Ok(new { Message = "شكرا لتوصيلك هذا الطلب ." });
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [CacheFilter(TimeDuration = 10000)]
        public IHttpActionResult DriversOrders()
        {
            string Token = HttpContext.Current.Request.Form["Token"];
            var driver = db.Drivers.SingleOrDefault(x => x.Token == Token);
             var orders=driver.Orders.Where(x => !x.ISArrivedToUser&&!x.ProblemInArriveToUser).OrderByDescending(x => x.Date).AsEnumerable().Select(x => new
            {
                x.ID,
                x.User.Phone,
                x.Product.Name,
                Date=x.Date.ToString("d"),
                Time = x.Date.ToString("T"),
                x.ISArrivedToUser,
                x.IsUseVisa,
                x.Number,
                x.UserLat,
                x.UserLog,
                x.Product.Provider.Lat,
                x.Product.Provider.Log,
                x.Product.Provider.TradeName,
                ProviderPhone = x.Product.Provider.Phone,
                ProviderLocation = x.Product.Provider.Location,
                x.Status,
                Offer = x.Offer_ID != null ? x.Offer.Name : null,
                x.Ready,
                DateNeeded = x.DateTimeNeeded.HasValue ? x.DateTimeNeeded.Value.ToString("d") : null,
                TimeNeeded = x.DateTimeNeeded.HasValue ? x.DateTimeNeeded.Value.ToString("T") : null,
                x.Discount,
                x.Price,
                x.Total,
                x.Reason,
                x.Delivery,
                x.Product.Provider.Currency,
            });
            return Ok(orders);
        }
        [HttpPost]
        [CacheFilter(TimeDuration = 10000)]
        public IHttpActionResult DriversPastOrders()
        {
            string Token = HttpContext.Current.Request.Form["Token"];
            var driver = db.Drivers.SingleOrDefault(x => x.Token == Token);
            var orders= driver.Orders.Where(x => x.ISArrivedToUser||x.ProblemInArriveToUser).OrderByDescending(x => x.Date).AsEnumerable().Select(x => new
            {
                x.ID,
                x.User.Phone,
                x.Product.Name,
                Date=x.Date.ToString("d"),
                Time = x.Date.ToString("T"),
                x.ISArrivedToUser,
                x.IsUseVisa,
                x.Number,
                x.UserLat,
                x.UserLog,
                x.Product.Provider.Lat,
                x.Product.Provider.Log,
                x.Product.Provider.TradeName,
                ProviderPhone = x.Product.Provider.Phone,
                ProviderLocation = x.Product.Provider.Location,
                x.Status,
                Offer = x.Offer_ID != null ? x.Offer.Name : null,
                x.Ready,
                DateNeeded = x.DateTimeNeeded.HasValue ? x.DateTimeNeeded.Value.ToString("d") : null,
                TimeNeeded = x.DateTimeNeeded.HasValue ? x.DateTimeNeeded.Value.ToString("T") : null,
                x.Discount,
                x.Price,
                x.Total,
                x.ProblemInArriveToUser,
                x.Reason,
                x.Delivery,
                x.Product.Provider.Currency,
            });
            return Ok(orders);
        }
        [HttpGet]
        public IHttpActionResult Companies()
        {
            var data = db.Companies.Select(x => new { x.ID,x.Name }).ToList();
            data.Insert(0, new {ID=0,Name="Free Hands" });
            return Ok(data);
        }
        [HttpPost]
        public IHttpActionResult MakeReason(int Order_ID)
        {
            try {
                string Token = HttpContext.Current.Request.Form["Token"];
                string Reason = HttpContext.Current.Request.Form["Reason"];
                Order order = db.Orders.SingleOrDefault(x => x.ID == Order_ID && x.Driver.Token == Token);
                order.ProblemInArriveToUser = true;
                order.Reason = Reason;
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Ok(new { Message = "شكرا لاضافة سبب لعدم استلام الطلب ." });
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public IHttpActionResult ForgetPassword_Driver()
        {
            string Email = HttpContext.Current.Request.Form["Email"];
            Driver driver = db.Drivers.SingleOrDefault(x => x.Email == Email);
            if (driver == null)
            {
                return BadRequest(" البريد الالكتروني غير موجود !!");
            }
            try
            {
                DEL.Send_Password(driver.Password, driver.Email);
                return Ok(new { Message = "يمكنك تصفح البريد الالكتروني ." });
            }
            catch
            {
                return BadRequest("حدث خطأ اثناء ارسال رسالة البريد !!");
            }
        }






        //__________________________________[Users]______________________________________________________

        [HttpGet]
        public IHttpActionResult Offers(double Lat,double Log)
        {
            var Location = new GeoCoordinate(Lat, Log);
            var Offers = db.Offers.Where(x => x.Active&&x.Provider.Admin_Accept)
                .AsEnumerable()
                .Select(x=>new {
                    x.ID,
                    x.Name,
                    x.Price,
                    x.Provider_ID,
                    x.Description,
                    ProdivderName =x.Provider.TradeName,
                    x.Provider.Currency,
                    x.Product_ID,
                    ProductName=x.Product.Name,
                    Photo=DEL.Domain+"/Uploads/Products/"+x.Product_ID+".jpg",
                    Distance=Location.GetDistanceTo(new GeoCoordinate(x.Provider.Lat,x.Provider.Log)) * 0.001
                }).Where(x=>x.Distance<50);
            return Ok(Offers);
        }
        [HttpGet]
        public IHttpActionResult GetNearProviders(double Lat, double Log)
        {
            var Location = new GeoCoordinate(Lat, Log);
            var Providers = db.Providers.Where(x=>x.IsAvaiable&&x.Admin_Accept).AsEnumerable().Select(x => new
            {
                x.ID,
                x.FirstName,
                x.LastName,
                x.TradeName,
                x.AllowVisa,
                x.IsExcelent,
                x.Discount,
                x.Currency,
                Gategories=string.Join(" , ",x.Products.Select(p=>p.Category.Name).Distinct().ToArray()),
                Photo=DEL.Domain+ "/Uploads/Provider_Photo/"+x.ID+".jpg",
                Rate = x.Provider_Rates.Where(r => r.Rate.HasValue).Count()>0?x.Provider_Rates.Where(r => r.Rate.HasValue).Average(r => r.Rate.Value):0,
                Distance =Location.GetDistanceTo(new GeoCoordinate(x.Lat,x.Log)) *0.001
            }).Where(x=>x.Distance<70);
            return Ok(Providers);
        }
        [HttpPost]
        public IHttpActionResult Search(double Lat, double Log)
        {
            string KeyWord = HttpContext.Current.Request.Form["KeyWord"];
            var Location = new GeoCoordinate(Lat, Log);
            var Providers = db.Providers.Where(x => x.IsAvaiable == true&&x.Admin_Accept&&x.TradeName.Contains(KeyWord)).AsEnumerable().Select(x => new
            {
                x.ID,
                x.FirstName,
                x.LastName,
                x.TradeName,
                x.AllowVisa,
                x.IsExcelent,
                x.Currency,
                Photo = DEL.Domain + "/Uploads/Provider_Photo/" + x.ID + ".jpg",
                Rate = x.Provider_Rates.Where(r => r.Rate.HasValue).Count() > 0 ? x.Provider_Rates.Where(r => r.Rate.HasValue).Select(r => r.Rate.Value).Average() : 0,
                Distance = Location.GetDistanceTo(new GeoCoordinate(x.Lat, x.Log)) * 0.001
            }).Where(x => x.Distance < 70);
            return Ok(Providers);
        }
        [HttpGet]
        public IHttpActionResult SingleProvider(int ID)
        {
            var Provider = db.Providers.Where(x => x.IsAvaiable && x.ID == ID && x.Admin_Accept).AsEnumerable().Select(x => new
            {
                x.ID,
                x.FirstName,
                x.LastName,
                x.TradeName,
                x.Location,
                x.IsExcelent,
                x.Discount,
                x.Currency,
                Rate = x.Provider_Rates.Count==0?0: x.Provider_Rates.Where(r => r.Rate.HasValue).Average(r => r.Rate.Value),
                Photo = DEL.Domain + "/Uploads/Provider_Photo/" + x.ID + ".jpg",
                Categories = x.Products.Where(p => p.Visible).GroupBy(c=>new {c.Cat_ID,c.Category.Name }).Select(c => new {
                    c.Key.Cat_ID,
                    c.Key.Name,
                    Products=c.Select(p=>new
                    {
                        p.ID,
                        p.Name,
                        p.Description,
                        p.Price,
                        p.Provider.Currency,
                        TimeToFinishH = TimeSpan.FromMinutes(p.TimeToFinish).ToString("hh"),
                        TimeToFinishM = TimeSpan.FromMinutes(p.TimeToFinish).ToString("mm"),
                        Rate = p.Product_Rates.Count == 0 ? 0 : p.Product_Rates.Where(r => r.Rate.HasValue).Average(r => r.Rate.Value),
                        Photo = DEL.Domain + "/Uploads/Products/" + p.ID + ".jpg"
                    })
                })
            }).FirstOrDefault();
            return Ok(Provider);
        }
        [HttpGet]
        public IHttpActionResult SingleProduct(int ID)
        {
            try
            {
                var Product = db.Products.Where(x => x.ID == ID&&x.Visible&&x.Provider.Admin_Accept).AsEnumerable().Select(x => new
                {
                    x.ID,
                    x.Name,
                    x.Price,
                    x.Provider.Currency,
                    x.Description,
                    TimeToFinishH = TimeSpan.FromMinutes(x.TimeToFinish).ToString("hh"),
                    TimeToFinishM = TimeSpan.FromMinutes(x.TimeToFinish).ToString("mm"),
                    Photo =DEL.Domain+"/Uploads/Products/"+x.ID+".jpg",
                    Options=x.Options.Select(p =>new
                    {
                        p.ID,
                        p.OptionName,
                        p.Price,
                        p.Description
                    }),
                    Offers = x.Offers.Where(o=>o.Active).Select(o => new
                    {
                        o.ID,
                        o.Name,
                        o.Description
                    })
                }).FirstOrDefault();
                if(Product==null)
                {
                    return BadRequest("This Product Not Found .");
                }
                return Ok(Product);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult AddToCart(int Product_ID, double Lat, double Log)
        {
            try
            {
                string Token = HttpContext.Current.Request.Form["Token"];
                int Number = int.Parse(HttpContext.Current.Request.Form["Number"]);
                string Offer_ID = HttpContext.Current.Request.Form["Offer_ID"];
                string[] Options = HttpContext.Current.Request.Form.GetValues("Options");

                User user = db.Users.SingleOrDefault(x => x.Token == Token);
                Order order = new Order
                {
                    Date = DateTime.Now,
                    Number = Number,
                    User_ID = user.ID,
                    Product_ID = Product_ID,
                    UserLat=Lat,
                    UserLog=Log,
                    IsOrderd=false,
                    Driver_ID=null,  
                                                         
                };
                if(string.IsNullOrEmpty(Offer_ID))
                {
                  if(Options!=null&&!string.IsNullOrEmpty(Options[0]))
                  {
                    foreach (var item in Options)
                    {
                        order.OrderOptions.Add(new OrderOption { Option_ID = int.Parse(item) });
                    }
                  }
                }
                else
                {
                    int ID = int.Parse(Offer_ID);
                    Offer offer = db.Offers.Find(ID);
                    if(!offer.Active)
                    {
                        return BadRequest("هذا العرض غير متاح !!");
                    }
                    order.Offer_ID = ID;
                }
               
                db.Orders.Add(order);
                db.SaveChanges();
                return Ok(new { Message = "تم الاضافة الي السلة بنجاح .",Order_ID=order.ID });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public IHttpActionResult DeleteFromCart(int Order_ID)
        {
            try
            {
                string Token = HttpContext.Current.Request.Form["Token"];
                Order order = db.Orders.Where(x => x.ID == Order_ID && x.User.Token == Token).FirstOrDefault();
                db.Orders.Remove(order);
                db.SaveChanges();
                return Ok(new { Message = "تم الحذف من السلة ." });

            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public IHttpActionResult UserLogin()
        {
            try
            {
                string Phone = HttpContext.Current.Request.Form["Phone"];
                string City = HttpContext.Current.Request.Form["City"];
                string Country = HttpContext.Current.Request.Form["Country"];
                var user = db.Users.Where(x => x.Phone == Phone).Select(x => new
                {
                    x.ID,
                    x.Phone,
                    x.Token,
                    x.IsActive,
                }).FirstOrDefault();
                string Request_ID = "";
                if (user != null)
                {
                    if(user.IsActive==false)
                    {
                       Request_ID = DEL.SendVerifySMS(user.Phone);
                       return Ok(new { Request_ID = Request_ID, User = user });
                    }                      
                    return Ok(new {User= user });
                }
                User newuser = new User { Phone = Phone, Token = DEL.encrypt(Phone), IsActive = false };
                DEL.Location location = DEL.CountryCityID(Country, City);
                newuser.Country_ID = location.Country_ID;
                newuser.City_ID = location.City_ID;
                db.Users.Add(newuser);
                db.SaveChanges();
                Request_ID = DEL.SendVerifySMS(newuser.Phone);
                return Ok(new { Request_ID = Request_ID, User = newuser } );
        }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public IHttpActionResult CheckVerify()
        {
            try
            {
                string Token = HttpContext.Current.Request.Form["Token"];
                string Code = HttpContext.Current.Request.Form["Code"];
                string request_id = HttpContext.Current.Request.Form["request_id"];
                if (DEL.CheckVerify(Code, request_id))
                {
                    User user = db.Users.Where(x => x.Token == Token).FirstOrDefault();
                    user.IsActive = true;
                    db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Ok(new { Message = "true" });
                }
                return BadRequest("false");
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [CacheFilter(TimeDuration =10000)]
        public IHttpActionResult Cart()
        {
            try
            {
                string Token = HttpContext.Current.Request.Form["Token"];
                User user = db.Users.SingleOrDefault(x => x.Token == Token);
                if (!user.IsActive)
                {
                    return BadRequest("This User Not Active !");
                }
                Datum data = db.Data.Find(1);
                var Data = user.Orders.Where(x => x.IsOrderd == false).OrderByDescending(x => x.ID).Select(x => new
                {
                    x.ID,
                    Date = x.Date.ToString("d"),
                    Time = x.Date.ToString("T"),
                    x.Number,
                    ProductName = x.Product.Name,
                    ProviderName = x.Product.Provider.TradeName,
                    ProviderLocation = x.Product.Provider.Location,
                    ProviderPhone = x.Product.Provider.Phone,
                    Offer = x.Offer_ID.HasValue ? x.Offer.Name : null,
                    data.Delivery,
                    data.TaxForVisa,
                    Discount = x.Product.Provider.Discount,
                    Options =  x.OrderOptions.Select(o => new { o.Option.OptionName, o.Option.Price }),
                    OptionsPrice =  x.OrderOptions.Sum(n => n.Option.Price),
                    OfferProductPrice = x.Offer_ID.HasValue ? x.Offer.Price : x.Product.Price,
                    TotalPrice = x.Offer_ID.HasValue ? DEL.GetTotalPrice(x.Offer.Price,x.Number, data.Delivery, null, x.Product.Provider.Discount)
                    : DEL.GetTotalPrice(x.Product.Price + x.OrderOptions.Sum(n => n.Option.Price),x.Number, data.Delivery, null, x.Product.Provider.Discount),
                });
                return Ok(Data);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [CacheFilter(TimeDuration = 10000)]
        public IHttpActionResult Orders()
        {
            try
            {
                string Token = HttpContext.Current.Request.Form["Token"];
                User user = db.Users.SingleOrDefault(x => x.Token == Token);
                if (!user.IsActive)
                {
                    return BadRequest("This User Not Active !");
                }
                Datum data = db.Data.Find(1);
                var Data = user.Orders.Where(x => x.IsOrderd).OrderByDescending(x => x.ID).Select(x => new
                {
                    x.ID,
                    Date=x.Date.ToString("d"),
                    Time = x.Date.ToString("T"),
                    x.Number,
                    x.Accepted,
                    ProviderAllowVisa = x.Product.Provider.AllowVisa,
                    AdminAllowVisa = data.AllowVisa,
                    x.IsUseVisa,
                    x.Status,
                    DriverName = x.Driver_ID.HasValue ? x.Driver.FirstName : null,
                    DriverPhone = x.Driver_ID.HasValue ? x.Driver.Phone : null,
                    ProductName = x.Product.Name,
                    ProviderName = x.Product.Provider.TradeName,
                    ProviderLocation = x.Product.Provider.Location,
                    ProviderPhone = x.Product.Provider.Phone,
                    Offer=x.Offer_ID.HasValue?x.Offer.Name:null,
                    Options = x.OrderOptions.Select(o => new { o.Option.OptionName, o.Option.Price }),
                    x.Ready,
                    x.ISArrivedToUser,
                    x.Delivery,
                    x.TaxForVisa,
                    x.Discount,
                    x.ProblemInArriveToUser,
                    x.Reason,
                    DateNeeded = x.DateTimeNeeded.HasValue ? x.DateTimeNeeded.Value.ToString("d") : null,
                    TimeNeeded = x.DateTimeNeeded.HasValue ? x.DateTimeNeeded.Value.ToString("T") : null,
                    x.Price,
                    x.Total,
                    x.Product.Provider.Currency,
                    TotalPrice = x.Offer_ID.HasValue ? DEL.GetTotalPrice(x.Offer.Price, x.Number, data.Delivery, null, x.Product.Provider.Discount)
                    : DEL.GetTotalPrice(x.Product.Price + x.OrderOptions.Sum(n => n.Option.Price), x.Number, data.Delivery, null, x.Product.Provider.Discount),
                    TotalPriceWithVisaTax = x.Offer_ID.HasValue ? DEL.GetTotalPrice(x.Offer.Price, x.Number, data.Delivery, data.TaxForVisa, x.Product.Provider.Discount)
                    : DEL.GetTotalPrice(x.Product.Price + x.OrderOptions.Sum(n => n.Option.Price), x.Number, data.Delivery, data.TaxForVisa, x.Product.Provider.Discount),
                });
                return Ok(Data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public IHttpActionResult Order()
        {
            try
            {
                string Token = HttpContext.Current.Request.Form["Token"];
                int Order_ID = int.Parse(HttpContext.Current.Request.Form["Order_ID"]);
                DateTime DateTimeNeeded = Convert.ToDateTime(HttpContext.Current.Request.Form["DateTimeNeeded"]);

                var order = db.Orders.SingleOrDefault(x => x.ID == Order_ID && x.User.Token == Token);
                if(order.IsOrderd)
                {
                    return BadRequest("هذا الطلب تم طلبه من قبل !!");
                }
                if (!order.Product.Provider.IsAvaiable|| !order.Product.Provider.Admin_Accept)
                {
                    return BadRequest("مقدم الخدمة غير متاح الان !!");
                }
                DateTime ActiveDateTime = DateTime.Now.AddMinutes(order.Product.TimeToFinish);
                if(ActiveDateTime.CompareTo(DateTimeNeeded)>0)
                {
                    return BadRequest("التاريخ والوقت المحدد غير صحيحان !");
                }
                order.DateTimeNeeded = DateTimeNeeded;
                Datum data = db.Data.Find(1);
                if(order.Product.Provider.Discount.HasValue)
                {
                    order.Discount = order.Product.Provider.Discount.Value;
                }
                order.Delivery = data.Delivery;

                double Price = order.Offer_ID.HasValue ? order.Offer.Price : order.Product.Price+order.OrderOptions.Sum(s=>s.Option.Price);
                Price = Price * order.Number;
                if (order.Discount.HasValue)
                {
                    double discount = (order.Discount.Value / 100) * Price;
                    Price = Price - discount;
                }
                order.Price = Price;

                order.Total = DEL.GetTotalPriceAfterFreeHandsTaxs(order.Price.Value, order.Delivery,null);
                order.IsOrderd = true;

                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                string Message = "You Have New Order In Free Hands . ";
                DEL.SendAcceptSms(order.Product.Provider.Phone, Message);

                return Ok(new { Message = "تم عمل الطلب بنجاح .." });
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public IHttpActionResult MakeOrderUseVisa(int Order_id)
        {
            string Token = HttpContext.Current.Request.Form["Token"];
            Order order = db.Orders.Where(x => x.User.Token == Token && x.ID == Order_id).SingleOrDefault();
            if(order==null)
            {
                return BadRequest();
            }
            Datum data = db.Data.Find(1);
            double TaxForVisa = (data.TaxForVisa * order.Total.Value) / 100;
            order.Total = order.Total + TaxForVisa;
            order.TaxForVisa = data.TaxForVisa;
            order.IsUseVisa = true;

            db.Entry(order).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Ok(new {Message="تمت عملية الدفع الالكتروني .. شكرا لك" });
        }

        [HttpPost]
        public IHttpActionResult Rate_Provider(int Provider_ID)
        {
            string Token = HttpContext.Current.Request.Form["Token"];
            double Rate = Convert.ToDouble(HttpContext.Current.Request.Form["Rate"]);
            string Comment = HttpContext.Current.Request.Form["Comment"];
            User user = db.Users.SingleOrDefault(x => x.Token == Token);
            try
            {
                System.Data.Entity.Core.Objects.ObjectParameter ReturnValue = new System.Data.Entity.Core.Objects.ObjectParameter("ReturnValue", typeof(int));
                db.RateProvider(Rate, user.ID, Comment, Provider_ID, ReturnValue);
                db.SaveChanges();
                if (ReturnValue.Value.ToString() == "0")
                {
                    return BadRequest("لا تستطيع التقييم الا مرة واحدة !!"); ;
                }
                else
                {
                    return Ok(new { Message = "تم التقييم بنجاح ." });
                }

            }
            catch(Exception ex) { return BadRequest(ex.Message); }
        }
        [HttpPost]
        public IHttpActionResult Rate_Product(int Product_ID)
        {
            string Token = HttpContext.Current.Request.Form["Token"];
            double Rate = Convert.ToDouble(HttpContext.Current.Request.Form["Rate"]);
            string Comment = HttpContext.Current.Request.Form["Comment"];
            User user = db.Users.SingleOrDefault(x => x.Token == Token);
            try
            {
                System.Data.Entity.Core.Objects.ObjectParameter ReturnValue = new System.Data.Entity.Core.Objects.ObjectParameter("ReturnValue", typeof(int));
                db.RateProduct(Rate, Product_ID, Comment, user.ID, ReturnValue);
                db.SaveChanges();
                if (ReturnValue.Value.ToString() == "0")
                {
                    return BadRequest("لا تستطيع التقييم الا مرة واحدة !!"); ;
                }
                else
                {
                    return Ok(new { Message = "تم التقييم بنجاح ." });
                }

            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
        [HttpGet]
        public IHttpActionResult Rates_Comments_Provider(int Provider_ID)
        {
            var Rates = db.Provider_Rates.Where(x => x.Provider_ID == Provider_ID&&x.Avaiable).Select(x => new
            {
                x.ID,
                x.Comment,
                x.Rate,
            });
            return Ok(Rates);
        }
        [HttpGet]
        public IHttpActionResult Rates_Comments_Product(int Product_ID)
        {
            var Rates = db.Product_Rates.Where(x => x.Product_ID == Product_ID&&x.Avaiable).Select(x => new
            {
                x.ID,
                x.Comment,
                x.Rate,
            });
            return Ok(Rates);
        }
    }
}

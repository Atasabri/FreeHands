using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using Nexmo.Api;
using FreeHands.Models;

namespace FreeHands
{
    public class DEL
    {

        private static string CipherKey = ConfigurationManager.AppSettings["Cipher"].ToString();
        public static string Domain = ConfigurationManager.AppSettings["Domain"].ToString();
        public static string ApiKey = ConfigurationManager.AppSettings["ApiKey"].ToString();
        public static string ApiSecret = ConfigurationManager.AppSettings["ApiSecret"].ToString();


        
        
        /// <summary>
        /// This Function For Calculate All Total Price That Will Pay 
        /// </summary>
        /// <param name="price"></param>
        /// <param name="delivery"></param>
        /// <param name="taxforvisa"></param>
        /// <returns></returns>
        public static double GetTotalPriceAfterFreeHandsTaxs(double price, int? delivery,double? taxforvisa)
        {
            double TotalPrice;
            TotalPrice = price + delivery.Value;
            if (taxforvisa.HasValue)
            {
                double TaxForVisa = (taxforvisa.Value / 100) * TotalPrice;
                TotalPrice = TotalPrice + TaxForVisa;
            }         
            return TotalPrice;
        }
        public static double GetTotalPrice(double price, int Number, int? delivery, double? taxforvisa, double? Discount)
        {
            price = price * Number;
            if (Discount.HasValue)
            {
                double discount = (Discount.Value / 100) * price;
                price = price - discount;
            }
            if (!delivery.HasValue)
            {
                delivery = 0;
            }
            price = price + delivery.Value;
            if (taxforvisa.HasValue)
            {
                double TaxForVisa = (taxforvisa.Value / 100) * price;
                price = price + TaxForVisa;
            }
            return price;
        }

        /// <summary>
        /// This Function For Calculate Money That Will Provider Get After Delete FreeHands Tax
        /// </summary>
        /// <param name="Price"></param>
        /// <returns></returns>
        public static double GetMoneyAfterTax(double Price,double Tax)
        {           
            double discountprice = (Tax * Price) / 100;
            return Price - discountprice;
        }

        /// <summary>
        /// Encrypt A string And Retuen Encryption String
        /// </summary>
        /// <param name="encryptString"></param>
        /// <returns></returns>
        public static string encrypt(string encryptString)
        {
            string EncryptionKey = CipherKey;
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
            });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptString;
        }
        /// <summary>
        /// Return A string After Decrypt
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = CipherKey;
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
           });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        /// <summary>
        /// Send Html E-Mail To Muli-Users
        /// </summary>
        /// <param name="Subject"></param>
        /// <param name="file"></param>
        /// <param name="To"></param>
        public static void Send_Mail(string Subject, HttpPostedFileBase file, List<string> To)
        {
            string From = ConfigurationManager.AppSettings["Email"].ToString();
            string Pass = ConfigurationManager.AppSettings["Password"].ToString();
            string Host = ConfigurationManager.AppSettings["Host"].ToString();
            int Port = int.Parse(ConfigurationManager.AppSettings["Port"].ToString());
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(From);
            foreach (var item in To)
            {
                if (item.Contains("@"))
                {
                    mail.To.Add(item);
                }
            }
            mail.Subject = Subject;
            StreamReader read = new StreamReader(file.InputStream);
            mail.Body = read.ReadToEnd();
            mail.IsBodyHtml = true;
            ///-------------------------------------------------------------------------//
            SmtpClient smtpMail = new SmtpClient();
            smtpMail.EnableSsl = false;
            smtpMail.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpMail.Host = Host;
            smtpMail.Port = Port;

            smtpMail.UseDefaultCredentials = false;
            smtpMail.Credentials = new NetworkCredential(From, Pass);
            ///-------------------------------------------------------------------------//
            smtpMail.Send(mail);
        }
        /// <summary>
        /// Send Password To E-Mail
        /// </summary>
        /// <param name="Subject"></param>
        /// <param name="file"></param>
        /// <param name="To"></param>
        public static void Send_Password(string UserPass, string To)
        {
            string From = ConfigurationManager.AppSettings["Email"].ToString();
            string Pass = ConfigurationManager.AppSettings["Password"].ToString();
            string Host = ConfigurationManager.AppSettings["Host"].ToString();
            int Port = int.Parse(ConfigurationManager.AppSettings["Port"].ToString());
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(From);
            mail.To.Add(To);
            mail.Subject = "FreeHands Confirmation Password";

            mail.Body = "Hi , Your Password Is : " + UserPass;
            ///-------------------------------------------------------------------------//
            SmtpClient smtpMail = new SmtpClient();
            smtpMail.EnableSsl = false;
            smtpMail.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpMail.Host = Host;
            smtpMail.Port = Port;

            smtpMail.UseDefaultCredentials = false;
            smtpMail.Credentials = new NetworkCredential(From, Pass);
            ///-------------------------------------------------------------------------//
            smtpMail.Send(mail);
        }

        /// <summary>
        /// Send Message To Phone 
        /// </summary>
        /// <param name="Phone"></param>
        /// <param name="Message"></param>
        public static void SendAcceptSms(string Phone,string Message)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                        | SecurityProtocolType.Tls11
                        | SecurityProtocolType.Tls12
                        | SecurityProtocolType.Ssl3;
            var client = new Client(creds: new Nexmo.Api.Request.Credentials
            {
                ApiKey = ApiKey,
                ApiSecret = ApiSecret
            });
            var results = client.SMS.Send(request: new SMS.SMSRequest
            {

                from = "Free Hands",
                to = Phone,
                text = Message,               
            });
        }
        /// <summary>
        /// Send Verfiy Code To Phone
        /// </summary>
        /// <param name="Phone"></param>
        /// <returns></returns>
        public static string SendVerifySMS(string Phone)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                        | SecurityProtocolType.Tls11
                        | SecurityProtocolType.Tls12
                        | SecurityProtocolType.Ssl3;
            var client = new Client(creds: new Nexmo.Api.Request.Credentials
            {
                ApiKey = ApiKey,
                ApiSecret = ApiSecret
            });
            var start = client.NumberVerify.Verify(new NumberVerify.VerifyRequest
            {
                number = Phone,
                brand = "Free Hands"
            });
           return start.request_id;
        }
        /// <summary>
        /// Check Verify Code Using request_id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="request_id"></param>
        /// <returns></returns>
        public static bool CheckVerify(string Code,string request_id)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                        | SecurityProtocolType.Tls11
                        | SecurityProtocolType.Tls12
                        | SecurityProtocolType.Ssl3;
            var client = new Client(creds: new Nexmo.Api.Request.Credentials
            {
                ApiKey = ApiKey,
                ApiSecret = ApiSecret
            });
            var result = client.NumberVerify.Check(new NumberVerify.CheckRequest
            {
                code = Code,
                request_id= request_id
            });

            if(result.status== "0")
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// This Function Return Country And City IDs For Users & Providers & Drivers 
        /// </summary>
        /// <param name="Country"></param>
        /// <param name="City"></param>
        /// <returns></returns>
        public static Location CountryCityID(string Country, string City)
        {
            DB db = new DB();
            Location Location = new Location();
            Country country = db.Countries.SingleOrDefault(x => x.Name == Country);
            if (country != null)
            {
                Location.Country_ID = country.ID;
                City city = db.Cities.SingleOrDefault(x => x.Name == City && x.Country_ID == country.ID);
                if (city != null)
                {
                    Location.City_ID = city.ID;
                }
                else
                {
                    City newcity = new City { Name = City, Country_ID = country.ID };
                    db.Cities.Add(newcity);
                    db.SaveChanges();
                    Location.City_ID = newcity.ID;
                }

            }
            else
            {
                Country newcountry = new Country { Name = Country };
                City newcity = new City { Name = City };
                db.Cities.Add(newcity);
                db.Countries.Add(newcountry);
                db.SaveChanges();
                Location.Country_ID = newcountry.ID;
                Location.City_ID = newcity.ID;
            }
            return Location;
        }
        
        public struct Location
        {
            public int Country_ID { get; set; }
            public int City_ID { get; set; }
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using GAuthenticator.Models;
using Google.Authenticator;

namespace GAuthenticator.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
     //   string userName = WebConfigurationManager.AppSettings["GAuthPrivateKey"]
      //  private const string key = "qazqaz12345"; //You can add your own Key
        public ActionResult Login()
        {           
            return View();
        }
        
        [HttpPost]
        public ActionResult Login(LoginModel login)
        {
            string message = "";
            bool status = false;
            //check UserName and password form our database hereEncoding.ASCII.GetBytes(input);
            string GAuthPrivKey = WebConfigurationManager.AppSettings["GAuthPrivateKey"];
            byte[] UserUniqueKey = Encoding.ASCII.GetBytes(login.UserName + GAuthPrivKey);
            string userSecretKey = (login.UserName + GAuthPrivKey);
            if (login.UserName == "TestUser" && login.Password == "12345") // Admin as user name and 12345 as Password
            {
                status = true;
                Session["UserName"] = login.UserName;

                if (WebConfigurationManager.AppSettings["GAuthEnable"].ToString() =="1")
                {
                    HttpCookie TwoFCookie = Request.Cookies["TwoFCookie"];
                    int k = 0;
                    if (TwoFCookie == null)
                    {
                        k = 1;
                    }
                    else
                    {

                        if (!string.IsNullOrEmpty(TwoFCookie.Values["UserCode"]))
                        {
                            string UserCodeE =TwoFCookie.Values["UserCode"];
                            byte[] bytes =Encoding.ASCII.GetBytes(UserCodeE);
        
                            if (UserUniqueKey.SequenceEqual(bytes))
                            {
                                FormsAuthentication.SetAuthCookie(Session["Username"].ToString(), false);
                                ViewBag.Message = "Welcome to Mr. " + Session["Username"].ToString();
                                
                                return RedirectToAction("UserProfile");
                            }
                            else
                            {
                                k = 1;
                            }
                        }
                    }

                    if (k == 1)
                    {

                        message = "Two Factor Authentication Verification";
                   
                        //Two Factor Authentication Setup
                        TwoFactorAuthenticator TwoFacAuth = new TwoFactorAuthenticator();

                        Session["UserUniqueKey"] = userSecretKey;
                        //var setupInfo = TwoFacAuth.GenerateSetupCode("TEST 2FA", login.UserName, UserUniqueKey, 300, 300);// old version of nuget package
                        var setup1s = TwoFacAuth.GenerateSetupCode("test", login.UserName, UserUniqueKey, qrPixelsPerModule:3,generateQrCode: true);
                        ViewBag.BarcodeImageUrl = setup1s.QrCodeSetupImageUrl;
                        ViewBag.SetupCode = setup1s.ManualEntryKey;
                    }
                }
                
                else
                {
                    FormsAuthentication.SetAuthCookie(Session["Username"].ToString(),true);
                    ViewBag.Message = "Welcome to Mr. " + Session["Username"].ToString();
                    return View("UserProfile");
                    
                }
            }

            else
            {
                message = "Please Enter the Valid Credential!";
            }

            ViewBag.Message = message;
            ViewBag.Status = status;
           
            return View();
        }
        [Authorize]
        public ActionResult UserProfile()
        {
            ViewBag.Message = "Welcome to  " + Session["Username"].ToString();
            
            return View();
        }

        public string TwoFactorAuthenticate(string Code)
        {
            var token = Code;
            TwoFactorAuthenticator TwoFacAuth = new TwoFactorAuthenticator();
            string userSecretKey = Session["UserUniqueKey"].ToString();
            byte[] UserUniqueKey = Encoding.ASCII.GetBytes(userSecretKey);
            bool isValid = TwoFacAuth.ValidateTwoFactorPIN(UserUniqueKey, token);

            if (isValid)
            {
                HttpCookie TwoFCookie = new HttpCookie("TwoFCookie");
                string UserCode = Encoding.ASCII.GetString(UserUniqueKey);

               TwoFCookie.Values.Add("UserCode", UserCode);
              //TwoFCookie.Expires = DateTime.Now.AddDays(30);
                Response.Cookies.Add(TwoFCookie);
                Session["IsValidTwoFactorAuthentication"] = true;
               
                return "Success";
            }
           
            return "Wrong Code";
        }
        public ActionResult TwoFactorAuthenticated()
        { 
            return RedirectToAction("UserProfile");
        }
        public ActionResult Logoff()
        {
            Session["UserName"] = null;
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
            
            return RedirectToAction("Login");
        }
    }
}
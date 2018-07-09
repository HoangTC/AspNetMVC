using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project1.DAL;
using Project1.Models;
using Project1.Configuration;
using System.Data.Entity.Validation;
using System.Web.Security;

namespace Project1.Controllers
{
    public class AccountsController : Controller
    {
        private ManagerContext db = new ManagerContext();
        private EnCodeMD5 encode = new EnCodeMD5();
        private Email email = new Email();
        private Validation valid = new Validation();

        [Authorize]
        public ActionResult Index()
        {
            var accounts = db.Users;
            return View(accounts.ToList());
        }

        public ActionResult Register()
        {
            if (Session["AccountId"] != null)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "Username,Password,RPassword,Email")] User account)
        {
            try
            {
                var vali = valid.ValidUserName(account.Username);
                if (vali != "")
                    ModelState.AddModelError("UserName", vali);
                vali = valid.ValidEmail(account.Email);
                if (vali != "")
                    ModelState.AddModelError("Email", vali);
                if (account.Password != "" && !account.Password.Equals(account.RPassword))
                    ModelState.AddModelError("RPassword", "Xác nhận mật khẩu không đúng.");
                if (ModelState.IsValid)
                {
                    //Gui mail xac nhan email dang ky
                    Guid activationCode = Guid.NewGuid();
                    if (email.SendMailAccount(account.Username, account.Email, activationCode.ToString()))
                    {
                        account.ActivationCode = activationCode.ToString();
                        account.TimeGetCode = DateTime.UtcNow;
                        account.Password = encode.EncodeMd(account.Password);
                        db.Users.Add(account);
                        db.SaveChanges();
                        account.Password = null;
                        account.RPassword = null;
                        ViewBag.Message = "Đăng ký thành công. Mã xác nhận đã được gửi đến email của bạn.";
                    }
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
            return View(account);
        }
        
        public ActionResult Login()
        {
            //string[] arr = { "One", "Two", "Three" };
            //var target = "Onee";
            //var results = Array.FindAll(arr, s => s.Equals(target));
            //bool a = Array.Exists(arr, element => element == "One");

            if (Session["AccountId"] != null)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "Username,Password")] User account)
        {
            if (ModelState.IsValid)
            {
                var us = db.Users.FirstOrDefault(u => u.Username == account.Username);
                if (us != null)
                {
                    if (DateTime.Compare(us.TimeCountLogin, DateTime.UtcNow.AddMinutes(-3)) <= 0)
                    {
                        us.CountLogin = 0;
                        us.TimeCountLogin = DateTime.UtcNow;
                        db.SaveChanges();
                    }
                    if (us.CountLogin < 3)
                    {
                        string pass = encode.EncodeMd(account.Password);
                        var user = db.Users.Where(u => u.Username == account.Username && u.Password == pass).FirstOrDefault();
                        if (user != null)
                        {
                            if(user.ConfirmActivity == true)
                            {
                                user.Password = pass;
                                user.CountLogin = 0;
                                db.SaveChanges();
                                Session["AccountId"] = user.Id;
                                FormsAuthentication.SetAuthCookie(user.Username, true);

                                if(Array.Exists(user.Roles.Select(r => r.Name).ToArray(), r => r == "SuperAdmin"))
                                    Session["RoleId"] = 1;
                                else if (Array.Exists(user.Roles.Select(r => r.Name).ToArray(), r => r == "Admins"))
                                    Session["RoleId"] = 2;
                                else
                                    Session["RoleId"] = 3;

                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                ViewBag.Error = "Tài khoản chưa được kích hoạt, ";
                            }
                        }
                        else
                        {
                            us.CountLogin++;
                            if (us.CountLogin == 3)
                            {
                                us.TimeCountLogin = DateTime.UtcNow;
                            }
                            db.SaveChanges();
                            ViewBag.Message = "Mật khẩu không chính xác, vui lòng nhập lại.";
                        }
                    }
                    else
                        ViewBag.Message = "Bạn đã đăng nhập quá 3 lần, vui lòng chờ 3 phút.";
                }
                else
                    ViewBag.Message = "Tên đăng nhập không tồn tại.";
            }
            return View(account);
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string Username, string Email)
        {
            var mail = db.Users.FirstOrDefault(a => a.Email == Email && a.Username == Username);
            if (mail != null)
            {
                var gettime = DateTime.UtcNow;
                Guid activationCode = Guid.NewGuid();
                if (email.SendMailPassword(Username, Email, activationCode.ToString()))
                {
                    mail.ActivationCode = activationCode.ToString();
                    mail.TimeGetCode = gettime;
                    db.SaveChanges();
                    ViewBag.Status = "Mã xác nhận đã gửi đến mail của bạn.";
                }
            }
            else
                ViewBag.Status = "Tài khoản hoặc Email không chính xác.";
            return View();
        }

        [HttpGet]
        public ActionResult ActivateAccount()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ActivateAccount(string Username, string Email)
        {
            var mail = db.Users.FirstOrDefault(a => a.Email == Email && a.Username == Username);
            if (mail != null)
            {
                var gettime = DateTime.UtcNow;
                Guid activationCode = Guid.NewGuid();
                if (email.SendMailAccount(Username, Email, activationCode.ToString()))
                {
                    mail.ActivationCode = activationCode.ToString();
                    mail.TimeGetCode = gettime;
                    db.SaveChanges();
                    ViewBag.Status = "Mã xác nhận tài khoản đã được đến email của bạn.";
                }
            }
            else
                ViewBag.Status = "Tài khoản hoặc Email không chính xác.";
            return View();
        }

        public ActionResult ConfirmAcc()
        {
            ViewBag.Message = "Lỗi xác nhận.";
            if (RouteData.Values["id"] != null)
            {
                Guid activationCode = new Guid(RouteData.Values["id"].ToString());
                var user = db.Users.FirstOrDefault(e => e.ActivationCode == activationCode.ToString());
                if (user != null)
                {
                    user.ActivationCode = null;
                    if (DateTime.Compare(user.TimeGetCode, DateTime.UtcNow.AddMinutes(-5)) >= 0)
                    {
                        user.ConfirmActivity = true;
                        ViewBag.Message = "Xác nhận thành công.";
                    }
                    db.SaveChanges();
                }
            }
            return View();
        }

        public ActionResult ConfirmPass()
        {
            ViewBag.Message = "Lỗi xác nhận.";
            if (RouteData.Values["id"] != null)
            {
                Guid activationCode = new Guid(RouteData.Values["id"].ToString());
                var user = db.Users.FirstOrDefault(e => e.ActivationCode == activationCode.ToString());
                if (user != null)
                {
                    if (DateTime.Compare(user.TimeGetCode, DateTime.UtcNow.AddMinutes(-5)) >= 0)
                    {
                        ViewBag.Id = user.Id;
                        ViewBag.Message = "Xác nhận thành công.";
                    }
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult ConfirmPass(int AccountId, string Password, string RPassword)
        {
            if (Password != "" && !Password.Equals(RPassword))
                ModelState.AddModelError("RPassword", "Password Mismatch");

            if (ModelState.IsValid)
            {
                var user = db.Users.FirstOrDefault(u => u.Id == AccountId);
                if (user != null)
                {
                    user.ActivationCode = null;
                    if (DateTime.Compare(user.TimeGetCode, DateTime.UtcNow.AddMinutes(-5)) < 0)
                    {
                        db.SaveChanges();
                        ViewBag.Message = "Lỗi xác nhận.";
                        return View();
                    }
                    user.CountLogin = 0;
                    user.Password = encode.EncodeMd(Password);
                    db.SaveChanges();
                    ViewBag.Message = "Xác nhận thành công.";
                    return View();
                }

            }
            ViewBag.Id = AccountId;
            var account = new User();
            account.Password = Password;
            return View(account);
        }

        public ActionResult Logoff()
        {
            Session["AccountId"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Accounts");
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

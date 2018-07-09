using Project1.DAL;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Project1.Configuration
{
    public class Validation : Controller
    {
        private ManagerContext db = new ManagerContext();
        public string ValidUserName(string UserName)
        {
            if (db.Users.Any(u => u.Username == UserName))
            {
                return "Tên tài khoản đã tồn tại.";
            }
            return "";
        }

        public string ValidEmail(string Email)
        {
            if (db.Users.Any(u => u.Email == Email))
            {
                return "Email đã được sử dụng.";
            }
            return "";
        }
        public JsonResult ValidateUsername(string Username)
        {
            if (db.Users.Any(u => u.Username == Username))
            {
                return Json(string.Format("Tên tài khoản '{0}' đã được sử dụng.", Username), JsonRequestBehavior.AllowGet);
            }
            else if (string.IsNullOrEmpty(Username))
            {
                return Json(string.Format("Tài khoản không được đ"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                string nameRegex = @"^[a-zA-Z0-9_]{5,255}$";
            Regex re = new Regex(nameRegex);
            if (!re.IsMatch(Username))
            {
                return Json(string.Format("Định dạng tài khoản không chính xác."), JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ValidateEmailAddress(string Email)
        {
            if (db.Users.Any(u => u.Email == Email))
            {
                return Json(string.Format("Email '{0}' đã được sử dụng.", Email), JsonRequestBehavior.AllowGet);
            }
            else if (string.IsNullOrEmpty(Email))
            {
                return Json(string.Format("Không được để trống"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                string nameRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                         @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                            @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
                Regex re = new Regex(nameRegex);
                if (!re.IsMatch(Email))
                {
                    return Json(string.Format("Định dạng Email không đúng"), JsonRequestBehavior.AllowGet);
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
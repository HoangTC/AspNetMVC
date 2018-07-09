using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace Project1.Configuration
{
    public class Email
    {
        public void SendMail(string Email, string Body)
        {
            var message = new MailMessage();
            message.To.Add(new MailAddress(Email));
            message.Subject = "Confirm Account";
            message.Body = Body;
            message.IsBodyHtml = true;
            using (var smtp = new SmtpClient())
            {
                smtp.Send(message);
            }
        }
        public bool SendMailPassword(string Username, string Email, string activationCode)
        {
            try
            {
                //string body = System.IO.File.ReadAllText(@"~\App_Data\Content.txt");
                string body =  System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/App_Data/Content.txt"));
                body = string.Format(body, Username, string.Format("{0}://{1}/Accounts/ConfirmPass/{2}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority, activationCode));
                SendMail(Email, body);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SendMailAccount(string Username, string Email, string activationCode)
        {
            try
            {
                //string body = System.IO.File.ReadAllText(@"~\App_Data\ActivateAccount.txt");
                string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/App_Data/ActivateAccount.txt"));
                body = string.Format(body, Username, string.Format("{0}://{1}/Accounts/ConfirmAcc/{2}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority, activationCode));
                SendMail(Email, body);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
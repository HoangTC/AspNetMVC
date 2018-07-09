using Project1.Configuration;
using Project1.DAL;
using Project1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Project1.Controllers
{
    public class HomeController : Controller
    {
        private ManagerContext db = new ManagerContext();

        //[Authorize]
        //[CustomAuthorize]
        public ActionResult Index()
        {
            if (Session["AccountId"] == null)
                Response.StatusCode = 500;// (int)HttpStatusCode.InternalServerError;
                //Response.StatusCode = (int)HttpStatusCode.BadRequest;
            else
                Response.StatusCode = (int)HttpStatusCode.OK;
            ViewBag.UserId = new SelectList(db.Users, "Id", "Username");
            ViewBag.RoleId = new SelectList(db.Roles, "Id", "Name");
            return View();
        }
    }
}
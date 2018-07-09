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

namespace Project1.Controllers
{
    [Authorize]
    public class RolesController : Controller
    {
        private ManagerContext db = new ManagerContext();

        [Authorize(Roles = CustomPermission.ShowRole)]
        public ActionResult Index()
        {
            return View(db.Roles.ToList());
        }

        [Authorize(Roles = CustomPermission.DetailsRole)]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Role role = db.Roles.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        [Authorize(Roles = CustomPermission.AddRole)]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = CustomPermission.AddRole)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Role role)// tao moi 1 role
        {
            if (ModelState.IsValid)
            {
                db.Roles.Add(role);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(role);
        }

        [Authorize(Roles = CustomPermission.AddPermissionIntoRole)]
        public ActionResult Add(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Role role = db.Roles.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            ViewBag.LstPermission = role.Permissions;
            ViewBag.RoleName = role.Name;
            ViewBag.RoleId = role.Id;
            ViewBag.PermissionId = new SelectList(db.Permissions, "Id", "Name");
            return View(role);
        }

        [Authorize(Roles = CustomPermission.AddPermissionIntoRole)]
        [HttpPost]
        public ActionResult Add(int permissionId)// add 1 permission moi cho role
        {
            int roleId = Int32.Parse(RouteData.Values["id"].ToString());
            if (roleId == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permission permission = db.Permissions.Find(permissionId);
            db.Permissions.Attach(permission);

            Role role = db.Roles.Find(roleId);
            db.Roles.Attach(role);

            role.Permissions.Add(permission);

            db.SaveChanges();

            ViewBag.RoleName = role.Name;
            ViewBag.RoleId = role.Id;
            ViewBag.LstPermission = db.Roles.Find(roleId).Permissions;
            ViewBag.PermissionId = new SelectList(db.Permissions, "Id", "Name");
            return View();
        }


        [Authorize(Roles = CustomPermission.EditRole)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Role role = db.Roles.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        [Authorize(Roles = CustomPermission.EditRole)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Role role)
        {
            if (ModelState.IsValid)
            {
                db.Entry(role).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(role);
        }


        [Authorize(Roles = CustomPermission.DeletePermissionRole)]
        [HttpGet]
        public ActionResult DeletePermissionInRole(int roleId, int permissionId)//xóa permission trong role
        {
            Permission permission = db.Permissions.Find(permissionId);
            db.Permissions.Attach(permission);

            Role role = db.Roles.Find(roleId);
            db.Roles.Attach(role);

            permission.Roles.Remove(role);
            db.SaveChanges();
            return RedirectToAction("Add", "Roles", new { id = roleId });
        }

        [Authorize(Roles = CustomPermission.DeleteRole)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Role role = db.Roles.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        [Authorize(Roles = CustomPermission.DeleteRole)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Role role = db.Roles.Find(id);
            db.Roles.Remove(role);
            db.SaveChanges();
            return RedirectToAction("Index");
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

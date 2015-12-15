using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using BugTracker.Models;

namespace BugTracker.Controllers
{
    [RequireHttps]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        // GET: List Users
        [Authorize(Roles="Admin")]
        public ActionResult ListUsers()
        {
            return View(db.Users.ToList());
        }

        //GET: Edit User Roles
        [Authorize(Roles = "Admin")]
        public ActionResult EditUserRoles(string id)
        {
            var user = db.Users.Find(id);
            AdminUserViewModel AdminModel = new AdminUserViewModel();
            UserRolesHelper helper = new UserRolesHelper();
            var selected = helper.ListUserRoles(id);
            AdminModel.Roles = new MultiSelectList(db.Roles, "Name", "Name", selected);
            AdminModel.User = user;

            return View(AdminModel);
        }

        //POST: Edit User Roles
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditUserRoles(string id, AdminUserViewModel model)
        {
            UserRolesHelper helper = new UserRolesHelper();
            if (ModelState.IsValid)
            {
                string[] empt = { };
                model.SelectedRoles = model.SelectedRoles ?? empt;

                foreach (var role in db.Roles)
                {
                    if (model.SelectedRoles.Contains(role.Name))
                    {
                        helper.AddUserToRole(id, role.Name);
                    }
                    else
                    {
                        helper.RemoveUserFromRole(id, role.Name);
                    }
                }
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        
    }
}
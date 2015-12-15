using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using BugTracker.Models;

namespace BugTracker.Controllers
{
    [RequireHttps]
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        [Authorize(Roles = "Admin, Project Manager, Developer")]
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return View(db.Projects.ToList());
            }
            else if (User.IsInRole("Project Manager") || User.IsInRole("Developer"))
            {
                var user = User.Identity.GetUserId();
                //var projects = db.Projects.Where(p => p.ProjectUsers.Contains(db.Users.Find(user))).ToList();
                var userProjects = new List<Project>();
                var projects = db.Projects.ToList();
                foreach (var project in projects)
                {
                    foreach (var u in project.ProjectUsers)
                    {
                        if (u.Id == user)
                        {
                            userProjects.Add(project);
                        }
                    }
                }
                return View(userProjects);
            }
            else
            {
                return View();
            }
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            project.Tickets = db.Tickets.Where(t => t.ProjectId == project.Id).ToList();
            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles="Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Id,Name")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        //GET: Edit Project Users
        public ActionResult EditProjectUsers(int projectId)
        {
            var project = db.Projects.Find(projectId);
            ProjectUsers projectModel = new ProjectUsers();
            UserProjectsHelper helper = new UserProjectsHelper();
            UserRolesHelper rolesHelper = new UserRolesHelper();

            var selected = helper.UsersInProject(projectId).Select(i => i.Id);
            var userRole = rolesHelper.UsersInRole("Developer");

            if (User.IsInRole("Admin"))
            {
                projectModel.Users = new MultiSelectList(db.Users, "Id", "DisplayName", selected);
            }
            else if (User.IsInRole("Project Manager"))
            {
                projectModel.Users = new MultiSelectList(userRole, "Id", "DisplayName", selected);
            }
            projectModel.Project = project;

            return View(projectModel);
        }

        //POST: Edit Project Users
        [HttpPost]
        public ActionResult EditProjectUsers(int projectId, ProjectUsers model)
        {
            UserProjectsHelper helper = new UserProjectsHelper();
            var project = db.Projects.Find(projectId);
            if (ModelState.IsValid)
            {
                string[] empt = { };
                model.SelectedUsers = model.SelectedUsers ?? empt;

                foreach (var user in db.Users)
                {
                    if (model.SelectedUsers.Contains(user.Id))
                    {
                        helper.AddUserToProject(user.Id, projectId);
                        //user.Projects.Add(project);
                    }
                    else
                    {
                        helper.RemoveUserFromProject(user.Id, projectId);
                        //user.Projects.Remove(project);
                    }
                }
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,Name")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
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

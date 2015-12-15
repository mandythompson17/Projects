using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Budgeter.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;

namespace Budgeter.Controllers
{
    [RequireHttps]
    [Authorize]
    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Households
        [AuthorizeHouseholdRequired]
        public ActionResult Index()
        {
            return View(db.Households.ToList());
        }

        // GET: Households/Details/5
        [AuthorizeHouseholdRequired]
        public ActionResult Details()
        {
            var id = Convert.ToInt32(User.Identity.GetHouseholdId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // GET: Households/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Households/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name")] Household household)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                db.Households.Add(household);
                user.HouseholdId = household.Id;
                user.Household = household;
                household.Members.Add(user);
                db.SaveChanges();

                var hName = db.Households.Where(hn => hn.Name == household.Name).FirstOrDefault();
                var hhid = hName.Id;

                HouseholdsController.DefaultCategories("Salary", hhid);
                HouseholdsController.DefaultCategories("Apparel", hhid);
                HouseholdsController.DefaultCategories("Bills", hhid);
                HouseholdsController.DefaultCategories("Gas", hhid);
                HouseholdsController.DefaultCategories("Grocery", hhid);
                HouseholdsController.DefaultCategories("Miscellaneous", hhid);
                await ControllerContext.HttpContext.RefreshAuthentication(user);
                return RedirectToAction("Details");
            }

            return View(household);
        }

        // POST: Join Household
        [HttpPost]
        [AuthorizeHouseholdRequired]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Join(string code)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                var invite = db.Invitations.First(i => i.Code == code);
                if (invite != null && user.Email == invite.Email)
                {
                    var household = db.Households.Find(invite.HouseholdId);
                    household.Members.Add(user);
                    user.HouseholdId = household.Id;
                    user.Household = household;
                    db.SaveChanges();
                    await ControllerContext.HttpContext.RefreshAuthentication(user);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Join Code or Email does not match invitation.");
                    return RedirectToAction("Create");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Leave Household
        [HttpPost]
        [AuthorizeHouseholdRequired]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Leave(int id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var household = db.Households.Find(id);
            user.HouseholdId = null;
            user.Household = null;
            household.Members.Remove(user);
            db.SaveChanges();
            await ControllerContext.HttpContext.RefreshAuthentication(user);
            return RedirectToAction("Create");

        }

        // GET: Households/Edit/5
        [AuthorizeHouseholdRequired]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [AuthorizeHouseholdRequired]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Household household)
        {
            if (ModelState.IsValid)
            {
                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = household.Id });
            }
            return View(household);
        }

        //POST: Households/Invite
        [HttpPost]
        [AuthorizeHouseholdRequired]
        [ValidateAntiForgeryToken]
        public ActionResult Invite([Bind(Include = "Email,HouseholdId")] Invitation invite)
        {
            invite.Household = db.Households.Find(invite.HouseholdId);
            // get Random Code
            int length = 5;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            string code = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
            invite.Code = code;
            invite.InviterId = User.Identity.GetUserId();
            invite.Inviter = db.Users.Find(User.Identity.GetUserId());
            db.Invitations.Add(invite);
            db.SaveChanges();
            var callback = Url.Action("Index", "Home", null, protocol: Request.Url.Scheme);
            var inviter = db.Users.Find(invite.InviterId);
            EmailService es = new EmailService();
            IdentityMessage message = new IdentityMessage {
                Destination = invite.Email,
                Subject = "You've been invited to join the " + invite.Household.Name + " Household on Thompson Budgeter",
                Body = "You've been invited by " + inviter.DisplayName + " to join the " + invite.Household.Name + " Household on Thompson Budgeter!<br> Login or register and use this code: <strong>"
                    + invite.Code + "</strong> on the Create/Join Household Page.<br>Click <a href=\"" + callback + "\">here</a> to accept the invitation."};
            es.SendAsync(message);
            return RedirectToAction("Details", new { id = invite.HouseholdId });
        }

        //// GET: Households/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Household household = db.Households.Find(id);
        //    if (household == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(household);
        //}

        //// POST: Households/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[AuthorizeHouseholdRequired]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Household household = db.Households.Find(id);
        //    db.Households.Remove(household);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        public static void DefaultCategories(string catName, int hhId)
        {
            Category category = new Category();
            category.Name = catName;
            category.HouseholdId = hhId;

            ApplicationDbContext db = new ApplicationDbContext();
            db.Categories.Add(category);
            db.SaveChanges();
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Budgeter.Models;

namespace Budgeter.Controllers
{
    [RequireHttps]
    [AuthorizeHouseholdRequired]
    public class CategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Categories
        public ActionResult Index()
        {
            var HId = Convert.ToInt32(User.Identity.GetHouseholdId());
            //var HBudgets = db.Budgets.Where(b => b.HouseholdId == HId).ToList();
            //var HTrans = db.Transactions.Where(t => t.Account.HouseholdId == HId).ToList();
            //var HCats = new List<Category>();
            //foreach (var bItem in HBudgets)
            //{
            //    var cat = db.Categories.Find(bItem.CategoryId);
            //    if (!HCats.Any(c => c.Id == cat.Id))
            //    {
            //        HCats.Add(cat);
            //    }
            //}
            //foreach (var trans in HTrans)
            //{
            //    var cat = db.Categories.Find(trans.CategoryId);
            //    if (!HCats.Any(c => c.Id == cat.Id))
            //    {
            //        HCats.Add(cat);
            //    }
            //}
            var HCats = db.Categories.Where(c => c.HouseholdId == HId).ToList().OrderBy(c => c.Name);
            foreach (var cat in HCats)
            {
                cat.Transactions = db.Transactions.Where(t => t.CategoryId == cat.Id && t.Account.HouseholdId == HId).ToList();
            }
            return View(HCats);
        }

        // GET: Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        //// GET: Categories/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        // GET: Categories/_Create
        public PartialViewResult _Create()
        {
            return PartialView();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,HouseholdId,IsExpense,IsDeleted")] Category category)
        {
            if (ModelState.IsValid)
            {
                var HId = Convert.ToInt32(User.Identity.GetHouseholdId());
                var household = db.Households.Find(HId);
                category.HouseholdId = HId;
                category.Household = household;
                category.IsDeleted = false;
                db.Categories.Add(category);
                household.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index", "Budgets");
            }

            return View(category);
        }

        // GET: Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,HouseholdId,Household,IsExpense,IsDeleted")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            //db.Categories.Remove(category);
            category.IsDeleted = true;
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

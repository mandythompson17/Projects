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
    public class BudgetsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Budgets
        public ActionResult Index()
        {
            var HId = Convert.ToInt32(User.Identity.GetHouseholdId());
            var HCats = db.Categories.Where(c => c.HouseholdId == HId).ToList();
            var month = DateTime.Now.Month;
            foreach (var cat in HCats)
            {
                if (cat.BudgetItems.Count == 0)
                {
                    Budget zero = new Budget
                    {
                        HouseholdId = HId,
                        CategoryId = cat.Id,
                        Category = cat,
                        Amount = 0,
                        Spent = 0,
                        IsOver = false
                    };
                    //var month = DateTime.Now.Month;
                    var transactions = db.Transactions.Where(t => t.CategoryId == cat.Id && t.IsDeleted == false && t.Date.Month == month).ToList();
                    foreach (var trans in transactions)
                    {
                        zero.Spent += trans.Amount;
                    }
                    db.Budgets.Add(zero);
                    cat.BudgetItems.Add(zero);
                }
            }
            db.SaveChanges();
            var budgets = db.Budgets.Where(b => b.HouseholdId == HId).Include(b => b.Category).ToList();
            foreach (var budget in budgets)
            {
                budget.Spent = (from t in db.Transactions
                                where t.CategoryId == budget.CategoryId && t.IsDeleted == false && t.Date.Month == month
                                select t.Amount).DefaultIfEmpty().Sum();
                if (budget.Category.IsExpense)
                {
                    if (budget.Amount + budget.Spent < 0)
                    {
                        budget.IsOver = true;
                    }
                    else
                    {
                        budget.IsOver = false;
                    }
                }
                else
                {
                    if (budget.Amount - budget.Spent < 0)
                    {
                        budget.IsOver = true;
                    }
                    else
                    {
                        budget.IsOver = false;
                    }
                }


            }
            db.SaveChanges();
            return View(budgets);
        }

        // GET: Budgets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = db.Budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }
            return View(budget);
        }

        //// GET: Budgets/Create
        //public ActionResult Create()
        //{
        //    ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
        //    return View();
        //}

        // GET: Budgets/_Create
        public PartialViewResult _Create()
        {
            var HId = Convert.ToInt32(User.Identity.GetHouseholdId());
            var cats = db.Categories.Where(c => c.HouseholdId == HId);
            ViewBag.CategoryId = new SelectList(cats, "Id", "Name");
            return PartialView();
        }

        // POST: Budgets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,HouseholdId,CategoryId,Amount,Spent,IsOver")] Budget budget)
        {
            var HId = Convert.ToInt32(User.Identity.GetHouseholdId());
            if (ModelState.IsValid)
            {
                budget.HouseholdId = HId;
                budget.Household = db.Households.Find(HId);
                var transactions = db.Transactions.Where(t => t.CategoryId == budget.CategoryId && t.Account.HouseholdId == HId && t.IsDeleted == false).ToList();
                foreach (var trans in transactions)
                {
                    budget.Spent += trans.Amount;
                }
                if (budget.Amount + budget.Spent < 0)
                {
                    budget.IsOver = true;
                }
                else
                {
                    budget.IsOver = false;
                }
                db.Budgets.Add(budget);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var cats = db.Categories.Where(c => c.HouseholdId == HId);
            ViewBag.CategoryId = new SelectList(cats, "Id", "Name", budget.CategoryId);
            return View(budget);
        }

        // GET: Budgets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = db.Budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", budget.CategoryId);
            return View(budget);
        }

        // POST: Budgets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,HouseholdId,CategoryId,Amount,Spent,IsOver")] Budget budget)
        {
            if (ModelState.IsValid)
            {
                var HId = Convert.ToInt32(User.Identity.GetHouseholdId());
                var transactions = db.Transactions.Where(t => t.CategoryId == budget.CategoryId && t.Account.HouseholdId == HId).ToList();
                foreach (var trans in transactions)
                {
                    budget.Spent += trans.Amount;
                }
                if (budget.Spent > budget.Amount)
                {
                    budget.IsOver = true;
                }
                else
                {
                    budget.IsOver = false;
                }
                db.Entry(budget).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", budget.CategoryId);
            return View(budget);
        }

        // GET: Budgets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = db.Budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }
            return View(budget);
        }

        // POST: Budgets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Budget budget = db.Budgets.Find(id);
            db.Budgets.Remove(budget);
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

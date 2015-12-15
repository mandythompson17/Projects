using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Budgeter.Models;

namespace Budgeter.Controllers
{
    [RequireHttps]
    [AuthorizeHouseholdRequired]
    public class BankAccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BankAccounts
        public ActionResult Index()
        {
            var HId = Convert.ToInt32(User.Identity.GetHouseholdId());
            var bankAccounts = db.BankAccounts.Where(b => b.IsDeleted == false && b.HouseholdId == HId).Include(b => b.Household);
            return View(bankAccounts.ToList());
        }

        // GET: BankAccounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            return View(bankAccount);
        }

        // GET: BankAccounts/_Create
        public PartialViewResult _Create()
        {
            var HId = Convert.ToInt32(User.Identity.GetHouseholdId());
            var members = db.Users.Where(u => u.HouseholdId == HId);
            ViewBag.OwnerId = new SelectList(members, "Id", "DisplayName");
            return PartialView();
        }

        // POST: BankAccounts/_Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Balance,DateOpened,IsDeleted,OwnerId")] BankAccount bankAccount)
        {
            var HId = Convert.ToInt32(User.Identity.GetHouseholdId());
            if (ModelState.IsValid)
            {
               
                var household = db.Households.Find(HId);
                bankAccount.HouseholdId = HId;
                bankAccount.Owner = db.Users.Find(bankAccount.OwnerId);
                bankAccount.DateOpened = System.DateTimeOffset.Now;
                bankAccount.IsDeleted = false;
                db.BankAccounts.Add(bankAccount);

                household.Accounts.Add(bankAccount);
                db.SaveChanges();
            }

            var members = db.Users.Where(u => u.HouseholdId == HId);
            ViewBag.OwnerId = new SelectList(members, "Id", "DisplayName", bankAccount.OwnerId);

            return RedirectToAction("Details", "Households");
        }

        // GET: BankAccounts/Create
        //public ActionResult Create()
        //{
        //    //ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name");
        //    return View();
        //}

        //// POST: BankAccounts/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,Name,HouseholdId,Balance")] BankAccount bankAccount)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.BankAccounts.Add(bankAccount);
        //        var household = db.Households.Find(bankAccount.HouseholdId);
        //        household.Accounts.Add(bankAccount);
        //        db.SaveChanges();
        //        return View();
        //    }

        //    //ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", bankAccount.HouseholdId);
        //    return View(bankAccount);
        //}

        // GET: BankAccounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            var HId = Convert.ToInt32(User.Identity.GetHouseholdId());
            var members = db.Users.Where(u => u.HouseholdId == HId);
            ViewBag.OwnerId = new SelectList(members, "Id", "DisplayName");
            //ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", bankAccount.HouseholdId);
            return View(bankAccount);
        }

        // POST: BankAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,HouseholdId,Balance,OwnerId,DateOpened")] BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                bankAccount.Owner = db.Users.Find(bankAccount.OwnerId);
                db.Entry(bankAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Households");
            }
            var members = db.Users.Where(u => u.HouseholdId == bankAccount.HouseholdId);
            ViewBag.OwnerId = new SelectList(members, "Id", "DisplayName", bankAccount.OwnerId);
            //ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", bankAccount.HouseholdId);
            return View(bankAccount);
        }

        // GET: BankAccounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            return View(bankAccount);
        }

        // POST: BankAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BankAccount bankAccount = db.BankAccounts.Find(id);
            bankAccount.IsDeleted = true;
            foreach (var transaction in bankAccount.Transactions)
            {
                transaction.IsDeleted = true;
            }
            //household.Accounts.Remove(bankAccount);
            ////db.BankAccounts.Remove(bankAccount);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //POST: BankAccounts/Reconcile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reconcile(int id, decimal recBalance)
        {
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount.Balance != recBalance)
            {
                var dif = recBalance - bankAccount.Balance;
                Transaction equate = new Transaction {
                    BankAccountId = bankAccount.Id,
                    Account = bankAccount,
                    Amount = dif,
                    Date = System.DateTimeOffset.Now,
                    Description = "Reconciliation",
                    UserId = User.Identity.GetUserId(),
                    IsDeleted = false
                };
                equate.User = db.Users.Find(equate.UserId);
                if (equate.Amount <= 0)
                {
                    equate.IsWithdrawal = true;
                }
                else
                {
                    equate.IsWithdrawal = false;
                }
                bankAccount.Balance += equate.Amount;
                db.Transactions.Add(equate);
                bankAccount.Transactions.Add(equate);
                db.SaveChanges();
            }
            return RedirectToAction("Details", new { id = bankAccount.Id });
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

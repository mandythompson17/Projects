﻿using System;
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
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index()
        {
            var transactions = db.Transactions.Include(t => t.Account).Include(t => t.Category).Include(t => t.User);
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public PartialViewResult _Create(int? id)
        {
            //ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "Name");
            var HId = Convert.ToInt32(User.Identity.GetHouseholdId());
            var cats = db.Categories.Where(c => c.HouseholdId == HId);
            ViewBag.CategoryId = new SelectList(cats, "Id", "Name");
            var transaction = new Transaction();
            transaction.BankAccountId = Convert.ToInt32(id);
            return PartialView(transaction);
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BankAccountId,UserId,CategoryId,Category,Date,Amount,IsWithdrawal,Description,IsDeleted")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                transaction.UserId = User.Identity.GetUserId();
                transaction.User = db.Users.Find(transaction.UserId);
                transaction.Category = db.Categories.Find(transaction.CategoryId);
                transaction.Category.Transactions.Add(transaction);
                if (transaction.Amount <= 0)
                {
                    transaction.IsWithdrawal = true;
                }
                else
                {
                    transaction.IsWithdrawal = false;
                }
                transaction.IsDeleted = false;
                transaction.Date = System.DateTimeOffset.Now;
                var account = db.BankAccounts.Find(transaction.BankAccountId);
                transaction.Account = account;
                transaction.Account.Balance += transaction.Amount;
                var budget = db.Budgets.FirstOrDefault(b => b.CategoryId == transaction.CategoryId);
                if (budget != null)
                {
                    budget.Spent += transaction.Amount;
                    if (budget.Amount + budget.Spent < 0)
                    {
                        budget.IsOver = true;
                    }
                    else
                    {
                        budget.IsOver = false;
                    }
                }
                db.Transactions.Add(transaction);
                account.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Details", "BankAccounts", new { id = transaction.BankAccountId });
            }

            //ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "Name", transaction.BankAccountId);
            var HId = Convert.ToInt32(User.Identity.GetHouseholdId());
            var cats = db.Categories.Where(c => c.HouseholdId == HId);
            ViewBag.CategoryId = new SelectList(cats, "Id", "Name", transaction.CategoryId);
            //ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", transaction.UserId);
            return View(transaction);
        }

        //// GET: Transactions/Create
        //public ActionResult Create()
        //{
        //    ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "Name");
        //    ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
        //    return View();
        //}

        //// POST: Transactions/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,BankAccountId,UserId,CategoryId,Category,Date,Amount,ReconciliationAmount,IsWithdrawal,Description,IsReconciled,IsDeleted")] Transaction transaction)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        transaction.UserId = User.Identity.GetUserId();
        //        transaction.User = db.Users.Find(transaction.UserId);
        //        if (transaction.Amount <= 0)
        //        {
        //            transaction.IsWithdrawal = true;
        //        }
        //        else
        //        {
        //            transaction.IsWithdrawal = false;
        //        }
        //        if (transaction.ReconciliationAmount > 0)
        //        {
        //            transaction.IsReconciled = false;
        //        }
        //        else
        //        {
        //            transaction.IsReconciled = true;
        //        }
        //        transaction.IsDeleted = false;
        //        transaction.Date = System.DateTimeOffset.Now;
        //        db.Transactions.Add(transaction);
        //        var account = db.BankAccounts.Find(transaction.BankAccountId);
        //        account.Transactions.Add(transaction);
        //        db.SaveChanges();
        //        return RedirectToAction("Details", "BankAccounts", new { id = transaction.BankAccountId });
        //    }

        //    //ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "Name", transaction.BankAccountId);
        //    ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
        //    //ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", transaction.UserId);
        //    return View(transaction);
        //}

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            var HId = Convert.ToInt32(User.Identity.GetHouseholdId());
            var accounts = db.BankAccounts.Where(a => a.HouseholdId == HId && a.IsDeleted == false);
            ViewBag.BankAccountId = new SelectList(accounts, "Id", "Name", transaction.BankAccountId);
            var cats = db.Categories.Where(c => c.HouseholdId == HId);
            ViewBag.CategoryId = new SelectList(cats, "Id", "Name", transaction.CategoryId);
            //ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", transaction.UserId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BankAccountId,UserId,CategoryId,Date,Amount,IsWithdrawal,Description,IsDeleted")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var OldTransaction = db.Transactions.AsNoTracking().FirstOrDefault(t => t.Id == transaction.Id);
                var account = db.BankAccounts.Find(transaction.BankAccountId);
                if (OldTransaction.BankAccountId != transaction.BankAccountId)
                {
                    var oldAccount = db.BankAccounts.Find(OldTransaction.BankAccountId);
                    oldAccount.Balance -= OldTransaction.Amount;
                    oldAccount.Transactions.Remove(transaction);
                    account.Transactions.Add(transaction);
                    account.Balance += transaction.Amount;
                }
                else
                {
                    if (OldTransaction.Amount != transaction.Amount)
                    {
                        account.Balance -= OldTransaction.Amount;
                        account.Balance += transaction.Amount;
                        //var dif = OldTransaction.Amount - transaction.Amount;
                        //account.Balance += dif;
                        var budget = db.Budgets.FirstOrDefault(b => b.CategoryId == transaction.CategoryId);
                        budget.Spent -= OldTransaction.Amount;
                        budget.Spent += transaction.Amount;
                        if (budget.Amount + budget.Spent < 0)
                        {
                            budget.IsOver = true;
                        }
                        else
                        {
                            budget.IsOver = false;
                        }

                    }
                }
                if (transaction.Amount <= 0)
                {
                    transaction.IsWithdrawal = true;
                }
                else
                {
                    transaction.IsWithdrawal = false;
                }
                db.Entry(transaction).State = EntityState.Modified;
                db.Entry(account).Property("Balance").IsModified = true;
                db.SaveChanges();
                return RedirectToAction("Details", "BankAccounts", new { id = transaction.BankAccountId });
            }
            var HId = Convert.ToInt32(User.Identity.GetHouseholdId());
            var accounts = db.BankAccounts.Where(a => a.HouseholdId == HId && a.IsDeleted == false);
            ViewBag.BankAccountId = new SelectList(accounts, "Id", "Name", transaction.BankAccountId);
            var cats = db.Categories.Where(c => c.HouseholdId == HId);
            ViewBag.CategoryId = new SelectList(cats, "Id", "Name", transaction.CategoryId);
            //ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", transaction.UserId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            var account = db.BankAccounts.Find(transaction.BankAccountId);
            transaction.IsDeleted = true;
            account.Balance -= transaction.Amount;
            var budget = db.Budgets.FirstOrDefault(b => b.CategoryId == transaction.CategoryId);
            budget.Spent -= transaction.Amount;
            if (budget.Spent > budget.Amount)
            {
                budget.IsOver = true;
            }
            else
            {
                budget.IsOver = false;
            }
            //db.Transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Details", "BankAccounts", new { id = transaction.BankAccountId });
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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Budgeter.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;

namespace Budgeter.Controllers
{
    [RequireHttps]
    public class ChartsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Charts
        public ActionResult Index()
        {
            return View();
        }

        //GET: Spending by Category for This Month
        public ActionResult GetCatTransChart()
        {
            var HId = Convert.ToInt32(User.Identity.GetHouseholdId());
            var household = db.Households.Find(HId);
            var month = DateTime.Now.Month;

            household.Categories = db.Categories.Where(c => c.HouseholdId == HId && c.IsDeleted == false).ToList();

            var catData = (from cat in household.Categories
                           where cat.IsExpense == true
                           let sum = (from t in db.Transactions
                                      where t.CategoryId == cat.Id && t.Account.HouseholdId == HId && t.IsDeleted == false && t.Date.Month == month && t.IsWithdrawal == true
                                      select t.Amount).DefaultIfEmpty().Sum()
                           select new
                           {
                               label = cat.Name,
                               value = Math.Abs(sum)
                           }).ToArray();
            var colors = new string[] { "#0000CC", "#9900FF", "#00CC00" };
            var donut = new
            {
                cData = catData,
                chartColors = colors
            };
            return Content(JsonConvert.SerializeObject(donut), "application/json");
            //var s = new[] { new { label = "2008", value= 20 },
            //    new { label= "2008", value= 5 },
            //    new { label= "2010", value= 7 },
            //    new { label= "2011", value= 10 },
            //    new { label= "2012", value= 20 }};
            //return Content(JsonConvert.SerializeObject(s), "application/json");
        }

        //GET: Budget by Category
        public ActionResult GetBudgetCats()
        {
            var HId = Convert.ToInt32(User.Identity.GetHouseholdId());
            var household = db.Households.Find(HId);
            household.Categories = db.Categories.Where(c => c.HouseholdId == HId && c.IsDeleted == false).ToList();

            var catData = (from cat in household.Categories
                           let sum = (from b in db.Budgets
                                      where b.CategoryId == cat.Id && b.HouseholdId == HId
                                      select b.Amount).DefaultIfEmpty()
                           select new
                           {
                               label = cat.Name,
                               value = sum
                           }).ToArray();
            var colors = new string[] { "#0000CC", "#9900FF", "#00CC00" };
            var donut = new
            {
                cData = catData,
                chartColors = colors
            };
            return Content(JsonConvert.SerializeObject(donut), "application/json");

        }

        //GET: Category Budget vs Spent
        public ActionResult GetBudgvSpent()
        {
            var HId = Convert.ToInt32(User.Identity.GetHouseholdId());
            var household = db.Households.Find(HId);
            household.Categories = db.Categories.Where(c => c.HouseholdId == HId && c.IsDeleted == false).ToList();
            var month = DateTime.Now.Month;

            var catData = (from cat in household.Categories
                           where cat.IsExpense == true
                           let budget = (from b in db.Budgets
                                      where b.CategoryId == cat.Id
                                      select b.Amount).DefaultIfEmpty().Sum()
                           let spent = (from t in db.Transactions
                                      where t.CategoryId == cat.Id && t.IsDeleted == false && t.Date.Month == month && t.IsWithdrawal == true
                                      select t.Amount).DefaultIfEmpty().Sum()
                           select new
                           {
                               label = cat.Name,
                               b = budget,
                               s = Math.Abs(spent)
                           }).ToArray();
            var colors = new string[] {"#006600", "#FF0000"};
            var bar = new
            {
                cData = catData,
                chartColors = colors
            };
            return Content(JsonConvert.SerializeObject(bar), "application/json");
        }

        //GET: Income vs Expenses over month range
        public ActionResult MonthlyIncomevsExpense()
        {
            var HId = Convert.ToInt32(User.Identity.GetHouseholdId());
            var household = db.Households.Find(HId);
            var monthsToDate = Enumerable.Range(1, DateTime.Today.Month)
                    .Select(m => new DateTime(DateTime.Today.Year, m, 1))
                    .ToList();
            var sums = (from month in monthsToDate
                       select new
                       {
                           month = month.ToString("MMM"),
                           income = (from account in household.Accounts
                                     where account.IsDeleted == false
                                     from transaction in account.Transactions
                                     where transaction.IsWithdrawal == false && transaction.IsDeleted == false && transaction.Date.Month == month.Month
                                     select transaction.Amount).DefaultIfEmpty().Sum(),
                           expense = Math.Abs((from account in household.Accounts
                                      where account.IsDeleted == false
                                      from transaction in account.Transactions
                                      where transaction.IsWithdrawal == true && transaction.IsDeleted == false && transaction.Date.Month == month.Month
                                      select transaction.Amount).DefaultIfEmpty().Sum())
                       }).ToArray();
            var colors = new string[] { "#006600", "#FF0000" };
            var line = new
            {
                cData = sums,
                chartColors = colors
            };
            return Content(JsonConvert.SerializeObject(line), "application/json");
        }
    }
}
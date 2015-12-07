using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class Category
    {
        public Category() 
        {
            this.Transactions = new HashSet<Transaction>();
            this.BudgetItems = new HashSet<Budget>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? HouseholdId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<Budget> BudgetItems { get; set; }
        public virtual Household Household { get; set; }
    }
}
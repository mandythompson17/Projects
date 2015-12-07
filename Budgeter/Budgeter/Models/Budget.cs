using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class Budget
    {
        public int Id { get; set; }
        public int HouseholdId { get; set; }
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }
        public decimal Spent { get; set; }
        public bool IsOver { get; set; }

        public virtual Household Household { get; set;}
        public virtual Category Category { get; set; }
    }
}
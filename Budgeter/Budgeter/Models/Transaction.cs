using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int BankAccountId { get; set; }
        public string UserId { get; set; }
        public int CategoryId { get; set; }
        public DateTimeOffset Date { get; set; }
        public decimal ReconciliationAmount { get; set; }
        public bool IsWithdrawal { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }

        public virtual BankAccount Account { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Category Category { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class Transaction
    {
        private int p1;
        private BankAccount bankAccount;
        private decimal p2;
        private DateTimeOffset dateTimeOffset;
        private string p3;
        private string p4;
        private ApplicationUser applicationUser;
        private bool p5;

        public Transaction(int p1, BankAccount bankAccount, decimal p2, DateTimeOffset dateTimeOffset, string p3, string p4, ApplicationUser applicationUser, bool p5)
        {
            // TODO: Complete member initialization
            this.p1 = p1;
            this.bankAccount = bankAccount;
            this.p2 = p2;
            this.dateTimeOffset = dateTimeOffset;
            this.p3 = p3;
            this.p4 = p4;
            this.applicationUser = applicationUser;
            this.p5 = p5;
        }

        public Transaction()
        {
            // TODO: Complete member initialization
        }
        public int Id { get; set; }
        public int BankAccountId { get; set; }
        public string UserId { get; set; }
        public int? CategoryId { get; set; }
        public DateTimeOffset Date { get; set; }
        public decimal Amount { get; set; }
        public bool IsWithdrawal { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }

        public virtual BankAccount Account { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Category Category { get; set; }
    }
}
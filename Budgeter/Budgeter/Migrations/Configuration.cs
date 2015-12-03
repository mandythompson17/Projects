namespace Budgeter.Migrations
{
    using Budgeter.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Budgeter.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Budgeter.Models.ApplicationDbContext context)
        {
            // ---------------------- Categories ----------------------- //

            if (!context.Categories.Any(c => c.Name == "Grocery"))
            {
                Category grocery = new Category();
                grocery.Name = "Grocery";
                context.Categories.Add(grocery);
            }
            if (!context.Categories.Any(c => c.Name == "Gas"))
            {
                Category gas = new Category();
                gas.Name = "Gas";
                context.Categories.Add(gas);
            }
        }
    }
}

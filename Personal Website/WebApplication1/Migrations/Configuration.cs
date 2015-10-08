namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Collections.Generic;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity;
    using WebApplication1.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<WebApplication1.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(WebApplication1.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "Moderator"))
            {
                roleManager.Create(new IdentityRole { Name = "Moderator" });
            }

            var userManager = new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "mandy.thompson17@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "mandy.thompson17@gmail.com",
                    Email = "mandy.thompson17@gmail.com",
                    FirstName = "Mandy",
                    LastName = "Thompson",
                    DisplayName = "MandyThompson"
                }, "biggy7x");
            }

            var userId = userManager.FindByEmail("mandy.thompson17@gmail.com").Id;
            userManager.AddToRole(userId, "Admin");

            if (!context.Users.Any(u => u.Email == "moderator@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "moderator@coderfoundry.com",
                    Email = "moderator@coderfoundry.com",
                    FirstName = "Coder",
                    LastName = "Foundry",
                    DisplayName = "CoderFoundry"
                }, "Password-1");
            }

            var userId2 = userManager.FindByEmail("moderator@coderfoundry.com").Id;
            userManager.AddToRole(userId2, "Moderator");
        }
    }
}

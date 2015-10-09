namespace WebApplication1.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WebApplication1.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<WebApplication1.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(WebApplication1.Models.ApplicationDbContext context)
        {
        //    var roleManager = new RoleManager<IdentityRole>(
        //         new RoleStore<IdentityRole>(context));

        //    if (!context.Roles.Any(r => r.Name == "Admin"))
        //    {
        //        roleManager.Create(new IdentityRole { Name = "Admin" });
        //    }

        //    var userManager = new UserManager<ApplicationUser>(
        //     new UserStore<ApplicationUser>(context));

        //    if (!context.Users.Any(u => u.Email == ConfigurationManager.AppSettings["AdminUser"])) 
        //    {
        //        userManager.Create(new ApplicationUser
        //        {
        //            UserName = ConfigurationManager.AppSettings["AdminUser"],
        //            Email = ConfigurationManager.AppSettings["AdminUser"],
        //            FirstName = "Mandy",
        //            LastName = "Thompson",
        //            DisplayName = "Mandy"
        //        }, ConfigurationManager.AppSettings["AdminPassword"]);
        //    }

        //    var userId = userManager.FindByEmail(ConfigurationManager.AppSettings["AdminUser"]).Id;
        //    userManager.AddToRole(userId, "Admin");

        //    if (!context.Roles.Any(r => r.Name == "Moderator"))
        //    {
        //        roleManager.Create(new IdentityRole { Name = "Moderator" });
        //    }

        //    userManager = new UserManager<ApplicationUser>(
        //     new UserStore<ApplicationUser>(context));

        //    if (!context.Users.Any(u => u.Email == ConfigurationManager.AppSettings["ModUser"]))
        //    {
        //        userManager.Create(new ApplicationUser
        //        {
        //            UserName = ConfigurationManager.AppSettings["ModUser"],
        //            Email = ConfigurationManager.AppSettings["ModUser"],
        //            FirstName = "Moderator",
        //            LastName = "M",
        //            DisplayName = "Moderator"
        //        }, ConfigurationManager.AppSettings["ModPassword"]);
        //    }

        //    userId = userManager.FindByEmail(ConfigurationManager.AppSettings["ModUser"]).Id;
        //    userManager.AddToRole(userId, "Moderator");

        //}

            var AdminUser = ConfigurationManager.AppSettings["AdminUser"];
            
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
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
                    DisplayName = "Mandy"
                }, "Working4theWeekend");
            }

            var userId = userManager.FindByEmail("mandy.thompson17@gmail.com").Id;
            userManager.AddToRole(userId, "Admin");

            if (!context.Roles.Any(r => r.Name == "Moderator"))
            {
                roleManager.Create(new IdentityRole { Name = "Moderator" });
            }

            userManager = new UserManager<ApplicationUser>(
             new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "moderator@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "moderator@coderfoundry.com",
                    Email = "moderator@coderfoundry.com",
                    FirstName = "Moderator",
                    LastName = "M",
                    DisplayName = "Moderator"
                }, "Password-1");
            }

            userId = userManager.FindByEmail("moderator@coderfoundry.com").Id;
            userManager.AddToRole(userId, "Moderator");

        }
    }
}

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

            
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            var userManager = new UserManager<ApplicationUser>(
             new UserStore<ApplicationUser>(context));

            var AdminUser = ConfigurationManager.AppSettings["AdminUser"];
            var AdminPassword = ConfigurationManager.AppSettings["AdminPassword"];

            if (!context.Users.Any(u => u.Email == AdminUser))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = AdminUser,
                    Email = AdminUser,
                    FirstName = "Mandy",
                    LastName = "Thompson",
                    DisplayName = "Mandy"
                }, AdminPassword);
            }

            var userId = userManager.FindByEmail(AdminUser).Id;
            userManager.AddToRole(userId, "Admin");

            if (!context.Roles.Any(r => r.Name == "Moderator"))
            {
                roleManager.Create(new IdentityRole { Name = "Moderator" });
            }

            userManager = new UserManager<ApplicationUser>(
             new UserStore<ApplicationUser>(context));

            var ModUser = ConfigurationManager.AppSettings["ModUser"];
            var ModPassword = ConfigurationManager.AppSettings["ModPassword"];

            if (!context.Users.Any(u => u.Email == ModUser))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = ModUser,
                    Email = ModUser,
                    FirstName = "Moderator",
                    LastName = "M",
                    DisplayName = "Moderator"
                }, ModPassword);
            }

            userId = userManager.FindByEmail(ModUser).Id;
            userManager.AddToRole(userId, "Moderator");

        }
    }
}

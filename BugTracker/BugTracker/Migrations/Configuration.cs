namespace BugTracker.Migrations
{
    using BugTracker.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BugTracker.Models.ApplicationDbContext context)
        {
            // ---------------------- ROLES ----------------------- //

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "Project Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }

            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }

            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }


            // ---------------------- USERS ----------------------- //

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var AdminUser = ConfigurationManager.AppSettings["AdminUser"];
            var AdminPassword = ConfigurationManager.AppSettings["AdminPassword"];
            var AdminPhone = ConfigurationManager.AppSettings["AdminPhone"];

            if (!context.Users.Any(u => u.Email == AdminUser))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = AdminUser,
                    Email = AdminUser,
                    PhoneNumber = AdminPhone,
                }, AdminPassword);
            }

            var userId = userManager.FindByEmail(AdminUser).Id;
            userManager.AddToRole(userId, "Admin");

            // ---------------------- Statuses ----------------------- //

            if (!context.TicketStatuses.Any(s => s.Name == "Unassigned"))
            {
                TicketStatus unassigned = new TicketStatus();
                unassigned.Name = "Unassigned";
                context.TicketStatuses.Add(unassigned);
            }

            if (!context.TicketStatuses.Any(s => s.Name == "Assigned / Open"))
            {
                TicketStatus assigned = new TicketStatus();
                assigned.Name = "Assigned / Open";
                context.TicketStatuses.Add(assigned);
            }

            if (!context.TicketStatuses.Any(s => s.Name == "Ready for Testing"))
            {
                TicketStatus testReady = new TicketStatus();
                testReady.Name = "Ready for Testing";
                context.TicketStatuses.Add(testReady);
            }

            if (!context.TicketStatuses.Any(s => s.Name == "Resolved"))
            {
                TicketStatus resolved = new TicketStatus();
                resolved.Name = "Resolved";
                context.TicketStatuses.Add(resolved);
            }

            // ---------------------- Statuses ----------------------- //

            if (!context.TicketPriorities.Any(p => p.Name == "Urgent"))
            {
                Priority urgent = new Priority();
                urgent.Name = "Urgent";
                context.TicketPriorities.Add(urgent);
            }
            if (!context.TicketPriorities.Any(p => p.Name == "High"))
            {
                Priority high = new Priority();
                high.Name = "High";
                context.TicketPriorities.Add(high);
            }
            if (!context.TicketPriorities.Any(p => p.Name == "Medium"))
            {
                Priority medium = new Priority();
                medium.Name = "Medium";
                context.TicketPriorities.Add(medium);
            }
            if (!context.TicketPriorities.Any(p => p.Name == "Low"))
            {
                Priority low = new Priority();
                low.Name = "Low";
                context.TicketPriorities.Add(low);
            }

            // ---------------------- Types ----------------------- //

            if (!context.TicketTypes.Any(t => t.Name == "Bug"))
            {
                TicketType bug = new TicketType();
                bug.Name = "Bug";
                context.TicketTypes.Add(bug);
            }
            if (!context.TicketTypes.Any(t => t.Name == "Upgrade"))
            {
                TicketType upgrade = new TicketType();
                upgrade.Name = "Upgrade";
                context.TicketTypes.Add(upgrade);
            }
        }
    }
}

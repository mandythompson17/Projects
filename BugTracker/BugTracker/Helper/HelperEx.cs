using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace BugTracker.Models
{
    // --- User Roles --- // 
    public class UserRolesHelper
    {
        private UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

        public bool IsUserInRole(string userId, string roleName)
        {
            return manager.IsInRole(userId, roleName);

        }

        public IList<string> ListUserRoles(string userId)
        {
            return manager.GetRoles(userId);
        }

        public bool AddUserToRole(string userId, string roleName)
        {
            var result = manager.AddToRole(userId, roleName);
            return result.Succeeded;
        }

        public bool RemoveUserFromRole(string userId, string roleName)
        {
            var result = manager.RemoveFromRole(userId, roleName);
            return result.Succeeded;
        }

        public IList<ApplicationUser> UsersInRole(string roleName)
        {
            var db = new ApplicationDbContext();
            var resultList = new List<ApplicationUser>();

            foreach (var user in db.Users)
            {
                if (IsUserInRole(user.Id, roleName))
                {
                    resultList.Add(user);
                }
            }
            return resultList;
        }

        public IList<ApplicationUser> UsersNotInRole(string roleName)
        {
            var resultList = new List<ApplicationUser>();

            foreach (var user in manager.Users)
            {
                if (!IsUserInRole(user.Id, roleName))
                {
                    resultList.Add(user);
                }
            }
            return resultList;
        }
    }

    // --- User Projects --- //
    public class UserProjectsHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public bool IsUserInProject(string userId, int projectId)
        {
            return db.Users.Find(userId).Projects.Any(p => p.Id == projectId);
        }

        public IList<Project> ListUserProjects(string userId)
        {
            var user = db.Users.Find(userId);
            return user.Projects.ToList();
        }

        public bool AddUserToProject(string userId, int projectId)
        {
            var user = db.Users.Find(userId);
            var project = db.Projects.Find(projectId);
            if (!IsUserInProject(userId, projectId))
            {
                project.ProjectUsers.Add(user);
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveUserFromProject(string userId, int projectId)
        {
            var user = db.Users.Find(userId);
            var project = db.Projects.Find(projectId);
            if (IsUserInProject(userId, projectId))
            {
                project.ProjectUsers.Remove(user);
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public IList<ApplicationUser> UsersInProject(int projectId)
        {
            var resultList = new List<ApplicationUser>();
            var Users = db.Users.ToList();

            foreach (var user in Users)
            {
                if (IsUserInProject(user.Id, projectId))
                {
                    resultList.Add(user);
                }
            }
            return resultList;
        }

        public IList<ApplicationUser> UsersNotInProject(int projectId)
        {
            var resultList = new List<ApplicationUser>();

            foreach (var user in db.Users)
            {
                if (!IsUserInProject(user.Id, projectId))
                {
                    resultList.Add(user);
                }
            }
            return resultList;
        }
    }
   // if (context.Users.Where(u => u.Roles.Any(r => r.Id)).ToList();

    // --- Tickets Histories --- //
    public class TicketHistoryHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public bool RecordTicketChanges(Ticket ticket, string userId)
        {
            if (ticket != null)
            {
                var OldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);
                var tick = ticket;
                var editId = Guid.NewGuid().ToString();
                var changed = System.DateTimeOffset.Now;

                if (OldTicket.Title != ticket.Title)
                {
                    TicketHistory thT = new TicketHistory
                    {
                        TicketId = ticket.Id,
                        Property = "Title",
                        OldValue = OldTicket.Title,
                        OldDisplayValue = OldTicket.Title,
                        NewDisplayValue = ticket.Title,
                        NewValue = ticket.Title,
                        EditId = editId,
                        DateChanged = changed,
                        UserId = userId
                    };
                    Notification titleChange = new Notification
                    {
                        TicketId = ticket.Id,
                        CreatorUserId = userId,
                        Creator = db.Users.Find(userId),
                        RecipientUserId = ticket.AssignedToUserId,
                        Recipient = db.Users.Find(ticket.AssignedToUserId),
                        Change = "Title",
                        Details = ticket.Title,
                        DateNotified = changed
                    };
                    db.Notifications.Add(titleChange);
                    if (ticket.AssignedToUserId != null && titleChange.CreatorUserId != ticket.AssignedToUserId)
                    {
                        SendTicketNotification(titleChange);
                    }
                    db.TicketHistories.Add(thT);
                }
                if (OldTicket.Description != ticket.Description)
                {
                    TicketHistory thD = new TicketHistory
                    {
                        TicketId = ticket.Id,
                        Property = "Description",
                        OldValue = OldTicket.Description,
                        OldDisplayValue = OldTicket.Description,
                        NewDisplayValue = ticket.Description,
                        NewValue = ticket.Description,
                        EditId = editId,
                        DateChanged = changed,
                        UserId = userId
                    };
                    Notification descriptionChange = new Notification
                    {
                        TicketId = ticket.Id,
                        CreatorUserId = userId,
                        Creator = db.Users.Find(userId),
                        RecipientUserId = ticket.AssignedToUserId,
                        Recipient = db.Users.Find(ticket.AssignedToUserId),
                        Change = "Description",
                        Details = ticket.Description,
                        DateNotified = changed
                    };
                    db.Notifications.Add(descriptionChange);
                    if (ticket.AssignedToUserId != null && descriptionChange.CreatorUserId != ticket.AssignedToUserId)
                    {
                        SendTicketNotification(descriptionChange);
                    }
                    db.TicketHistories.Add(thD);
                }
                if (OldTicket.ProjectId != ticket.ProjectId)
                {
                    TicketHistory thP = new TicketHistory
                    {
                        TicketId = ticket.Id,
                        Property = "Project",
                        OldValue = OldTicket.ProjectId.ToString(),
                        OldDisplayValue = OldTicket.Project.Name,
                        NewDisplayValue = ticket.Project.Name,
                        NewValue = ticket.ProjectId.ToString(),
                        EditId = editId,
                        DateChanged = changed,
                        UserId = userId
                    };
                    Notification projectChange = new Notification
                    {
                        TicketId = ticket.Id,
                        CreatorUserId = userId,
                        Creator = db.Users.Find(userId),
                        RecipientUserId = ticket.AssignedToUserId,
                        Recipient = db.Users.Find(ticket.AssignedToUserId),
                        Change = "Project",
                        Details = ticket.Project.Name,
                        DateNotified = changed
                    };
                    db.Notifications.Add(projectChange);
                    if (ticket.AssignedToUserId != null && projectChange.CreatorUserId != ticket.AssignedToUserId)
                    {
                        SendTicketNotification(projectChange);
                    }
                    db.TicketHistories.Add(thP);
                }
                if (OldTicket.TicketTypeId != ticket.TicketTypeId)
                {
                    TicketHistory thTy = new TicketHistory
                    {
                        TicketId = ticket.Id,
                        Property = "Type",
                        OldValue = OldTicket.TicketTypeId.ToString(),
                        OldDisplayValue = OldTicket.Type.Name,
                        NewDisplayValue = ticket.Type.Name,
                        NewValue = ticket.TicketTypeId.ToString(),
                        EditId = editId,
                        DateChanged = changed,
                        UserId = userId
                    };
                    Notification typeChange = new Notification
                    {
                        TicketId = ticket.Id,
                        CreatorUserId = userId,
                        Creator = db.Users.Find(userId),
                        RecipientUserId = ticket.AssignedToUserId,
                        Recipient = db.Users.Find(ticket.AssignedToUserId),
                        Change = "Type",
                        Details = ticket.Type.Name,
                        DateNotified = changed
                    };
                    db.Notifications.Add(typeChange);
                    if (ticket.AssignedToUserId != null && typeChange.CreatorUserId != ticket.AssignedToUserId)
                    {
                        SendTicketNotification(typeChange);
                    }
                    db.TicketHistories.Add(thTy);
                }
                if (OldTicket.TicketPriorityId != ticket.TicketPriorityId)
                {
                    TicketHistory thPr = new TicketHistory
                    {
                        TicketId = ticket.Id,
                        Property = "Priority",
                        OldValue = OldTicket.TicketPriorityId.ToString(),
                        OldDisplayValue = OldTicket.Priority.Name,
                        NewDisplayValue = ticket.Priority.Name,
                        NewValue = ticket.TicketPriorityId.ToString(),
                        EditId = editId,
                        DateChanged = changed,
                        UserId = userId
                    };
                    Notification priorityChange = new Notification
                    {
                        TicketId = ticket.Id,
                        CreatorUserId = userId,
                        Creator = db.Users.Find(userId),
                        RecipientUserId = ticket.AssignedToUserId,
                        Recipient = db.Users.Find(ticket.AssignedToUserId),
                        Change = "Priority",
                        Details = ticket.Priority.Name,
                        DateNotified = changed
                    };
                    db.Notifications.Add(priorityChange);
                    if (ticket.AssignedToUserId != null && priorityChange.CreatorUserId != ticket.AssignedToUserId)
                    {
                        SendTicketNotification(priorityChange);
                    }
                    db.TicketHistories.Add(thPr);
                }
                if (OldTicket.TicketStatusId != ticket.TicketStatusId)
                {
                    tick.Status = db.TicketStatuses.Find(ticket.TicketStatusId);
                    TicketHistory thS = new TicketHistory
                    {
                        TicketId = ticket.Id,
                        Property = "Status",
                        OldValue = OldTicket.TicketStatusId.ToString(),
                        OldDisplayValue = OldTicket.Status.Name,
                        NewDisplayValue = tick.Status.Name,
                        NewValue = ticket.TicketStatusId.ToString(),
                        EditId = editId,
                        DateChanged = changed,
                        UserId = userId
                    };
                    Notification statusChange = new Notification
                    {
                        TicketId = ticket.Id,
                        CreatorUserId = userId,
                        Creator = db.Users.Find(userId),
                        RecipientUserId = ticket.AssignedToUserId,
                        Recipient = db.Users.Find(ticket.AssignedToUserId),
                        Change = "Status",
                        Details = tick.Status.Name,
                        DateNotified = changed
                    };
                    db.Notifications.Add(statusChange);
                    if (ticket.AssignedToUserId != null && statusChange.CreatorUserId != ticket.AssignedToUserId)
                    {
                        SendTicketNotification(statusChange);
                    }
                    db.TicketHistories.Add(thS);
                }
                if (OldTicket.AssignedToUserId != ticket.AssignedToUserId)
                {
                    tick.AssignedToUser = db.Users.Find(ticket.AssignedToUserId);
                    TicketHistory thA = new TicketHistory
                    {
                        TicketId = ticket.Id,
                        Property = "AssignedTo",
                        OldValue = "",
                        OldDisplayValue = "",
                        NewDisplayValue = tick.AssignedToUser.DisplayName,
                        NewValue = ticket.AssignedToUserId,
                        EditId = editId,
                        DateChanged = changed,
                        UserId = userId
                    };
                    Notification ticketAssignment = new Notification
                    {
                        TicketId = ticket.Id,
                        CreatorUserId = userId,
                        Creator = db.Users.Find(userId),
                        RecipientUserId = ticket.AssignedToUserId,
                        Recipient = db.Users.Find(ticket.AssignedToUserId),
                        Change = "Ticket Assigned",
                        Details = tick.AssignedToUser.DisplayName,
                        DateNotified = changed
                    };
                    db.Notifications.Add(ticketAssignment);
                    if (ticket.AssignedToUserId != null && ticketAssignment.CreatorUserId != ticket.AssignedToUserId)
                    {
                        AssignmentNotification(ticketAssignment);
                    }
                    db.TicketHistories.Add(thA);
                }
                if (OldTicket.FileUrl != ticket.FileUrl)
                {
                    TicketHistory thF = new TicketHistory
                    {
                        TicketId = ticket.Id,
                        Property = "FileUrl",
                        OldValue = OldTicket.FileUrl,
                        OldDisplayValue = OldTicket.FileUrl,
                        NewDisplayValue = ticket.FileUrl,
                        NewValue = ticket.FileUrl,
                        EditId = editId,
                        DateChanged = changed,
                        UserId = userId
                    };
                    db.TicketHistories.Add(thF);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public void SendTicketNotification(Notification note)
        {
            Ticket ticket = db.Tickets.Find(note.TicketId);
            ApplicationUser dest = note.Recipient;
            EmailService es = new EmailService();
            IdentityMessage message = new IdentityMessage
            {
                Destination = dest.Email,
                Subject = "There's been a change to " + ticket.Title,
                Body = note.Creator.DisplayName + " changed the " + note.Change + " to " + note.Details + "."
            };
            es.SendAsync(message);
            
        }
        public void AssignmentNotification(Notification note)
        {
            Ticket ticket = db.Tickets.Find(note.TicketId);
            ApplicationUser dest = note.Recipient;
            EmailService es = new EmailService();
            IdentityMessage message = new IdentityMessage
            {
                Destination = dest.Email,
                Subject = "New Ticket Assignment",
                Body = "You've been assigned to " + ticket.Title + " in " + ticket.Project.Name + "."
            };
            es.SendAsync(message);
        }
        public void CommentNotification(Notification note)
        {
            Ticket ticket = db.Tickets.Find(note.TicketId);
            ApplicationUser dest = note.Recipient;
            EmailService es = new EmailService();
            IdentityMessage message = new IdentityMessage
            {
                Destination = dest.Email,
                Subject = "New Comment on " + ticket.Title,
                Body = note.Creator.DisplayName + " commented: \"" + note.Details + "\" at " + note.DateNotified + "."
            };
            es.SendAsync(message);
        }
    }
}
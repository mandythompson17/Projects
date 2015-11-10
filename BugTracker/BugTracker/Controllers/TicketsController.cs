using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using BugTracker.Models;

namespace BugTracker.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        [Authorize]
        public ActionResult Index()
        {
            var tickets = db.Tickets.Include(t => t.Project).Include(t => t.Status).Include(t => t.Type).Include(t => t.Priority).Include(t => t.Owner).Include(t => t.AssignedToUser);
            if (User.IsInRole("Admin"))
            {
                return View(tickets.ToList());
            }
            else if (User.IsInRole("Project Manager"))
            {
                var pm = db.Users.Find(User.Identity.GetUserId());
                var ticketList = new List<Ticket>();
                foreach (var tick in tickets.Where(t => t.OwnerUserId == pm.Id || t.AssignedToUserId == pm.Id))
                {
                    ticketList.Add(tick);
                }
                foreach (var project in pm.Projects)
                {
                    var projectTickets = (tickets.Where(t => t.ProjectId == project.Id));
                    foreach (var ticket in projectTickets)
                    {
                        if (ticket.OwnerUserId != pm.Id && ticket.AssignedToUserId != pm.Id)
                        {
                            ticketList.Add(ticket);
                        }
                        
                    }
                }
            
                return View(ticketList);
            }
            else if (User.IsInRole("Developer"))
            {
                var dev = db.Users.Find(User.Identity.GetUserId());
                var ticketList = new List<Ticket>();
                foreach (var tick in tickets.Where(t => t.OwnerUserId == dev.Id || t.AssignedToUserId == dev.Id))
                {
                    ticketList.Add(tick);
                }
                foreach (var project in dev.Projects)
                {
                    var projectTickets = (tickets.Where(t => t.ProjectId == project.Id));
                    foreach (var ticket in projectTickets)
                    {
                        if (ticket.OwnerUserId != dev.Id && ticket.AssignedToUserId != dev.Id)
                        {
                            ticketList.Add(ticket);
                        }

                    }
                }
                return View(ticketList);
            }
            else if (User.IsInRole("Submitter"))
            {
                var sub = db.Users.Find(User.Identity.GetUserId());
                tickets = tickets.Where(t => t.OwnerUserId == sub.Id);
                if (tickets != null)
                {
                    return View(tickets.ToList());
                }
                else
                {
                    return View();
                }
            }
            else {
                
                    return View();
                }
        }

        // GET: Tickets/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: Tickets/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedToUserId,FileUrl")] Ticket ticket, HttpPostedFileBase image)
        {
            if (image != null && image.ContentLength > 0)
            {
                //check the file name to make sure it's an image
                var ext = Path.GetExtension(image.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".gif" && ext != ".bmp" && ext != ".pdf" && ext != ".txt")
                {
                    ModelState.AddModelError("image", "Invalid Format.");
                }
            }
            if (ModelState.IsValid)
            {
                ticket.OwnerUserId = User.Identity.GetUserId();
                ticket.Owner = db.Users.Find(User.Identity.GetUserId());
                ticket.Created = System.DateTimeOffset.Now;
                ticket.Priority = db.TicketPriorities.Find(ticket.TicketPriorityId);
                ticket.TicketStatusId = db.TicketStatuses.FirstOrDefault(s => s.Name == "Unassigned").Id;
                if (image != null)
                {
                    // relative server path
                    var filePath = "/Attachments";
                    // path on physical drive on server
                    var absPath = Server.MapPath("~" + filePath);
                    // media url for relative 
                    ticket.FileUrl = filePath + "/" + image.FileName;
                    // save image
                    Directory.CreateDirectory(absPath);
                    image.SaveAs(Path.Combine(absPath, image.FileName));
                }
                db.Tickets.Add(ticket);
                ticket.Owner.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
           // var projectDevs = ticket.Project.ProjectUsers.Any(User.IsInRole("Developer"));
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.AssignedToUserId = new SelectList(ticket.Project.ProjectUsers, "Id", "DisplayName", ticket.AssignedToUserId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "Id, Title, Description, Created, Updated, ProjectId, Project, TicketTypeId, Type, TicketPriorityId, Priority, TicketStatusId, Status, OwnerUserId, Owner, AssignedToUserId, AssignedtoUser, FileUrl")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                TicketHistoryHelper thHelper = new TicketHistoryHelper();
                var OldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);
                var editId = Guid.NewGuid().ToString();
                var changed = System.DateTimeOffset.Now;

                if (OldTicket.AssignedToUserId != ticket.AssignedToUserId)
                {
                    ticket.AssignedToUser = db.Users.Find(ticket.AssignedToUserId);
                    TicketHistory thA = new TicketHistory
                    {
                        TicketId = ticket.Id,
                        Property = "AssignedTo",
                        OldValue = OldTicket.AssignedToUserId == null ? "" : OldTicket.AssignedToUserId,
                        OldDisplayValue = OldTicket.AssignedToUserId == null ? "" : OldTicket.AssignedToUser.DisplayName,
                        NewDisplayValue = ticket.AssignedToUser.DisplayName,
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
                        Details = ticket.AssignedToUser.DisplayName,
                        DateNotified = changed
                    };
                    db.Notifications.Add(ticketAssignment);
                    if (ticket.AssignedToUserId != null && ticketAssignment.CreatorUserId != ticket.AssignedToUserId)
                    {
                        thHelper.AssignmentNotification(ticketAssignment);
                    }
                    db.TicketHistories.Add(thA);
                }
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
                        thHelper.SendTicketNotification(titleChange);
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
                        thHelper.SendTicketNotification(descriptionChange);
                    }
                    db.TicketHistories.Add(thD);
                }
                if (OldTicket.ProjectId != ticket.ProjectId)
                {
                    ticket.Project = db.Projects.Find(ticket.ProjectId);
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
                        thHelper.SendTicketNotification(projectChange);
                    }
                    db.TicketHistories.Add(thP);
                }
                if (OldTicket.TicketTypeId != ticket.TicketTypeId)
                {
                    ticket.Type = db.TicketTypes.Find(ticket.TicketTypeId);
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
                        thHelper.SendTicketNotification(typeChange);
                    }
                    db.TicketHistories.Add(thTy);
                }
                if (OldTicket.TicketPriorityId != ticket.TicketPriorityId)
                {
                    ticket.Priority = db.TicketPriorities.Find(ticket.TicketPriorityId);
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
                        thHelper.SendTicketNotification(priorityChange);
                    }
                    db.TicketHistories.Add(thPr);
                }
                if (OldTicket.TicketStatusId != ticket.TicketStatusId)
                {
                    ticket.Status = db.TicketStatuses.Find(ticket.TicketStatusId);
                    TicketHistory thS = new TicketHistory
                    {
                        TicketId = ticket.Id,
                        Property = "Status",
                        OldValue = OldTicket.TicketStatusId.ToString(),
                        OldDisplayValue = OldTicket.Status.Name,
                        NewDisplayValue = ticket.Status.Name,
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
                        Details = ticket.Status.Name,
                        DateNotified = changed
                    };
                    db.Notifications.Add(statusChange);
                    if (ticket.AssignedToUserId != null && statusChange.CreatorUserId != ticket.AssignedToUserId)
                    {
                        thHelper.SendTicketNotification(statusChange);
                    }
                    db.TicketHistories.Add(thS);
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
                var histList = db.TicketHistories.Where(th => th.TicketId == ticket.Id).ToList();
                foreach (var tickHist in histList)
                {
                    if (!ticket.Histories.Contains(tickHist))
                    {
                        ticket.Histories.Add(tickHist);
                    }
                }
                ticket.Updated = System.DateTimeOffset.Now;
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.AssignedToUserId = new SelectList(ticket.Project.ProjectUsers, "Id", "DisplayName", ticket.AssignedToUserId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // POST: Comments
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult SubmitComment([Bind(Include = "Id, TicketId, TicketComment, UserId, FileUrl, Created")] Comment comment, HttpPostedFileBase CommentImage)
        {
            if (CommentImage != null && CommentImage.ContentLength > 0)
            {
                //check the file name to make sure it's an image
                var ext = Path.GetExtension(CommentImage.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".gif" && ext != ".bmp" && ext != ".pdf" && ext != ".txt")
                {
                    ModelState.AddModelError("image", "Invalid Format.");
                }
            }
            var ticket = db.Tickets.Find(comment.TicketId);
            if (ticket == null)
            {
                ModelState.AddModelError("Comment", "Invalid Post. Something went wrong.");
                return RedirectToAction("Index", "Tickets");
            }
            if (ModelState.IsValid)
            {
                TempData["TicketComment"] = comment.TicketComment;
                if (CommentImage != null)
                {
                    // relative server path
                    var filePath = "/Attachments";
                    // path on physical drive on server
                    var absPath = Server.MapPath("~" + filePath);
                    // media url for relative 
                    comment.FileUrl = filePath + "/" + CommentImage.FileName;
                    // save image
                    Directory.CreateDirectory(absPath);
                    CommentImage.SaveAs(Path.Combine(absPath, CommentImage.FileName));
                }

            }
            comment.Created = System.DateTimeOffset.Now;
            comment.User = db.Users.Find(comment.UserId);
            Notification commentMade = new Notification
            {
                TicketId = ticket.Id,
                CreatorUserId = comment.UserId,
                Creator = db.Users.Find(comment.UserId),
                RecipientUserId = ticket.AssignedToUserId,
                Recipient = db.Users.Find(ticket.AssignedToUserId),
                Change = "Comment",
                Details = comment.TicketComment,
                DateNotified = comment.Created
            };
            TicketHistoryHelper helper = new TicketHistoryHelper();
            db.Notifications.Add(commentMade);
            if (ticket.AssignedToUserId != null && commentMade.CreatorUserId != ticket.AssignedToUserId)
            {
                helper.CommentNotification(commentMade);
            }
            db.TicketComments.Add(comment);
            db.SaveChanges();
            return RedirectToAction("Details", new { ticket.Id });
        }
    }
}

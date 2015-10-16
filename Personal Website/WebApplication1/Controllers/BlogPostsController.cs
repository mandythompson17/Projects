using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using PagedList;
using PagedList.Mvc;
using Microsoft.AspNet.Identity;

namespace WebApplication1.Controllers
{
    [RequireHttps]
    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BlogPosts
        public ActionResult Index(int? page, string Query = null, string category = null)
        {
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            if (!string.IsNullOrWhiteSpace(Query) && !string.IsNullOrWhiteSpace(category))
            {
                return View(db.Posts.OrderByDescending(model => model.Created).ToPagedList(pageNumber, pageSize));
            }
            else
            {
                var query = db.Posts.AsQueryable();
                query = !string.IsNullOrWhiteSpace(Query) ? query.Where(q => q.Title.Contains(Query) || q.Body.Contains(Query)) : query;
                query = !string.IsNullOrWhiteSpace(category) ? query.Where(c => c.Category == category) : query;
                ViewBag.Query = Query;
                return View(query.OrderByDescending(model => model.Created).ToPagedList(page ?? 1, 3));
            }
        }


        // GET: BlogPosts/Details/5
        public ActionResult Details(string Slug)
        {
            if (String.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.FirstOrDefault(p => p.Slug == Slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Create
        [Authorize(Roles="Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="Admin")]
        public ActionResult Create([Bind(Include = "Id,Created,Updated,Title,Slug,Body,Category,MediaURL,Published")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (image!=null && image.ContentLength >0)
            {
                //check the file name to make sure it's an image
                var ext = Path.GetExtension(image.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".gif" && ext != ".bmp")
                {
                    ModelState.AddModelError("image", "Invalid Format.");
                }
            }
            
            if (ModelState.IsValid)
            {
                var Slug = StringUtilities.URLFriendly(blogPost.Title);
                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title.");
                    return View(blogPost);
                }
                if (db.Posts.Any(p=>p.Slug == Slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique.");
                    return View(blogPost);
                }
                blogPost.Slug = Slug;

                if (image != null)
                {
                    // relative server path
                    var filePath = "/Uploads";
                    // path on physical drive on server
                    var absPath = Server.MapPath("~" + filePath);
                    // media url for relative 
                    blogPost.MediaURL = filePath + "/" + image.FileName;
                    // save image
                    Directory.CreateDirectory(absPath);
                    image.SaveAs(Path.Combine(absPath, image.FileName));
                }

                blogPost.Created = System.DateTimeOffset.Now;
                db.Posts.Add(blogPost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        [Authorize(Roles="Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="Admin")]
        public ActionResult Edit([Bind(Include = "Id,Title,Body,Category,MediaURL,Published")] BlogPost blogPost)
        {
           /* if (imageEdit != null && image.ContentLength > 0)
            {
                //check the file name to make sure it's an image
                var ext = Path.GetExtension(imageEdit.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".gif" && ext != ".bmp")
                {
                    ModelState.AddModelError("image", "Invalid Format.");
                }
            }*/
            if (ModelState.IsValid)
            {
               /* if (image != null)
                {
                    // relative server path
                    var filePath = "/Uploads";
                    // path on physical drive on server
                    var absPath = Server.MapPath("~" + filePath);
                    // media url for relative 
                    blogPost.MediaURL = filePath + "/" + image.FileName;
                    // save image
                    Directory.CreateDirectory(absPath);
                    image.SaveAs(Path.Combine(absPath, image.FileName));
                }*/
                blogPost.Updated = System.DateTimeOffset.Now;
                db.Posts.Attach(blogPost);
                db.Entry(blogPost).Property("Body").IsModified = true;
                db.Entry(blogPost).Property("Category").IsModified = true;
                db.Entry(blogPost).Property("MediaURL").IsModified = true;
                db.Entry(blogPost).Property("Updated").IsModified = true;
                db.Entry(blogPost).Property("Published").IsModified = true;
                //db.Entry(blogPost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        [Authorize(Roles="Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogPost blogPost = db.Posts.Find(id);
            db.Posts.Remove(blogPost);
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
     
        // ------------------------ COMMENTS -------------------- //

        // POST: Comments
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult SubmitComment([Bind(Include = "Id, PostId, Body")] Comment comment)
        {
            var blogPost = db.Posts.Find(comment.PostId);
            if (blogPost == null)
            {
                ModelState.AddModelError("Comment", "Invalid Post. Something went wrong.");
                return RedirectToAction("Index", "BlogPosts");
            }
            if (!ModelState.IsValid)
            {
                TempData["Body"] = comment.Body;

            }
            comment.AuthorId = User.Identity.GetUserId();
            comment.Created = System.DateTimeOffset.Now;
            db.Comments.Add(comment);
            db.SaveChanges();
            return RedirectToAction("Details", new { blogPost.Slug });
        }

        // GET: Edit Comment
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult EditComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Edit Comment
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult EditComment([Bind(Include = "Id,Body")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                BlogPost blogPost = db.Posts.Find(comment.PostId);
                comment.Updated = System.DateTimeOffset.Now;
                db.Comments.Attach(comment);
                db.Entry(comment).Property("Body").IsModified = true;
                db.Entry(blogPost).Property("Updated").IsModified = true;
                db.SaveChanges();
                return RedirectToAction("Details", new { blogPost.Slug });
            }
            return View(comment);
        }

        // GET: Delete Comment
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult DeleteComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Delete Comment
     /*   [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult DeleteCommentConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            BlogPost blogPost = db.Posts.Find(comment.PostId);
            db.Comments.Remove(comment);
            db.SaveChanges();
            return RedirectToAction("Details", new { blogPost.Slug });
        } */

    }

}

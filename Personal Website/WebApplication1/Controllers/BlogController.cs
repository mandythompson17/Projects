using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [RequireHttps]
    public class BlogController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Blog
        public ActionResult Index()
        {
            var Posts = db.Posts.ToList();
            return View(Posts);
        }

        // Create new Blog post
        public ActionResult Create()
        {
            return View();
        }

        //Submit new Blog post
        [HttpPost]
        public ActionResult Create(BlogPost Post)
        {
            Post.Created = System.DateTimeOffset.Now;
            db.Posts.Add(Post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //See one Blog post
        public ActionResult Details(int? id)
        {
            var Post = db.Posts.Find(id);
            return View(Post);
        }

        //Go to edit Blog post
        public ActionResult EditPost(int? id)
        {
            var Post = db.Posts.Find(id);
            return View(Post);
        }

        //Save changes to Blog post
        [HttpPost]
        public ActionResult EditPost(BlogPost Post)
        {
            Post.Updated = System.DateTimeOffset.Now;
            //db.Posts.Attach(Post);
           // db.Entry(Post).Property("Title").IsModified = true;
            //db.Entry(Post).Property("Body").IsModified = true;
            db.Entry(Post).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Go to Delete Blog post
           public ActionResult DeletePost(int? id)
        {
            var Post = db.Posts.Find(id);
            return View(Post);
        }
        //Delete Post
        [HttpPost]
           public ActionResult DeletePost(BlogPost Post)
           {
               db.Posts.Remove(Post);
               db.SaveChanges();
               return RedirectToAction("Index");
           }
    }
}
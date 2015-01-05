using NLog;
using RafflesChart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
namespace RafflesChart.Controllers
{
    public class BlogsController : Controller
    {
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.Message = "Blog";

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateInput(false)]
        public ActionResult SaveBlog(BlogViewModel vm)
        {
            ViewBag.Message = "Blog";
            //Logger logger = LogManager.GetCurrentClassLogger();            

            using (var context = new ApplicationDbContext())
            {
                var blogentry = new Blog();
                blogentry.Content = System.Text.Encoding.ASCII.GetBytes(vm.Content);
                blogentry.CreatedDate = DateTime.Now;
                blogentry.UpdatedDate = blogentry.CreatedDate;
                blogentry.Title = vm.Title;
                blogentry.Page = vm.Page; 
                context.Blogs.Add(blogentry);
                context.SaveChanges();
            }

            //logger.Trace("content:" + Request.Form["content"]);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            using (var context = new ApplicationDbContext())
            {
                var vm = from x in context.Blogs.ToList()
                         let blogvm = new BlogViewModel()
                         {
                             Title = x.Title,
                             CreatedDate = x.CreatedDate,
                             UpdatedDate = x.UpdatedDate,
                             PageType = ConvertToPageType(x.Page),
                             Id = x.Id,
                             Content = System.Text.Encoding.ASCII.GetString(x.Content)
                         }
                         select blogvm;
                return View(vm.ToList());
            }
        }

        private string ConvertToPageType(string p)
        {
            if (string.IsNullOrEmpty(p)) { return ""; }
            if (p.Equals("0"))
            {
                return "2 cents worth";
            }
            else if (p.Equals("1"))
            {
                return "Blog";
            }
            else if (p.Equals("2"))
            {
                return "Corporate Action";
            }
            else if (p.Equals("3"))
            {
                return "Foreign Market";
            }
            else
            {
                return "";
            }

        }

       [Authorize(Roles = "Admin")]
        public ActionResult Detail(int Id)
        {
            return RetrieveBlogViewModel("Detail",Id);
        }


        private ActionResult RetrieveBlogViewModel(string vname ,int Id)
        {
            using (var context = new ApplicationDbContext())
            {
                var blog = context.Blogs.SingleOrDefault(x => x.Id == Id);
                var blogvm = new BlogViewModel()
                {
                    Title = blog.Title,
                    CreatedDate = blog.CreatedDate,
                    Id = blog.Id,
                    Page = blog.Page,
                    Content = System.Text.Encoding.ASCII.GetString(blog.Content)
                };
                return View(vname,blogvm);
            }
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int Id)
        {
            return RetrieveBlogViewModel("Edit", Id);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateInput(false)]
        public ActionResult EditBlog(BlogViewModel vm)
        {
            ViewBag.Message = "Blog";
          
            using (var context = new ApplicationDbContext())
            {
                var blogentry = context.Blogs.SingleOrDefault(x=> x.Id== vm.Id);
                blogentry.Content = System.Text.Encoding.ASCII.GetBytes(vm.Content);
                blogentry.CreatedDate =vm.CreatedDate;
                blogentry.UpdatedDate = DateTime.Now ;
                blogentry.Title = vm.Title;
                blogentry.Page = vm.Page;
                context.SaveChanges();
            }
            
            return RedirectToAction("Index", "Blogs");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int Id)
        {
            return RetrieveBlogViewModel("Delete", Id);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfrim(BlogViewModel vm)
        {
            ViewBag.Message = "Blog";
            
            using (var context = new ApplicationDbContext())
            {
                var blogentry = context.Blogs.SingleOrDefault(x => x.Id == vm.Id);
                context.Blogs.Remove(blogentry);                
                context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
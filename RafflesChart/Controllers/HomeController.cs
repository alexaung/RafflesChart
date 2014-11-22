using NLog;
using Postal;
using RafflesChart.Models;
using RafflesChart.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using EntityFramework.Extensions;
namespace RafflesChart.Controllers
{

    [Authorize]
    public class HomeController : Controller
    {

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [AllowAnonymous]
        public ActionResult GetCaptcha()
        {
            string cpt;
            byte[] bytes;
            AccountController.GenerateCaptcha(out cpt, out bytes);

            //var mem = new System.IO.FileStream(@"d:\zarni\ctcha.jpeg", FileMode.Create);
            //Bmp.Save(@"d:\zarni\ctcha4.png", System.Drawing.Imaging.ImageFormat.Png);
            //var stream = new MemoryStream();
            //Bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            Session["Captcha"] = cpt;
            return File(bytes, "image/png");
            //Response.BinaryWrite(bytes);

            // return View();
        }

        [AllowAnonymous]
        public ActionResult Sort()
        {
            ViewBag.Message = "Contact Us";

            return View();
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Contact Us";

            return View();
        }
                
        [AllowAnonymous]
        public ActionResult Privacy()
        {
            ViewBag.Message = "Privacy";

            return View();
        }

        [AllowAnonymous]
        public ActionResult Disclaimer()
        {
            ViewBag.Message = "Disclaimer";

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Contact(ContactViewModel vm)
        {

            ViewBag.Message = "Disclaimer";

            if (!CaptchaMvc.HtmlHelpers.CaptchaHelper.IsCaptchaValid(this, "error"))
            {
                ModelState.AddModelError("captcha", "Invalid captcha");
                return View();
            }

            await SendContactEmail(vm.Email, vm.Subject, vm.Message, vm.Name);
            TempData["Sent"] = "1";
            return RedirectToAction("Contact");
        }

        public async Task SendContactEmail(string email, string subject, string message, string name)
        {
            dynamic mail = new Email("ContactEmail");
            mail.Email = email;
            mail.Subject = subject;
            mail.Message = message;
            mail.Name = name;
            await mail.SendAsync();
        }

        [Authorize(Roles = "Admin,SpecialMember")]
        public ActionResult SpecialMember()
        {
            return View();
        }

        [Authorize(Roles = "Admin,SpecialMember")]
        public ActionResult RecentBlogs()
        {
            return RetrieveBlogList("RecentBlogs");
        }

        [Authorize(Roles = "Admin,SpecialMember")]
        public ActionResult BlogArchives()
        {
            return RetrieveBlogList("BlogArchives");
        }

        private ActionResult RetrieveBlogList(string vname)
        {
            using (var context = new ApplicationDbContext())
            {
                var vm = from x in context.Blogs.ToList()
                         let blogvm = new BlogViewModel()
                         {
                             Title = x.Title,
                             CreatedDate = x.CreatedDate,
                             Id = x.Id,
                             Content = System.Text.Encoding.ASCII.GetString(x.Content)
                         }
                         select blogvm;
                return View(vname,vm.ToList());
            }
        }

        [Authorize(Roles = "Admin,SpecialMember")]
        public ActionResult BlogDetail(int Id)
        {
            using (var context = new ApplicationDbContext())
            {
                var blog = context.Blogs.SingleOrDefault(x => x.Id == Id);
                var blogvm = new BlogViewModel()
                {
                    Title = blog.Title,
                    CreatedDate = blog.CreatedDate,
                    Id = blog.Id,
                    Content = System.Text.Encoding.ASCII.GetString(blog.Content)
                };
                return View(blogvm);
            }
        }

        [Authorize(Roles = "Admin")]      
        public ActionResult ViewHit()
        {           
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult GetHitsJson(string dt)
        {
            var dtstart = DateTime.ParseExact("1 Jan 2014", "d MMM yyyy",null);
            var dtend = DateTime.Now;

            if (dt != null && dt.Length == 23)
            {
                string strstart = dt.Split(" - ".ToCharArray())[0];
                string strend = dt.Split(" - ".ToCharArray())[3];
                DateTime.TryParseExact(strstart, new string[] { "MM/dd/yyyy" }, null, System.Globalization.DateTimeStyles.None, out dtstart);
                bool parseok = DateTime.TryParseExact(strend, new string[] { "MM/dd/yyyy" }, null, System.Globalization.DateTimeStyles.None, out dtend);
                if (parseok)
                {
                   dtend= dtend.Date.AddDays(1);
                }
            }

            using (var context = new ApplicationDbContext())
            {
                var hits = from h in context.Hits
                           where h.CreatedDate >= dtstart && h.CreatedDate <=dtend
                           join hl in context.HitLabels on h.Url equals hl.Url into sub
                           from s in sub.DefaultIfEmpty()
                           select new { h.Url, Description = s.Description ?? "", h.CreatedDate };
                var hgrp = from h in hits
                           group h by h.Url into grp
                           select new { grp.Key, Count = grp.Count() };

                var vm = from g in hgrp
                         from h in hits.Select(x => new { x.Description, x.Url }).Distinct()
                         where g.Key == h.Url
                         select new HitViewModel() { Url = h.Url, Page = h.Description, Count = g.Count };


                return Json(vm.ToList() , JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ConfigureHit()
        {
            using (var context = new ApplicationDbContext())
            {
                var hits = from h in context.HitLabels
                         select h;  
                var urls = hits.Select( x=> x.Url).ToList();
                var raw = (from h in context.Hits.ToList()
                           where urls.Contains(h.Url) == false
                           select new { h.Url}).Distinct().ToList();
                var result = hits.ToList().Concat(raw.Select(x=> new HitLabel(){ Description ="" , Url = x.Url}));
                return View(result.ToList());
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> SaveLabel(IList<HitLabel> vm)
        {
            using (var context = new ApplicationDbContext())
            {
                var urlList = vm.Select(x => x.Url).ToString();
                context.HitLabels.Where(x => urlList.Contains(x.Url)).Delete();
                foreach (var item in vm)
                {
                    context.HitLabels.Add(item);
                }
                await context.SaveChangesAsync();
                return RedirectToAction("ViewHit");
            }
        }
    }
}
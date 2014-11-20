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

        [HttpPost]
        [AllowAnonymous, ValidateInput(false)]
        public ActionResult Upload(BlogViewModel vm)
        {
            ViewBag.Message = "Blog";
            Logger logger = LogManager.GetCurrentClassLogger();
            TempData["blogresult"] = vm.Content;

            using (var context = new ApplicationDbContext())
            {
                var blogentry = new Blog();
                blogentry.Content = System.Text.Encoding.ASCII.GetBytes(vm.Content);
                blogentry.CreatedDate = DateTime.Now;
                blogentry.Title = vm.Title;

                context.Blogs.Add(blogentry);
                context.SaveChanges();
            }

            //logger.Trace("content:" + Request.Form["content"]);
            return RedirectToAction("BlogList");
        }
        
        [AllowAnonymous]
        public ActionResult BlogResult()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult BlogList()
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
                return View(vm.ToList());
            }
        }

        [AllowAnonymous]
        public ActionResult BlogView(int Id)
        {
            ViewBag.Message = "Blog";
            using (var context = new ApplicationDbContext())
            {
                var blog = context.Blogs.SingleOrDefault( x=> x.Id == Id) ;
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

        [AllowAnonymous]
        public ActionResult Blog()
        {
            ViewBag.Message = "Blog";

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

    }
}
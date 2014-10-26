﻿using Postal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RafflesChart.Controllers {

   [Authorize]
    public class HomeController : Controller {

        [AllowAnonymous]
        public ActionResult Index() {
            return View();
        }

        public ActionResult About() {
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
        public ActionResult Contact() {
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
        public ActionResult Contact(ContactViewModel vm)
        {
            
            ViewBag.Message = "Disclaimer";

            if (!CaptchaMvc.HtmlHelpers.CaptchaHelper.IsCaptchaValid(this, "error"))
             {
                 ModelState.AddModelError("captcha", "Invalid captcha");
                return View();
            }

            SendContactEmail(vm.From, vm.Subject, vm.Message);
            TempData["Sent"] = "1";
            return RedirectToAction("Contact");
        }

        public void SendContactEmail(string from, string subject, string message)
        {
            dynamic mail = new Email("ContactEmail");
            mail.From = from;
            mail.Subject = subject;
            mail.Message = message;
            mail.SendAsync();
        }

        [Authorize(Roles = "Admin,SpecialMember")]
        public ActionResult SpecialMember()
        {
            return View();
        }

    }
}
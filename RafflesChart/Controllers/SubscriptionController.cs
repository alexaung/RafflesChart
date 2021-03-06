﻿using Postal;
using RafflesChart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RafflesChart.Controllers
{
    public class SubscriptionController : Controller
    {
        // GET: Subscription
        public ActionResult Index()
        {
            using (var db = new ApplicationDbContext())
            {
                var vm = db.Subscriptions.Where(x=> x.Status== "Sale").ToList();

                return View("Add",vm);
            } 
           
        }
        public ActionResult Add()
        {
            //var month = Request.Form["Des"];
            var price = Request.Form["Price"];
            var itemName = Request.Form["Description"];
            var total = double.Parse(price);
            NVPAPICaller test = new NVPAPICaller();
            string retMsg = "";
            string token = "";
            TempData["Total"] = total.ToString();
            TempData["ItemName"] = itemName;
            TempData["Price"] = price;
            TempData["Month"] = "1";
            TempData["SubscriptionId"] = Request.Form["SubscriptionId"];
            bool ret = test.ShortcutExpressCheckout(total.ToString(), itemName, "1", price, ref token, ref retMsg);
            if (ret)
            {
               return Redirect(retMsg);
            }
            else
            {
                // Response.Redirect("APIError.aspx?" + retMsg);
                TempData["SetError"] = retMsg;
                return View();
            }           
        }

        public async Task SendPaymentEmail(string email , string subscription, string price, string month, string total, string paypalref)
        {
            dynamic mail = new Email("Payment");
            mail.User =  email;
            mail.Subscription = subscription;
            mail.Price = price;
            mail.Month = month;
            mail.Total = total;
            mail.PaypalRef = paypalref;
            await mail.SendAsync();
        }

        public ActionResult ConfirmCheckout()
        {
            var token = Request.QueryString["token"].ToString();
            var payer = Request.QueryString["PayerID"].ToString();
            ViewBag.Total = TempData["Total"];
            ViewBag.Token = token;
            ViewBag.PayerId = payer;
            return View();
        }

        public ActionResult MySubscription()
        {
            using (var db = new ApplicationDbContext())
            {
                var userId = User.Identity.Name;
                var vm = db.UserSubscriptions.Where(x => x.UserId == userId).OrderByDescending(x=> x.CreatedDate);
                return View(vm.ToList());
            }
        }

        public ActionResult ProceedCheckout()
        {
            NVPAPICaller test = new NVPAPICaller();
            string retMsg = "";
            string token = Request.Form["Token"];
            string PayerId = Request.Form["PayerId"];
            string total = Request.Form["Total"];

            var decoder = new NVPCodec();
            bool ret = test.ConfirmPayment(total, token, PayerId, ref decoder, ref retMsg);
            if (ret)
            {
                string paypalref = decoder["PAYMENTINFO_0_TRANSACTIONID"];
                var usersubscription = new UserSubscription();
                using(var db = new ApplicationDbContext())
	            {
		           
                    usersubscription.CreatedDate = DateTime.Now;
                    usersubscription.SubscriptionId = int.Parse(TempData["SubscriptionId"].ToString());
                    usersubscription.Price = decimal.Parse( TempData["Price"].ToString());
                    usersubscription.Month = int.Parse(TempData["Month"].ToString());
                    usersubscription.ItemName = TempData["ItemName"].ToString();
                    usersubscription.PaypalRef = paypalref;
                    usersubscription.UserId = User.Identity.Name;
                    db.UserSubscriptions.Add(usersubscription);
                    db.SaveChanges();
	            }

                var taskemail = SendPaymentEmail( User.Identity.Name, usersubscription.ItemName, usersubscription.Price.ToString() 
                                , usersubscription.Month.ToString()
                                , (usersubscription.Price * usersubscription.Month).ToString(), usersubscription.PaypalRef );
                // taskemail.Start();
                TempData["Success"] = "Payment Successful";
                //Response.Redirect(retMsg);
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ConfirmError"] = retMsg;
                TempData["Total"] = total;
                return RedirectToAction("ConfirmCheckout", new { @token = token, @PayerID = PayerId });
            }
        }
    }
}
using PaypalMVC.Models;
//using SandboxTest.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
namespace PalpalMVC.Controllers
{
	public class PayPalController : Controller
	{
        public ActionResult Subscription()
        {
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }
		public ActionResult RedirectFromPaypal()
		{
			return View();
		}

        public ActionResult ConfirmCheckout()
        {
            var token = Request.QueryString["token"].ToString();
            var payer = Request.QueryString["PayerID"].ToString();

            ViewBag.Token = token;
            ViewBag.PayerId = payer;
            return View();
        }

        public ActionResult ProceedCheckout()
        {
            NVPAPICaller test = new NVPAPICaller();
            string retMsg = "";
            string token = Request.Form["Token"];
            string PayerId = Request.Form["PayerId"];
           
            var decoder = new NVPCodec();
            bool ret = test.ConfirmPayment("10", token, PayerId,ref decoder,ref retMsg);
            if (ret)
            {
                Response.Redirect(retMsg);
            }
            else
            {
                Response.Redirect("APIError.aspx?" + retMsg);
            }
            return View();
        }

		public ActionResult CancelFromPaypal()
		{
			return View();
		}
		
		public ActionResult NotifyFromPaypal()
		{
			return 	View();
		}

        public ActionResult ExpressCheckOut()
        {
            return View();
        }

		public ActionResult ValidateCommand(string product, string totalPrice)
		{
			bool useSandbox = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSandbox"]);
			var paypal = new PayPalModel(useSandbox);
			
			paypal.item_name = product;
			paypal.amount = totalPrice;
			return View(paypal);
		}
	}
}
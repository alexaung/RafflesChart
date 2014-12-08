using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RafflesChart.Controllers
{
    public class SubscriptionController : Controller
    {
        // GET: Subscription
        public ActionResult Index()
        {
            return View("Add");
        }
        public ActionResult Add()
        {
            var month = Request.Form["SelectedMonth"];
            var price = Request.Form["Price"];
            var itemName = Request.Form["ItemName"];
            var total = int.Parse(month) * double.Parse(price);
            NVPAPICaller test = new NVPAPICaller();
            string retMsg = "";
            string token = "";
            TempData["Total"] = total.ToString();
            bool ret = test.ShortcutExpressCheckout(total.ToString(), itemName, month, price, ref token, ref retMsg);
            if (ret)
            {
               return Redirect(retMsg);
            }
            else
            {
                Response.Redirect("APIError.aspx?" + retMsg);
            }
            return View();
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
                TempData["Success"] = "Payment Successful";
                //Response.Redirect(retMsg);
                return RedirectToAction("Index");
            }
            else
            {
                Response.Redirect("APIError.aspx?" + retMsg);
            }
            return View();
        }
    }
}
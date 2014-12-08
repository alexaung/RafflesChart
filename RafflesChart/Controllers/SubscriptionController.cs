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
        public ActionResult Add()
        {
            var month = Request.Form["SelectedMonth"];
            return View();
        }
    }
}
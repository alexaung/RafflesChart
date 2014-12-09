using RafflesChart.Models;
using RafflesChart.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RafflesChart.Controllers
{
    public class ManageSubscriptionController : Controller
    {
        // GET: ManageSubscription
        public ActionResult Add()
        {
            return View();
        }
        public ActionResult Edit()
        {
            var vm = new SubscriptionEntryViewModel();
            return View(vm);
        }

        public ActionResult List()
        {
            var vm = new List<SubscriptionEntryViewModel>();
            return View(vm);
        }
    }
}
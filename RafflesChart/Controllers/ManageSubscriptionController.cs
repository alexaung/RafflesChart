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
        [HttpPost]
        public ActionResult Add(SubscriptionEntryViewModel vm)
        {
            using (var db= new ApplicationDbContext())
            {
                var subscription = new Subscription();
                subscription.Name = vm.Name;
                subscription.Price = vm.Price;
                subscription.Detail = vm.Detail;
                var dtnow = DateTime.Now;
                subscription.CreatedDate = dtnow;
                subscription.UpdatedDate = dtnow;
                subscription.Status = vm.Status;
                db.Subscriptions.Add(subscription);
                db.SaveChanges();
            }
            return RedirectToAction("List");
        }

        public ActionResult Add()
        {
            return View();
        }
        public ActionResult Edit(int Id)
        {
            using (var db = new ApplicationDbContext())
            {
                var vm = db.Subscriptions.FirstOrDefault(x=> x.Id == Id);

                return View(vm);
            }            
        }

        [HttpPost]
        public ActionResult Edit(Subscription vm)
        {
            using (var db = new ApplicationDbContext())
            {
                var subscription = db.Subscriptions.FirstOrDefault(x => x.Id == vm.Id);

                subscription.Name = vm.Name;
                subscription.Price = vm.Price;
                subscription.Detail = vm.Detail;
                subscription.UpdatedDate = DateTime.Now;
                subscription.Status = vm.Status;
                db.SaveChanges();
                return RedirectToAction("List");
            }
        }

        public ActionResult List()
        {
            using (var db = new ApplicationDbContext())
            {
                var vm = db.Subscriptions.ToList();

                return View(vm);
            }            
        }
    }
}
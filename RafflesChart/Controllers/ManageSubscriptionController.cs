using RafflesChart.Models;
using RafflesChart.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RafflesChart.Controllers
{
    [Authorize]
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
         public ActionResult Payment(){
             return View();
         }
         
        public ActionResult List()
        {
            using (var db = new ApplicationDbContext())
            {
                var vm = db.Subscriptions.ToList();

                return View(vm);
            }            
        }

        [Authorize(Roles = "Admin")]
        public ActionResult GetPaymentsJson(string paymentdate,
                               string username,string useremail,string subscriptionname,string paypalref )
        {
            var dtstart = DateTime.ParseExact("1 Jan 2014", "d MMM yyyy", null);
            var dtend = DateTime.Now;
            DateTime d1 = new DateTime(1970, 1, 1);
            if (paymentdate != null && paymentdate.Length == 23)
            {
                string strstart = paymentdate.Split(" - ".ToCharArray())[0];
                string strend = paymentdate.Split(" - ".ToCharArray())[3];
                DateTime.TryParseExact(strstart, new string[] { "MM/dd/yyyy" }, null, System.Globalization.DateTimeStyles.None, out dtstart);
                bool parseok = DateTime.TryParseExact(strend, new string[] { "MM/dd/yyyy" }, null, System.Globalization.DateTimeStyles.None, out dtend);
                if (parseok)
                {
                    dtend = dtend.Date.AddDays(1);
                }
            }

            using (var context = new ApplicationDbContext())
            {
                var subscriptions = context.UserSubscriptions.AsQueryable();
                var users = context.Users.AsQueryable();
                if (!string.IsNullOrEmpty(username))
                {
                    users = users.Where(u => u.Name.Contains(username));
                }
                if (!string.IsNullOrEmpty(useremail))
                {
                    users = users.Where(u => u.Email.Contains(useremail));
                }
                if (!string.IsNullOrEmpty(paypalref))
                {
                    subscriptions = subscriptions.Where(x => x.PaypalRef.Contains(paypalref));
                }

                if (!string.IsNullOrEmpty(subscriptionname))
                {
                    subscriptions = subscriptions.Where(x => x.ItemName.Contains(subscriptionname));
                }
                
                 var vm =  from u in users
                           from s in subscriptions
                           where s.CreatedDate >= dtstart && s.CreatedDate <= dtend
                            && s.UserId == u.UserName
                            select new { UserEmail = u.Email , UserName= u.Name ,
                                        SubscriptionName = s.ItemName , 
                                        Price = s.Price,
                                        Month =s.Month, 
                                        CreatedDate = s.CreatedDate,
                                        PaypalRef = s.PaypalRef};

                 var ngvm = vm.ToList().Select(x => new
                 {
                     x.UserName,
                     x.UserEmail,
                     x.SubscriptionName,
                     x.Price,
                     x.Month,
                     x.PaypalRef,
                     CreatedDate = new TimeSpan(x.CreatedDate.AddHours(-8).Ticks - d1.Ticks).TotalMilliseconds,
                 });


                 return Json(ngvm, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RafflesChart.Models;
using System.Web.Security;

namespace RafflesChart.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        public ActionResult GetEventDate(int i){
            var str = DateTime.Now.AddMonths(i).ToString("G");
            return Json(str);
        }
        [AllowAnonymous]
        // GET: Events
        public async Task<ActionResult> Index()
        {
            var evts = await GetEventsToDisplayAsync();  
            return View(evts);
        }

        private async Task<EventViewModel[]> GetEventsToDisplayAsync()
        {
            var uEvent = await db.EventUsers.Where(u => u.UserEmail == User.Identity.Name).ToArrayAsync();
            var arrEv = await (from e in db.Events
                               where e.Date >= DateTime.Now
                               select e).ToArrayAsync();
            bool adminUser = false;
            if (User.IsInRole("Admin"))
            {
                adminUser = true;
            }
            var sql = from e in arrEv
                      let rg = uEvent.Any(x => x.EventId == e.Id)
                      select new EventViewModel
                      {
                          AvailableEventId = e.Id,
                          AvailableEventName = e.Name,
                          AvailableEventDate = e.Date,
                          AvailableEventLocation = e.Location,
                          AvailableEventDescription = e.Description,
                          Registered = rg,
                          Users = GetUserForEvent(e.Id, adminUser)
                      }
                ;
            var evts = sql.ToArray();
            return evts;
        }

        private IEnumerable<Registrant> GetUserForEvent(int p, bool forAdmin)
        {
            var emails = db.EventUsers.Where(x => x.EventId == p).Select(x => x.UserEmail);
            var regs = from em in emails
                       from u in db.Users
                       where em == u.Email && forAdmin
                       select new Registrant() { Email = u.Email, FullName = u.Name, PhoneNumber = u.PhoneNumber };

            var guests = from gu in db.EventGuestUsers
                         where gu.EventId == p
                         select new Registrant() { Email = gu.Email, FullName = gu.Name, PhoneNumber = gu.PhoneNumber };
            return regs.Concat(guests).ToArray();
        }

        // GET: Events/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = await db.Events.FindAsync(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // GET: Events/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }
        [AllowAnonymous]       
        public async Task<ActionResult> GuestRegister(int eventId)
        {
            var evts = await GetEventsToDisplayAsync();
            var vm = new SearchUser() { EventId = eventId };

            evts.FirstOrDefault(x => x.AvailableEventId == eventId).GuestSignUp = vm;

            return View("Index",vm);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> GuestRegister(SearchUser suser)
        {
            if (ModelState.IsValid)
            {
                string cpt = Session["GuestCaptcha"] as string;

                if (!suser.Captcha.Equals(cpt))
                {
                    //ModelState.AddModelError("", "Wrong Captcha!");
                    return Json(false);
                }
            }
            var evtuser = new EventGuestUser();
            evtuser.Email = suser.Email;
            evtuser.EventId = suser.EventId;
            evtuser.PhoneNumber = suser.PhoneNumber;
            evtuser.Name = suser.Name;
            db.EventGuestUsers.Add(evtuser);
            await db.SaveChangesAsync();
            //return RedirectToAction("Index","Home");
            return Json(true);
        }

        [HttpPost]
        public async Task<ActionResult> Register(int eventId)
        {
            //EventUser evtusr
            string useremail = "";
            useremail = User.Identity.Name;
            int evtid = eventId;
            var evtuser = new EventUser();
            evtuser.UserEmail = useremail;
            evtuser.EventId = evtid; 
			
			var evtfound = db.EventUsers.FirstOrDefault(x => x.UserEmail == useremail && 
									  x.EventId	== evtid);
			if(evtfound!=null){
                db.EventUsers.Remove(evtfound);
			}	
			else
			{			
				db.EventUsers.Add(evtuser);
			}
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
           
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Date,Location,Description")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Events.Add(@event);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = await db.Events.FindAsync(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Date,Location,Description")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = await db.Events.FindAsync(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Event @event = await db.Events.FindAsync(id);
            db.Events.Remove(@event);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

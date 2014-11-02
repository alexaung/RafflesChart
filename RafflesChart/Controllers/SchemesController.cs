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
using NPOI.HSSF.UserModel;
using System.IO;
using NPOI.XSSF.UserModel;
using Microsoft.AspNet.Identity;
using System.Web.Security;
using EntityFramework.Extensions;
using RafflesChart.Extensions;
using Newtonsoft.Json;

namespace RafflesChart.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SchemesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: Schemes
        public  ActionResult IndexA()
        {
            return View();
        }

        // GET: Schemes
        [HttpGet]
        public async Task<ActionResult> SchemeList()
        {
            var ls = await db.Schemes.ToListAsync();

            var ret = JsonConvert.SerializeObject (ls);
            return Content(ret);
        }

        // GET: Schemes
        public async Task<ActionResult> Index()
        {
            return View(await db.Schemes.ToListAsync());
        }

        [HttpGet]
        public async Task<ActionResult> DetailJson(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Scheme scheme = await db.Schemes
                                        .Where(s => s.Id == id)
                                        .FirstOrDefaultAsync();
            if (scheme == null)
            {
                return HttpNotFound();
            }

            var vm = GetSchemeViewModel(scheme);

            return Json(vm , JsonRequestBehavior.AllowGet);
        }

        // GET: Schemes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Scheme scheme = await db.Schemes
                                        .Where(s => s.Id == id)
                                        .FirstOrDefaultAsync();
            if (scheme == null)
            {
                return HttpNotFound();
            }

            var vm = GetSchemeViewModel(scheme);

            return View(vm);
        }

       

        // GET: Schemes/Create
        public ActionResult Create()
        {
            var vm = new SchemeViewModel();
            return View(vm);
        }

         [HttpGet]  //[ValidateAntiForgeryToken]        
        public  ActionResult Sample()
        {
             SchemeViewModel viewModel = new SchemeViewModel();
             viewModel.Name = "test";
             return Content(JsonConvert.SerializeObject(viewModel));
         }

        // POST: Schemes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]  //[ValidateAntiForgeryToken]        
        public async Task<ActionResult> Create(SchemeViewModel viewModel)
        {
          
            if (ModelState.IsValid)
            {
                var scheme = new Scheme() { 
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    Markets = viewModel.Markets,
                    Indicators = viewModel.Indicators,
                    BullBearTests = viewModel.BullBearTests,
                    BackTests = viewModel.BackTests,
                    PatternScanners = viewModel.PatternScanners,
                    Scanners = viewModel.Scanners,
                    ScannerFlag = viewModel.ScannerFlag,
                    CustomIndicatorsFlag = viewModel.CustomIndicatorsFlag,
                    LiveFlag = viewModel.LiveFlag,
                    CIAddFlag = viewModel.CIAddFlag,
                    ScannerAddFlag = viewModel.ScannerAddFlag,
                    SignalAddFlag = viewModel.SignalAddFlag,
                    TrendAddFlag = viewModel.TrendAddFlag,
                    PatternAddFlag = viewModel.PatternAddFlag                       
                };

                db.Schemes.Add(scheme);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        // GET: Schemes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Scheme scheme = await db.Schemes                                        
                                        .Where(s => s.Id == id)
                                        .FirstOrDefaultAsync();
            if (scheme == null)
            {
                return HttpNotFound();
            }           
            
            var vm = new SchemeViewModel() {
                Id = scheme.Id,
                Name = scheme.Name,
                Description = scheme.Description,
                Markets = scheme.Markets,
                Indicators = scheme.Indicators,
                BullBearTests = scheme.BullBearTests,
                BackTests = scheme.BackTests,
                PatternScanners = scheme.PatternScanners,
                Scanners = scheme.Scanners,
                ScannerFlag = scheme.ScannerFlag,
                CustomIndicatorsFlag = scheme.CustomIndicatorsFlag,
                LiveFlag = scheme.LiveFlag,
                CIAddFlag = scheme.CIAddFlag,
                ScannerAddFlag = scheme.ScannerAddFlag,
                SignalAddFlag = scheme.SignalAddFlag,
                TrendAddFlag = scheme.TrendAddFlag,
                PatternAddFlag = scheme.PatternAddFlag
            };

            return View(vm);
        }

        // POST: Schemes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SchemeViewModel viewModel) {
            if (ModelState.IsValid) {


                var users = await (from user in db.Users 
                            from chartuser in db.ChartUsers 
                            where user.Email == chartuser.Login && 
                                    user.SchemeId == viewModel.Id
                            select chartuser ).ToArrayAsync() ;
                foreach (var user in users)
                {
                    user.Live = viewModel.LiveFlag;
                    user.CI_Add = viewModel.CIAddFlag;
                    user.CustomIndicators = viewModel.CustomIndicatorsFlag;
                    user.Pattern_Add = viewModel.PatternAddFlag;
                    user.Scanner = viewModel.ScannerFlag;
                    user.Scanner_Add = viewModel.ScannerAddFlag;
                    user.Signal_Add = viewModel.SignalAddFlag;
                    user.Trend_Add = viewModel.TrendAddFlag;                    
                }
                Scheme scheme = await db.Schemes
                                        .Where(s => s.Id == viewModel.Id)
                                        .FirstOrDefaultAsync();
                
                scheme.Name = viewModel.Name;
                scheme.Description = viewModel.Description;
                scheme.Markets = viewModel.Markets;
                scheme.Indicators = viewModel.Indicators;
                scheme.BullBearTests = viewModel.BullBearTests;
                scheme.BackTests = viewModel.BackTests;
                scheme.PatternScanners = viewModel.PatternScanners;
                scheme.Scanners = viewModel.Scanners;
                scheme.LiveFlag = viewModel.LiveFlag;
                scheme.ScannerFlag = viewModel.ScannerFlag;
                scheme.CustomIndicatorsFlag = viewModel.CustomIndicatorsFlag;
                scheme.CIAddFlag = viewModel.CIAddFlag;
                scheme.ScannerAddFlag = viewModel.ScannerAddFlag;
                scheme.SignalAddFlag = viewModel.SignalAddFlag;
                scheme.TrendAddFlag = viewModel.TrendAddFlag;
                scheme.PatternAddFlag = viewModel.PatternAddFlag;

                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(viewModel);
        }

        // GET: Schemes/Delete/5
        [HttpPost]
        public async Task<ActionResult> DeleteA(int? id)
        {
            Scheme scheme = await db.Schemes.FindAsync(id);
            var vm = new { deleteScheme = scheme };
            
            db.Schemes.Remove(scheme);
            await db.SaveChangesAsync();

            
            var j = JsonConvert.SerializeObject(vm);

            return Content(j);
        }

        // GET: Schemes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Scheme scheme = await db.Schemes
                                        .Where(s => s.Id == id)
                                        .FirstOrDefaultAsync();
            if (scheme == null)
            {
                return HttpNotFound();
            }

            var vm = GetSchemeViewModel(scheme);
            return View(vm);
        }

        // POST: Schemes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Scheme scheme = await db.Schemes.FindAsync(id);
            db.Schemes.Remove(scheme);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Activate() {
            var vm = new ActivateSchemeViewModel();

            var schemes = await db.Schemes.ToArrayAsync();

            vm.Schemes = schemes.Select(s => new SelectListItem() {
                Text = string.Format("({0}) {1}", s.Name, s.Description),
                Value = s.Id.ToString()
            }).ToArray();
            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> Activate(ActivateSchemeViewModel vm) {

            var emails = GetEmails(vm.Users.InputStream).ToArray();

            Scheme scheme = await db.Schemes
                                        .Where(s => s.Id == vm.SelectedSchemeId)
                                        .FirstOrDefaultAsync();

            var backTests = (scheme.BackTests == null) ? new string[] { } : scheme.BackTests.ToString().Split(';');
            var bullBearTests = (scheme.BullBearTests == null) ? new string[]{} : scheme.BullBearTests.ToString().Split(';');
            var indicators = (scheme.Indicators == null) ? new string[]{} : scheme.Indicators.ToString().Split(';');
            var markets = (scheme.Markets == null) ? new string[] { } : scheme.Markets.ToString().Split(';');
            var patternScanners = (scheme.PatternScanners == null) ? new string[] { } : scheme.PatternScanners.ToString().Split(';');
            var scanners = (scheme.Scanners == null) ? new string[] { } : scheme.Scanners.ToString().Split(';');

            var errorEmails = new List<string>();            
            foreach (string email in emails) {
             
                var appuser = await db.Users.FirstOrDefaultAsync(x => x.Email == email);
                var uid = Guid.Parse(appuser.Id);
                var user = await db.ChartUsers.FirstOrDefaultAsync(x => x.Id == uid);
                if (user == null)
                {
                    errorEmails.Add(email);
                }
                else {
                   
                    var userId = user.Id;

                    user.Expires = vm.ExpiredDate.Value;
                    appuser.ModifiedDate = DateTime.Now; 
                   user.Scanner      = scheme.ScannerFlag; 
                   user.CustomIndicators     = scheme.CustomIndicatorsFlag  ;
                   user.CI_Add       = scheme.CIAddFlag  ;
                   user.Scanner_Add     = scheme.ScannerAddFlag  ;
                   user.Signal_Add     = scheme.SignalAddFlag  ;
                   user.Trend_Add    = scheme.TrendAddFlag  ;
                   user.Pattern_Add    = scheme.PatternAddFlag  ;
                   user.Live = scheme.LiveFlag;

                    await db.UserBackTests.Where(bt => bt.UserId == userId).DeleteAsync();
                    await db.UserBullBearTests.Where(bt => bt.UserId == userId).DeleteAsync();
                    await db.UserIndicators.Where(bt => bt.UserId == userId).DeleteAsync();
                    await db.UserMarkets.Where(bt => bt.UserId == userId).DeleteAsync();
                    await db.UserPatternScanners.Where(bt => bt.UserId == userId).DeleteAsync();
                    await db.UserScanners.Where(bt => bt.UserId == userId).DeleteAsync();

                    if (backTests.Any()) {
                        AddUserFunction(backTests, user, db.UserBackTests);
                    }
                    if (bullBearTests.Any()) {
                        AddUserFunction(bullBearTests, user, db.UserBullBearTests);
                    }
                    if (indicators.Any()) {
                        AddUserFunction(indicators, user, db.UserIndicators);
                        //user.CustomIndicators = true;
                    }
                    if (markets.Any()) {
                        AddUserFunction(markets, user, db.UserMarkets);
                    }
                    if (patternScanners.Any()) {
                        AddUserFunction(patternScanners, user, db.UserPatternScanners);
                        //user.Pattern_Add = true;
                    }
                    if (scanners.Any()) {
                        AddUserFunction(scanners, user, db.UserScanners);
                        //user.Scanner = true;
                        //user.Scanner_Add = true;
                    }
                    appuser.SchemeId = scheme.Id;
                }

                await db.SaveChangesAsync();
            }

            if (errorEmails.Any()) {
                this.TempData.AddErrorEmails(errorEmails);
                return RedirectToAction("ActivateErrorEmails");
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ActivateErrorEmails() {
            var emails = this.TempData.GetErrorEmails();
            return View(emails);
        }

        [HttpGet]
        public ActionResult ActivateFunction() {
            var vm = new ActivateFunctionViewModel();
            
            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> ActivateFunction(ActivateFunctionViewModel vm) {

            var emails = GetEmails(vm.Users.InputStream).ToArray();

            var backTests = (vm.BackTests == null) ? new string[] { } : vm.BackTests.ToString().Split(';');
            var bullBearTests = (vm.BullBearTests == null) ? new string[] { } : vm.BullBearTests.ToString().Split(';');
            var indicators = (vm.Indicators == null) ? new string[] { } : vm.Indicators.ToString().Split(';');
            var markets = (vm.Markets == null) ? new string[] { } : vm.Markets.ToString().Split(';');
            var patternScanners = (vm.PatternScanners == null) ? new string[] { } : vm.PatternScanners.ToString().Split(';');
            var scanners = (vm.Scanners == null) ? new string[] { } : vm.Scanners.ToString().Split(';');

            if (backTests.Any()) {
                await db.UserBackTests.Where(ub => backTests.Contains(ub.FormulaName)).DeleteAsync();
            }
            if (bullBearTests.Any()) {
                await db.UserBullBearTests.Where(ub => bullBearTests.Contains(ub.FormulaName)).DeleteAsync();
            }
            if (indicators.Any()) {
                await db.UserIndicators.Where(ub => indicators.Contains(ub.Indicator)).DeleteAsync();
            }
            if (markets.Any()) {
                await db.UserMarkets.Where(ub => markets.Contains(ub.Market)).DeleteAsync();
            }
            if (patternScanners.Any()) {
                await db.UserPatternScanners.Where(ub => patternScanners.Contains(ub.Scanner)).DeleteAsync();
            }
            if (scanners.Any()) {
                await db.UserScanners.Where(ub => scanners.Contains(ub.Scanner)).DeleteAsync();
            }

            var errorEmails = new List<string>();            
            foreach (string email in emails) {
                var appuser = await db.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
                var uid = Guid.Parse(appuser.Id);
                var user = await db.ChartUsers.Where(u => u.Id == uid).FirstOrDefaultAsync();
                if (appuser == null)
                {
                    errorEmails.Add(email);
                }
                else {
                    appuser.ModifiedDate = DateTime.Now;
                    user.Expires = vm.ExpiredDate.Value;                 

                    if (backTests.Any()) {
                        AddUserFunction(backTests, user, db.UserBackTests);
                    }
                    if (bullBearTests.Any()) {
                        AddUserFunction(bullBearTests, user, db.UserBullBearTests);
                    }
                    if (indicators.Any()) {
                        AddUserFunction(indicators, user, db.UserIndicators);
                        
                    }
                    if (markets.Any()) {
                        AddUserFunction(markets, user, db.UserMarkets);
                    }
                    if (patternScanners.Any()) {
                        AddUserFunction(patternScanners, user, db.UserPatternScanners);
                       
                    }
                    if (scanners.Any()) {
                        AddUserFunction(scanners, user, db.UserScanners);
                       
                    }
                }
                                
                await db.SaveChangesAsync();
            }

            if (errorEmails.Any()) {
                this.TempData.AddErrorEmails(errorEmails);
                return RedirectToAction("ActivateErrorEmails");
            }

            return RedirectToAction("Index");

        }

        public ActionResult SchemeUserData()
        {
            var vm = new SchemeUserViewModel();
            //var userGuid = Membership.GetUser();
            //var ug = (Guid)userGuid.ProviderUserKey;
            
            var sql1 = from x in db.UserMarkets  
                       from u in db.Users
                       where x.UserId.ToString() == u.Id
                       select u.Email + " - " + x.Market;

            var sql2 = from x in db.UserIndicators
                       from u in db.Users
                       where x.UserId.ToString() == u.Id
                       select u.Email + " - " + x.Indicator;

            var sql3 = from x in db.UserBullBearTests
                       from u in db.Users
                       where x.UserId.ToString() == u.Id
                       select u.Email + " - " + x.FormulaName;

            var sql4 = from x in db.UserBackTests
                       from u in db.Users
                       where x.UserId.ToString() == u.Id
                       select u.Email + " - " + x.FormulaName;

            var sql5 = from x in db.UserPatternScanners
                       from u in db.Users
                       where x.UserId.ToString() == u.Id
                       select u.Email + " - " + x.Scanner;

            var sql6 = from x in db.UserScanners
                       from u in db.Users
                       where x.UserId.ToString() == u.Id
                       select u.Email + " - " + x.Scanner;

            var sql12 = from x in Enumerable.Range(0, 5)
                       select "zarni@demo.com" + " - " + "HKEX";
            vm.UserMarkets = sql1 ;
            vm.UserIndicators = sql2;
            vm.UserBullBearTests = sql3;
            vm.UserBackTests = sql4;
            vm.UserPatternScanners = sql5;
            vm.UserScanners = sql6;

            return View(vm);
        }

        private void AddUserFunction<T>(string[] functions, ChartUser user, IDbSet<T> dbSet)             
            where T : class, IUserFunction, new() {

            foreach (string function in functions) {
                var model = new T() {
                    UserId = user.Id,
                    FunctionName = function
                };

                dbSet.Add(model);
            }
        }

        private IEnumerable<string> GetEmails(Stream stream) {
            var workbook = new XSSFWorkbook(stream);
            var sheet = workbook.GetSheetAt(0);  
            
            var iterator = sheet.GetRowEnumerator();
            var count = 1;
            while (iterator.MoveNext()) {
                if (count > 1) {
                    var row = (XSSFRow)iterator.Current;
                    var cell = row.GetCell(0);
                    if (cell != null) {
                        var value = cell.StringCellValue;
                        if (!string.IsNullOrWhiteSpace(value)) {
                            yield return value;
                        }
                    }
                }
                count++;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private SchemeViewModel GetSchemeViewModel(Scheme scheme) {
            var vm = new SchemeViewModel() {
                Id = scheme.Id,
                Name = scheme.Name,
                Description = scheme.Description,
                Markets = scheme.Markets,
                Indicators = scheme.Indicators,
                BullBearTests = scheme.BullBearTests,
                BackTests = scheme.BackTests,
                PatternScanners = scheme.PatternScanners,
                Scanners = scheme.Scanners,
                ScannerAddFlag = scheme.ScannerAddFlag,
                ScannerFlag = scheme.ScannerFlag,
                CIAddFlag = scheme.CIAddFlag,
                TrendAddFlag = scheme.TrendAddFlag,
                SignalAddFlag = scheme.SignalAddFlag,
                PatternAddFlag = scheme.PatternAddFlag,
                CustomIndicatorsFlag = scheme.CustomIndicatorsFlag
            };

            return vm;
        }
    }
}

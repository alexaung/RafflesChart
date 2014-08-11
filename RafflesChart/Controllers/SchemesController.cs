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

namespace RafflesChart.Controllers
{
    public class SchemesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Schemes
        public async Task<ActionResult> Index()
        {
            return View(await db.Scheme.ToListAsync());
        }

        // GET: Schemes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Scheme scheme = await db.Scheme
                                        .Include(s => s.UserMarkets)
                                        .Include(s => s.UserIndicators)
                                        .Include(s => s.UserBullBearTests)
                                        .Include(s => s.UserBackTests)
                                        .Include(s => s.PatternScanners)
                                        .Include(s => s.Scanners)
                                        .Where(s => s.Id == id)
                                        .FirstOrDefaultAsync();
            if (scheme == null) {
                return HttpNotFound();
            }

            var vm = await GetSchemeViewModelAsync(scheme);

            return View(vm);
        }

        // GET: Schemes/Create
        public async Task<ActionResult> Create()
        {
            var vm = new SchemeViewModel();

            var userMarkets = await db.UserMarkets.ToArrayAsync();
            var userIndicators = await db.UserIndicators.ToArrayAsync();
            var userBullBearTests = await db.UserBullBearTests.ToArrayAsync();
            var userBackTests = await db.UserBackTests.ToArrayAsync();
            var patternScanners = await db.PatternScanners.ToArrayAsync();
            var scanners = await db.Scanners.ToArrayAsync();

            vm.AllUserMarkets = userMarkets.Select(m => new SelectListItem() { 
                Text = m.Code, 
                Value = m.Id.ToString() 
            }).ToArray();

            vm.AllUserIndicators = userIndicators.Select(m => new SelectListItem() {
                Text = m.Code,
                Value = m.Id.ToString()
            }).ToArray();

            vm.AllUserBullBearTests = userBullBearTests.Select(m => new SelectListItem() {
                Text = m.Code,
                Value = m.Id.ToString()
            }).ToArray();

            vm.AllUserBackTests = userBackTests.Select(m => new SelectListItem() {
                Text = m.Code,
                Value = m.Id.ToString()
            }).ToArray();

            vm.AllPatternScanners = patternScanners.Select(m => new SelectListItem() {
                Text = m.Code,
                Value = m.Id.ToString()
            }).ToArray();

            vm.AllScanners = scanners.Select(m => new SelectListItem() {
                Text = m.Code,
                Value = m.Id.ToString()
            }).ToArray();

            return View(vm);
        }

        // POST: Schemes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SchemeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var scheme = new Scheme() { 
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    Scanner = viewModel.Scanner,
                    CustomIndicators = viewModel.CustomIndicators,
                    Live = viewModel.Live,
                    CiAdd = viewModel.CiAdd,
                    ScannerAdd = viewModel.ScannerAdd,
                    TrendAdd = viewModel.TrendAdd,
                    PatternAdd = viewModel.PatternAdd,
                    Expires = viewModel.Expires
                };

                viewModel.UserMarketIds = viewModel.UserMarketIds ?? new int[] { };
                viewModel.UserIndicatorIds = viewModel.UserIndicatorIds?? new int[] { };
                viewModel.UserBullBearTestIds= viewModel.UserBullBearTestIds ?? new int[] { };
                viewModel.UserBackTestIds = viewModel.UserBackTestIds ?? new int[] { };
                viewModel.PatternScannerIds = viewModel.PatternScannerIds ?? new int[] { };
                viewModel.ScannerIds = viewModel.ScannerIds ?? new int[] { };

                var selectedMarkets = await db.UserMarkets.Where(um => viewModel.UserMarketIds.Contains(um.Id)).ToArrayAsync();
                scheme.UserMarkets = selectedMarkets;

                var selectedIndicators = await db.UserIndicators.Where(um => viewModel.UserIndicatorIds.Contains(um.Id)).ToArrayAsync();
                scheme.UserIndicators = selectedIndicators;

                var selectedBullBearTests = await db.UserBullBearTests.Where(um => viewModel.UserBullBearTestIds.Contains(um.Id)).ToArrayAsync();
                scheme.UserBullBearTests = selectedBullBearTests;

                var selectedBackTests = await db.UserBackTests.Where(um => viewModel.UserBackTestIds.Contains(um.Id)).ToArrayAsync();
                scheme.UserBackTests = selectedBackTests;

                var selectedPatternScanners = await db.PatternScanners.Where(um => viewModel.PatternScannerIds.Contains(um.Id)).ToArrayAsync();
                scheme.PatternScanners = selectedPatternScanners;

                var selectedScanners = await db.Scanners.Where(um => viewModel.ScannerIds.Contains(um.Id)).ToArrayAsync();
                scheme.Scanners = selectedScanners;  

                db.Scheme.Add(scheme);
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
            Scheme scheme = await db.Scheme
                                        .Include(s => s.UserMarkets)
                                        .Include(s => s.UserIndicators)
                                        .Include(s => s.UserBullBearTests)
                                        .Include(s => s.UserBackTests)
                                        .Include(s => s.PatternScanners)
                                        .Include(s => s.Scanners)
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
                Scanner = scheme.Scanner,
                CustomIndicators = scheme.CustomIndicators,
                Live = scheme.Live,
                CiAdd = scheme.CiAdd,
                ScannerAdd = scheme.ScannerAdd,
                TrendAdd = scheme.TrendAdd,
                PatternAdd = scheme.PatternAdd,
                Expires = scheme.Expires,
                UserMarketIds = scheme.UserMarkets.Select(m => m.Id).ToArray()
            };

            var userMarkets = await db.UserMarkets.ToArrayAsync();
            var userIndicators = await db.UserIndicators.ToArrayAsync();
            var userBullBearTests = await db.UserBullBearTests.ToArrayAsync();
            var userBackTests = await db.UserBackTests.ToArrayAsync();
            var patternScanners = await db.PatternScanners.ToArrayAsync();
            var scanners = await db.Scanners.ToArrayAsync();

            vm.AllUserMarkets = userMarkets.Select(m => new SelectListItem() {
                Text = m.Code,
                Value = m.Id.ToString(),
                Selected = scheme.UserMarkets.Any(um => um.Id == um.Id)
            }).ToArray();

            vm.AllUserIndicators = userIndicators.Select(m => new SelectListItem() {
                Text = m.Code,
                Value = m.Id.ToString(),
                Selected = scheme.UserIndicators.Any(um => um.Id == um.Id)
            }).ToArray();

            vm.AllUserBullBearTests = userBullBearTests.Select(m => new SelectListItem() {
                Text = m.Code,
                Value = m.Id.ToString(),
                Selected = scheme.UserBullBearTests.Any(um => um.Id == um.Id)
            }).ToArray();

            vm.AllUserBackTests = userBackTests.Select(m => new SelectListItem() {
                Text = m.Code,
                Value = m.Id.ToString(),
                Selected = scheme.UserBackTests.Any(um => um.Id == um.Id)
            }).ToArray();

            vm.AllPatternScanners = patternScanners.Select(m => new SelectListItem() {
                Text = m.Code,
                Value = m.Id.ToString(),
                Selected = scheme.PatternScanners.Any(um => um.Id == um.Id)
            }).ToArray();

            vm.AllScanners = scanners.Select(m => new SelectListItem() {
                Text = m.Code,
                Value = m.Id.ToString(),
                Selected = scheme.Scanners.Any(um => um.Id == um.Id)
            }).ToArray();

            return View(vm);
        }

        // POST: Schemes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SchemeViewModel viewModel) {
            if (ModelState.IsValid) {
                Scheme scheme = await db.Scheme
                                        .Include(s => s.UserMarkets)
                                        .Include(s => s.UserIndicators)
                                        .Include(s => s.UserBullBearTests)
                                        .Include(s => s.UserBackTests)
                                        .Include(s => s.PatternScanners)
                                        .Include(s => s.Scanners)
                                        .Where(s => s.Id == viewModel.Id)
                                        .FirstOrDefaultAsync();

                scheme.Name = viewModel.Name;
                scheme.Description = viewModel.Description;
                scheme.Scanner = viewModel.Scanner;
                scheme.CustomIndicators = viewModel.CustomIndicators;
                scheme.Live = viewModel.Live;
                scheme.CiAdd = viewModel.CiAdd;
                scheme.ScannerAdd = viewModel.ScannerAdd;
                scheme.TrendAdd = viewModel.TrendAdd;
                scheme.PatternAdd = viewModel.PatternAdd;
                scheme.Expires = viewModel.Expires;
                scheme.UserMarkets.Clear();

                viewModel.UserMarketIds = viewModel.UserMarketIds ?? new int[] { };
                viewModel.UserIndicatorIds = viewModel.UserIndicatorIds ?? new int[] { };
                viewModel.UserBullBearTestIds = viewModel.UserBullBearTestIds ?? new int[] { };
                viewModel.UserBackTestIds = viewModel.UserBackTestIds ?? new int[] { };
                viewModel.PatternScannerIds = viewModel.PatternScannerIds ?? new int[] { };
                viewModel.ScannerIds = viewModel.ScannerIds ?? new int[] { };
                                
                var selectedMarkets = await db.UserMarkets.Where(um => viewModel.UserMarketIds.Contains(um.Id)).ToArrayAsync();
                foreach (var sm in selectedMarkets) {
                    scheme.UserMarkets.Add(sm);
                }

                var selectedIndicators = await db.UserIndicators.Where(um => viewModel.UserIndicatorIds.Contains(um.Id)).ToArrayAsync();
                foreach (var sm in selectedIndicators) {
                    scheme.UserIndicators.Add(sm);
                }

                var selectedBullBearTests = await db.UserBullBearTests.Where(um => viewModel.UserBullBearTestIds.Contains(um.Id)).ToArrayAsync();
                foreach (var sm in selectedBullBearTests) {
                    scheme.UserBullBearTests.Add(sm);
                }

                var selectedBackTests = await db.UserBackTests.Where(um => viewModel.UserBackTestIds.Contains(um.Id)).ToArrayAsync();
                foreach (var sm in selectedBackTests) {
                    scheme.UserBackTests.Add(sm);
                }

                var selectedPatternScanners = await db.PatternScanners.Where(um => viewModel.PatternScannerIds.Contains(um.Id)).ToArrayAsync();
                foreach (var sm in selectedPatternScanners) {
                    scheme.PatternScanners.Add(sm);
                }

                var selectedScanners = await db.Scanners.Where(um => viewModel.ScannerIds.Contains(um.Id)).ToArrayAsync();
                foreach (var sm in selectedScanners) {
                    scheme.Scanners.Add(sm);
                }

                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(viewModel);
        }

        // GET: Schemes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Scheme scheme = await db.Scheme
                                        .Include(s => s.UserMarkets)
                                        .Include(s => s.UserIndicators)
                                        .Include(s => s.UserBullBearTests)
                                        .Include(s => s.UserBackTests)
                                        .Include(s => s.PatternScanners)
                                        .Include(s => s.Scanners)
                                        .Where(s => s.Id == id)
                                        .FirstOrDefaultAsync();
            if (scheme == null)
            {
                return HttpNotFound();
            }

            var vm = await GetSchemeViewModelAsync(scheme);
            return View(vm);
        }

        // POST: Schemes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Scheme scheme = await db.Scheme.FindAsync(id);
            db.Scheme.Remove(scheme);
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

        private async Task<SchemeViewModel> GetSchemeViewModelAsync(Scheme scheme) {
            var vm = new SchemeViewModel() {
                Id = scheme.Id,
                Name = scheme.Name,
                Description = scheme.Description,
                Scanner = scheme.Scanner,
                CustomIndicators = scheme.CustomIndicators,
                Live = scheme.Live,
                CiAdd = scheme.CiAdd,
                ScannerAdd = scheme.ScannerAdd,
                TrendAdd = scheme.TrendAdd,
                PatternAdd = scheme.PatternAdd,
                Expires = scheme.Expires,
                UserMarketIds = scheme.UserMarkets.Select(m => m.Id).ToArray(),
                UserIndicatorIds = scheme.UserIndicators.Select(m => m.Id).ToArray(),
                UserBullBearTestIds = scheme.UserBullBearTests.Select(m => m.Id).ToArray(),
                UserBackTestIds = scheme.UserBackTests.Select(m => m.Id).ToArray(),
                PatternScannerIds = scheme.PatternScanners.Select(m => m.Id).ToArray(),
                ScannerIds = scheme.Scanners.Select(m => m.Id).ToArray(),

            };

            var userMarkets = await db.UserMarkets.ToArrayAsync();
            var userIndicators = await db.UserIndicators.ToArrayAsync();
            var userBullBearTests = await db.UserBullBearTests.ToArrayAsync();
            var userBackTests = await db.UserBackTests.ToArrayAsync();
            var patternScanners = await db.PatternScanners.ToArrayAsync();
            var scanners = await db.Scanners.ToArrayAsync();

            vm.AllUserMarkets = userMarkets.Select(m => new SelectListItem() {
                Text = m.Code,
                Value = m.Id.ToString(),
                Selected = scheme.UserMarkets.Any(um => um.Id == um.Id)
            }).ToArray();

            vm.AllUserIndicators = userIndicators.Select(m => new SelectListItem() {
                Text = m.Code,
                Value = m.Id.ToString(),
                Selected = scheme.UserIndicators.Any(um => um.Id == um.Id)
            }).ToArray();

            vm.AllUserBullBearTests = userBullBearTests.Select(m => new SelectListItem() {
                Text = m.Code,
                Value = m.Id.ToString(),
                Selected = scheme.UserBullBearTests.Any(um => um.Id == um.Id)
            }).ToArray();

            vm.AllUserBackTests = userBackTests.Select(m => new SelectListItem() {
                Text = m.Code,
                Value = m.Id.ToString(),
                Selected = scheme.UserBackTests.Any(um => um.Id == um.Id)
            }).ToArray();

            vm.AllPatternScanners = patternScanners.Select(m => new SelectListItem() {
                Text = m.Code,
                Value = m.Id.ToString(),
                Selected = scheme.PatternScanners.Any(um => um.Id == um.Id)
            }).ToArray();

            vm.AllScanners = scanners.Select(m => new SelectListItem() {
                Text = m.Code,
                Value = m.Id.ToString(),
                Selected = scheme.Scanners.Any(um => um.Id == um.Id)
            }).ToArray();
            return vm;
        }
    }
}

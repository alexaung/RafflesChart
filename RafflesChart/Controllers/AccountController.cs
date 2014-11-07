using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using NPOI.XSSF.UserModel;
using Postal;
using RafflesChart.Extensions;
using RafflesChart.Models;
using System.Drawing;
using Microsoft.AspNet.Identity.EntityFramework;
using RafflesChart.App_Start;
using NLog;
using EntityFramework.Extensions;
using System.Text;

namespace RafflesChart.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ActionResult Welcome()
        {
            return View();
        }
        public ActionResult FAQ()
        {
            return View();
        }


        public ActionResult SyncSingle()
        {
            using (var db = new ApplicationDbContext())
            {
               var cuser =  db.ChartUsers.FirstOrDefault(x => x.Login == "zarniaung006@gmail.com");
               cuser.Password = "blahblah";
               var userid = User.Identity.Name ;
               var user = db.Users.FirstOrDefault(x => x.UserName == userid);
               var tk = UserManager.GeneratePasswordResetToken(user.Id);

               UserManager.ResetPassword(user.Id.ToString(), tk, cuser.Password);

               db.SaveChanges();
            }
            return RedirectToAction("Welcome");
        }

        public ActionResult Sync()
        {
            using (var db = new ApplicationDbContext())
            {
                var cuser = db.ChartUsers.ToList();
                foreach (var item in cuser.AsParallel())
	            {
                    //item.Password = "blahblah";
                    if ( string.IsNullOrEmpty( item.Password) ==false &&  item.Password.Length < 30)
                    { 
                        var user = db.Users.FirstOrDefault(x => x.Email == item.Login);
                        if (user != null)
                        {
                            var tk = UserManager.GeneratePasswordResetToken(user.Id);

                            UserManager.ResetPassword(user.Id.ToString(), tk, item.Password);
                        }
                    }
	            }
                
                //db.SaveChanges();
            }
            return RedirectToAction("Welcome");
        }


        public ActionResult UserGuide()
        {
            return View();
        }

        public ActionResult ScriptWriting()
        {
            return View();
        }
        
        public ActionResult Tutorial()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult AccountRegisterMessage()
        {
            return View();
        }

        public ApplicationUserManager UserManager {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [HttpGet]        
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetUsersJson(SearchKey suser)
        {
            var vm = await GetUserViewModel(suser);
            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetUsers(SearchKey suser)
        {
            var vm = await GetUserViewModel(suser);
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles="Admin")]
        public async Task<ActionResult> DeletePicked(string[] picked)
        {
            using (var db = new ApplicationDbContext())
            {
                var users = await db.Users.Where(x=> picked.Contains( x.Email)).ToArrayAsync();
                foreach (var item in users)
	            {
		            db.Users.Remove(item);
                   
	            }
                
                var chartuser = await db.ChartUsers.Where(x=> picked.Contains(x.Login)).ToArrayAsync();
                foreach (var item in chartuser)
                {
                    var userId = item.Id;
                    db.ChartUsers.Remove(item); 
                   
                    await db.UserBackTests.Where(bt => bt.UserId == userId).DeleteAsync();
                    await db.UserBullBearTests.Where(bt => bt.UserId == userId).DeleteAsync();
                    await db.UserIndicators.Where(bt => bt.UserId == userId).DeleteAsync();
                    await db.UserMarkets.Where(bt => bt.UserId == userId).DeleteAsync();
                    await db.UserPatternScanners.Where(bt => bt.UserId == userId).DeleteAsync();
                    await db.UserScanners.Where(bt => bt.UserId == userId).DeleteAsync();
                }


               

                
                await db.SaveChangesAsync();
            }
            return Json(true);
        }
        private ApplicationDbContext db = new ApplicationDbContext();

        public async Task<List<string>> UpdateUserProperties(int schemeid,  string[] emails,string exp)
        {
            Scheme scheme = await db.Schemes
                                        .Where(s => s.Id == schemeid)
                                        .FirstOrDefaultAsync();

            var backTests = (scheme.BackTests == null) ? new string[] { } : scheme.BackTests.ToString().Split(';');
            var bullBearTests = (scheme.BullBearTests == null) ? new string[] { } : scheme.BullBearTests.ToString().Split(';');
            var indicators = (scheme.Indicators == null) ? new string[] { } : scheme.Indicators.ToString().Split(';');
            var markets = (scheme.Markets == null) ? new string[] { } : scheme.Markets.ToString().Split(';');
            var patternScanners = (scheme.PatternScanners == null) ? new string[] { } : scheme.PatternScanners.ToString().Split(';');
            var scanners = (scheme.Scanners == null) ? new string[] { } : scheme.Scanners.ToString().Split(';');

            var errorEmails = new List<string>();
            foreach (string email in emails)
            {

                var appuser = await db.Users.FirstOrDefaultAsync(x => x.Email == email);

                if (appuser == null)
                {
                    errorEmails.Add(email);
                }
                else
                {
                    var uid = Guid.Parse(appuser.Id);
                    var user = await db.ChartUsers.FirstOrDefaultAsync(x => x.Id == uid);
                    if (user == null)
                    {
                        errorEmails.Add(email);
                        continue;
                    }
                    var userId = user.Id;

                    user.Expires = DateTime.Parse(exp);
                    appuser.ModifiedDate = DateTime.Now;
                    user.Scanner = scheme.ScannerFlag;
                    user.CustomIndicators = scheme.CustomIndicatorsFlag;
                    user.CI_Add = scheme.CIAddFlag;
                    user.Scanner_Add = scheme.ScannerAddFlag;
                    user.Signal_Add = scheme.SignalAddFlag;
                    user.Trend_Add = scheme.TrendAddFlag;
                    user.Pattern_Add = scheme.PatternAddFlag;
                    user.Live = scheme.LiveFlag;

                    await db.UserBackTests.Where(bt => bt.UserId == userId).DeleteAsync();
                    await db.UserBullBearTests.Where(bt => bt.UserId == userId).DeleteAsync();
                    await db.UserIndicators.Where(bt => bt.UserId == userId).DeleteAsync();
                    await db.UserMarkets.Where(bt => bt.UserId == userId).DeleteAsync();
                    await db.UserPatternScanners.Where(bt => bt.UserId == userId).DeleteAsync();
                    await db.UserScanners.Where(bt => bt.UserId == userId).DeleteAsync();

                    if (backTests.Any())
                    {
                        AddUserFunction(backTests, user, db.UserBackTests);
                    }
                    if (bullBearTests.Any())
                    {
                        AddUserFunction(bullBearTests, user, db.UserBullBearTests);
                    }
                    if (indicators.Any())
                    {
                        AddUserFunction(indicators, user, db.UserIndicators);
                        //user.CustomIndicators = true;
                    }
                    if (markets.Any())
                    {
                        AddUserFunction(markets, user, db.UserMarkets);
                    }
                    if (patternScanners.Any())
                    {
                        AddUserFunction(patternScanners, user, db.UserPatternScanners);
                        //user.Pattern_Add = true;
                    }
                    if (scanners.Any())
                    {
                        AddUserFunction(scanners, user, db.UserScanners);
                        //user.Scanner = true;
                        //user.Scanner_Add = true;
                    }
                    appuser.SchemeId = scheme.Id;
                }

                await db.SaveChangesAsync();
            }
            return errorEmails;
        }

        private void AddUserFunction<T>(string[] functions, ChartUser user, IDbSet<T> dbSet)
          where T : class, IUserFunction, new()
        {

            foreach (string function in functions)
            {
                var model = new T()
                {
                    UserId = user.Id,
                    FunctionName = function
                };

                dbSet.Add(model);
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> SchemePicked(string[] picked, int scheme,string exp)
        {
            using (var db = new ApplicationDbContext())
            {
                var users = await (
                        from user in db.Users 
                        from cuser in db.ChartUsers
                        where  picked.Contains(user.Email)
                         && user.Email == cuser.Login
                        select new { user,cuser}
                            ).ToArrayAsync();

                var schemedb = await db.Schemes.FirstOrDefaultAsync(x => x.Id == scheme);
               
                foreach (var item in users)
                {                  
                    item.user.SchemeId = scheme;
                    item.user.ModifiedDate = DateTime.Now;
                    item.cuser.Live = schemedb.LiveFlag;
                    item.cuser.CustomIndicators = schemedb.CustomIndicatorsFlag;
                    item.cuser.Scanner = schemedb.ScannerFlag;
                    item.cuser.CI_Add = schemedb.CIAddFlag;
                    item.cuser.Pattern_Add = schemedb.PatternAddFlag;
                    item.cuser.Scanner_Add = schemedb.SignalAddFlag;
                    item.cuser.Trend_Add = schemedb.TrendAddFlag;
                    item.cuser.Signal_Add = schemedb.SignalAddFlag;
                }

                await db.SaveChangesAsync();

                await UpdateUserProperties(scheme, picked, exp);
            }
            return Json(true);
        }

        public async Task<ActionResult> Download()
        {
            string header = "Name,Email,PhoneNumber,Role,Scheme,Expires" + 
                    ",Live,CustomIndicators,Scanner,CiAdd,ScannerAdd,PatternAdd,SignalAdd,TrendAdd";

           
            string filename = "users.csv";

            var skey = new SearchKey();
            skey.Name = Request.Form["Name"];
            skey.Email = Request.Form["Email"];
            skey.PhoneNumber = Request.Form["PhoneNumber"];
            skey.SelectedScheme = Request.Form["SelectedScheme"];

            var datalist = await GetUserViewModel(skey);
            //var data = Encoding.UTF8.GetBytes(billcsv);
            var data = new StringBuilder();
            data.AppendLine(header);
            foreach (var item in datalist)
            {
              data.Append(item.Name);
              data.Append("," + item.Email);
              data.Append("," + item.PhoneNumber);
              data.Append("," + item.Role);
              data.Append("," + item.Scheme);
              data.Append(",\"" + item.ExpiresDate + "\"");
              data.Append("," + item.Live);
                          
              data.Append("," + item.CustomIndicators);
              data.Append("," + item.Scanner);
                         
              data.Append("," + item.CiAdd);
              data.Append("," + item.ScannerAdd);
              data.Append("," + item.PatternAdd);
              data.Append("," + item.SignalAdd );
              data.Append("," + item.TrendAdd);
              data.AppendLine();
            }
            var fdata = Encoding.UTF8.GetBytes(data.ToString());
            return File(fdata, "text/csv", filename);
        }

        private async Task<List<UserViewModel>> GetUserViewModel(SearchKey suser)
        {
            using (var db = new ApplicationDbContext())
            {
                var usr = suser.Name ?? "";
                var email = suser.Email ?? "";
                var ph = suser.PhoneNumber ?? "";
                var scheme = suser.SelectedScheme ?? "";
               
                var uids = await db.Users.Where(x => x.Name.StartsWith(usr) &&
                                        x.Email.StartsWith(email) &&
                                        x.PhoneNumber.StartsWith(ph)
                            ).Select(x => new { x.Name, x.Email, x.PhoneNumber, x.CreatedDate, Id = x.Id, x.Roles, x.SchemeId,x.ModifiedDate }).ToArrayAsync();

                var schemes = await db.Schemes.ToListAsync();
                
                List<UserViewModel> vm = new List<UserViewModel>();
                var roles = await db.Roles.ToListAsync();
                var emails = uids.Select(x => x.Email);
                var userids = uids.Select(x =>  Guid.Parse( x.Id));
                if (string.IsNullOrEmpty(suser.SelectedScheme) == false)
                {
                    var selectedscheme = int.Parse(suser.SelectedScheme);
                    emails =  uids.Where(x => x.SchemeId == selectedscheme).Select(x=> x.Email);
                }
                var usersql = await (from cu in db.ChartUsers
                            where emails.Contains(cu.Login)
                                  select cu).ToArrayAsync();
                var users = from u in uids
                            from cu in usersql
                            where u.Email == cu.Login
                            select new { CU = cu, UID = u };
                var usermkts = db.UserMarkets.Where(u => userids.Contains(u.UserId));
                               
                foreach (var ur in users.OrderByDescending(x=> x.UID.ModifiedDate))
                {
                    var item = new UserViewModel();
                    var sql = (from r in roles
                               from r2 in ur.UID.Roles
                               where r.Id == r2.RoleId
                               select r.Name).ToList();
                    var rr = string.Join(",", sql);

                    item.Name = ur.UID.Name;
                    item.Email = ur.UID.Email;
                    item.PhoneNumber = ur.UID.PhoneNumber;
                    item.Role = rr ?? "User";
                    item.Scheme = "na";
                    
                    DateTime d1 = new DateTime(1970, 1, 1);
                    DateTime d2 = (DateTime)ur.CU.Expires;
                    var x = new TimeSpan(d2.AddHours(-8).Ticks - d1.Ticks).TotalMilliseconds;
                    item.Expires = x;
                    if (ur.UID.ModifiedDate.HasValue)
                    {
                        item.ModifiedDate = new TimeSpan(ur.UID.ModifiedDate.Value.AddHours(-8).Ticks - d1.Ticks ).TotalMilliseconds;
                    }
                    item.ExpiresDate = ur.CU.Expires;

                    if (ur.UID.CreatedDate.HasValue)
                    {
                        item.CreatedDate = new TimeSpan(ur.UID.CreatedDate.Value.AddHours(-8).Ticks - d1.Ticks).TotalMilliseconds;
                    }
                    item.ExpiresDate = ur.CU.Expires;

                    item.Live = ur.CU.Live;
                    
                    item.CustomIndicators = ur.CU.CustomIndicators; 
                    item.Scanner = ur.CU.Scanner;
                    
                    item.CiAdd = ur.CU.CI_Add;
                    item.ScannerAdd = ur.CU.Scanner_Add;
                    item.PatternAdd = ur.CU.Pattern_Add;                   
                    item.SignalAdd = ur.CU.Signal_Add;
                    item.TrendAdd = ur.CU.Trend_Add;
                    
                    item.Picked = false;
                    if (ur.UID.SchemeId.HasValue)
                    {
                        item.Scheme = schemes.FirstOrDefault(xx => xx.Id == ur.UID.SchemeId).Name;
                    }

                    var mkts = usermkts.Where(m=> m.UserId == ur.CU.Id).Select(r=> r.Market).Aggregate((p,q) => p + "," + q );
                    item.UserMarkets = mkts;
                    vm.Add(item);
                }
                return vm;
            }
        }

        public bool SchemeCheck(int? dbScheme, string scheme)
        {
            if (string.IsNullOrEmpty(scheme))
            {
                return true;
            }
            else
            {
                if (dbScheme.HasValue)
                {
                    return (dbScheme.Value == int.Parse(scheme));
                }
                else {
                    return false;
                }
            }
        }

        [Authorize (Roles="Admin")]
        public async Task<ActionResult> Edit(string email)
        {
            var user = new ChartUserViewModel();
            var appuser = UserManager.FindByEmail(email);
            ChartUser chuser;
            using (var db = new ApplicationDbContext())
            {
                chuser = db.ChartUsers.FirstOrDefault(x => x.Id.ToString() == appuser.Id);
                
                var schemes = await db.Schemes.ToArrayAsync();

                user.Schemes = schemes.Select(s => new SelectListItem()
                {
                    Text = string.Format("({0}) {1}", s.Name, s.Description),
                    Value = s.Id.ToString()
                }).ToArray();
                user.ApplicationUserModel = appuser;
                user.ChartUserModel = chuser;
                user.SelectedScheme = appuser.SchemeId.ToString();
            }
           
           
            using (var db = new ApplicationDbContext())
            {
            }
           
            return View(user);
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Details(string email)
        {
            var appuser = UserManager.FindByEmail(email);
            ChartUser chuser;
            using (var db = new ApplicationDbContext())
            {
                chuser = db.ChartUsers.FirstOrDefault(x => x.Id.ToString() == appuser.Id);

            }
            var user = new ChartUserViewModel()
            {
                ApplicationUserModel = appuser,
                ChartUserModel = chuser
            };
            return View(user);       
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteConfirm(string email)
        {
            var appuser = UserManager.FindByEmail(email);
            ChartUser chuser;
            using (var db = new ApplicationDbContext())
            {
                chuser = db.ChartUsers.FirstOrDefault(x => x.Id.ToString() == appuser.Id);

            }
            var user = new ChartUserViewModel()
            {
                ApplicationUserModel = appuser,
                ChartUserModel = chuser
            };
            return View(user);            
        }


        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(string email)
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            

            ApplicationDbContext db = new ApplicationDbContext();
            logger.Trace("Getting AspnetUser");
            var us = await db.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            logger.Trace("Deleting AspnetUser");

            Guid ug = Guid.Parse(us.Id);
            db.Users.Remove(us);

            logger.Trace("Geting User");
            var chartus = await db.ChartUsers.Where(u => u.Login == email).FirstOrDefaultAsync();
            logger.Trace("Removing User");
            db.ChartUsers.Remove(chartus);

            
            logger.Trace("Removing User Back Test");
            db.UserBackTests.RemoveRange(db.UserBackTests.Where(x => x.UserId == ug));
            logger.Trace("Removing Use Bull Bear Test");
            db.UserBullBearTests.RemoveRange(db.UserBullBearTests.Where(x => x.UserId == ug));
            logger.Trace("Removing User Indicator ");
            db.UserIndicators.RemoveRange(db.UserIndicators.Where(x => x.UserId == ug));
            logger.Trace("Removing User Marker");
            db.UserMarkets.RemoveRange(db.UserMarkets.Where(x => x.UserId == ug));
            logger.Trace("Removing User Pattern Scanner ");
            db.UserPatternScanners.RemoveRange(db.UserPatternScanners.Where(x => x.UserId == ug));
            logger.Trace("Removing User Scanner");
            db.UserScanners.RemoveRange(db.UserScanners.Where(x => x.UserId == ug));

            logger.Trace("Saving Final DB");
            int result = await db.SaveChangesAsync();
            if (result > 0)
            {
                logger.Trace(" Result Greater than zero");
            }
            else
            {
                logger.Trace("Result not greater zero");
            }
            return RedirectToAction("GetUsers");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(ChartUserViewModel vm)
        {
            ApplicationUser user = vm.ApplicationUserModel;
            ChartUser chrtuser = vm.ChartUserModel;
            ApplicationDbContext db = new ApplicationDbContext();
            var us = await db.Users.Where(u => u.Email == user.Email).FirstOrDefaultAsync();
            us.Name = user.Name;
            us.PhoneNumber = user.PhoneNumber;
            us.ModifiedDate = DateTime.Now;
            if (string.IsNullOrEmpty(vm.SelectedScheme) == false)
            {
                us.SchemeId = int.Parse( vm.SelectedScheme);
            }

            var cus = await db.ChartUsers.Where(u => u.Login == user.Email).FirstOrDefaultAsync();
            cus.Scanner = chrtuser.Scanner;
            cus.CustomIndicators = chrtuser.CustomIndicators;
            cus.Live = chrtuser.Live;
            cus.CI_Add = chrtuser.CI_Add;

            cus.Scanner_Add = chrtuser.Scanner_Add;
            cus.Signal_Add = chrtuser.Signal_Add;
            cus.Trend_Add = chrtuser.Trend_Add;
            cus.Pattern_Add = chrtuser.Pattern_Add;
            cus.Expires = chrtuser.Expires;
           int result = await  db.SaveChangesAsync();
           if (result>0)
           { }
                return RedirectToAction("GetUsers");
          

        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.Email, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);
                    Session["activeuser"] = model.Email;
                    if (string.IsNullOrEmpty (returnUrl) || "/".Equals(returnUrl))
                    {
                        var adminrole = UserManager.GetRoles(user.Id);
                        if (adminrole.Contains("Admin"))
                        {
                            return RedirectToLocal(returnUrl);
                        }
                        else 
                        { 
                            return RedirectToAction("Welcome");
                        }
                    }
                    else
                    {
                        return RedirectToLocal(returnUrl);
                    }
                  
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]        
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!CaptchaMvc.HtmlHelpers.CaptchaHelper.IsCaptchaValid(this, "error"))
                {
                    ModelState.AddModelError("captcha", "Invalid captcha");
                    return View();
                }
                var user = new ApplicationUser() { 
                    Name = model.Name,
                    PhoneNumber = model.PhoneNumber,
                    PhoneNumberConfirmed = true,
                    UserName = model.Email,
                    Email = model.Email ,
                    //Expires = DateTime.ParseExact("2030-Dec-31", "yyyy-MMM-dd",null)
                };
                
                var passwordValidator = (PasswordValidator)UserManager.PasswordValidator;
                
                //var password = Membership.GeneratePassword(passwordValidator.RequiredLength, 0);

                var txt = shuffle<char>(AsciiNumber());
                var cpt = String.Join("", txt.Take(6).ToArray());
                IdentityResult result = await UserManager.CreateAsync(user, cpt);


                if (result.Succeeded)
                {
                    // await SignInAsync(user, isPersistent: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");                    
                    
                    var chartuser = new ChartUser() {
                    Login = user.Email,
                    Id  = Guid.Parse(user.Id),
                    Password = cpt,
                    Expires = DateTime.ParseExact("2030-Dec-31", "yyyy-MMM-dd", null)
                     };

                    using(var db = new ApplicationDbContext())
	                {
                        db.ChartUsers.Add(chartuser);
                        var usertoconfirm = db.Users.FirstOrDefault(x=>x.Email == user.Email);
                        usertoconfirm.EmailConfirmed = true;
                        usertoconfirm.CreatedDate = DateTime.Now;
                        db.SaveChanges();
	                } 
                    await SendUserRegistrationEmailAsync(model.Email, cpt);
                    TempData["EmailedPassword"] = "Your password has been emailed. Please use this to login.";
                    return RedirectToAction("AccountRegisterMessage", "Account");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private static async Task SendPasswordResetEmailAsync(string email, string resetmessage)
        {

            dynamic mail = new Email("ResetPassword");
            mail.To = email;
            mail.Email = email;
            mail.ResetMessage = resetmessage;            
            await mail.SendAsync();
        }

        private static async Task SendUserRegistrationEmailAsync(string email, string password) {

            dynamic mail = new Email("UserRegister");
            mail.To = email;
            mail.Email = email;
            mail.Password = password;
            await mail.SendAsync();
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null) 
            {
                return View("Error");
            }

            IdentityResult result = await UserManager.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
            {
                return View("ConfirmEmail");
            }
            else
            {
                AddErrors(result);
                return View();
            }
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    ModelState.AddModelError("", "The user either does not exist or is not confirmed.");
                    return View();
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                 string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                 var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                 //await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                 await SendPasswordResetEmailAsync(model.Email,  "Please reset your password by clicking here: " + callbackUrl );
                 return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
    
        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            if (code == null) 
            {
                return View("Error");
            }
            return View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "No user found.");
                    return View();
                }
                IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
                if (result.Succeeded)
                {
                    using (var db = new ApplicationDbContext())
                    {
                        var cuser = db.ChartUsers.FirstOrDefault(x => x.Login == user.Email);
                        cuser.Password = model.Password;
                        db.SaveChanges();
                    }
                    return RedirectToAction("ResetPasswordConfirmation", "Account");
                }
                else
                {
                    AddErrors(result);
                    return View();
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                await SignInAsync(user, isPersistent: false);
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                        await SignInAsync(user, isPersistent: false);
                        
                        using (var db = new ApplicationDbContext())
                        {
                            var cuser = db.ChartUsers.FirstOrDefault(x => x.Login == user.Email);
                            cuser.Password = model.NewPassword;
                            db.SaveChanges();
                        }
                        // return RedirectToAction("ResetPasswordConfirmation", "Account");
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };
                IdentityResult result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        
                        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        // SendEmail(user.Email, callbackUrl, "Confirm your account", "Please confirm your account by clicking this link");
                        
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        private IEnumerable<string> GetEmails(Stream stream)
        {
            var workbook = new XSSFWorkbook(stream);
            var sheet = workbook.GetSheetAt(0);

            var iterator = sheet.GetRowEnumerator();
            var count = 1;
            while (iterator.MoveNext())
            {
                if (count > 1)
                {
                    var row = (XSSFRow)iterator.Current;
                    var cell = row.GetCell(0);
                    if (cell != null)
                    {
                        var value = cell.StringCellValue;
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            yield return value;
                        }
                    }
                }
                count++;
            }
        }
        
        [HttpPost]
        public async Task<ActionResult> UploadSpecial(UploadSpecialViewModel vm)
        {
            var users = GetEmails(vm.Users.InputStream).ToArray();
           
            var errorEmails = new List<string>();
            var succesUserCount = 0;

            
            using (var db = new ApplicationDbContext())
            {
                var goodEmails = db.Users.Where(x => users.Contains(x.Email)).ToArray();
                var memrole = db.Roles.FirstOrDefault(x => x.Name == "SpecialMember");
                List<string> successEmails = new List<string>();
                string rid = "";
                if (memrole == null)
                {
                    var created = db.Roles.Add(new IdentityRole("SpecialMember"));
                    rid = created.Id;
                }
                else
                {
                    rid = memrole.Id;
                }
                if (string.IsNullOrEmpty(rid) ==false)
                {
                    memrole.Users.ToList().ForEach(x=> memrole.Users.Remove(x));
                    succesUserCount = goodEmails.Count();
                    foreach (var item in goodEmails)
                    {
                        var urole = new IdentityUserRole();
                        urole.RoleId = rid;
                        urole.UserId = item.Id;
                        item.Roles.Add(urole);
                        successEmails.Add(item.Email);
                    }
                    await db.SaveChangesAsync();
                }
                errorEmails = users.Except(successEmails).ToList();
            }

            var uploadUserResult = new UploadUserResult()
            {
                SuccessUserCount = succesUserCount,
                ErrorUserEmails = errorEmails
            };

            this.TempData.AddUploadUserResult(uploadUserResult);
            return RedirectToAction("UploadUserResult");
        }
        //
        // POST: /Account/UploadUsers
        [HttpPost]
        public async Task<ActionResult> UploadUsers(ActivateFunctionViewModel vm) {
            var users = GetUsers(vm.Users.InputStream).ToArray();
            var errorEmails = new List<string>();
            var succesUserCount = 0;
            var passwordValidator = (PasswordValidator)UserManager.PasswordValidator;

            

            foreach (var user in users) {
               
                var nums = shuffle<char>(NumOToT());
               // var raw = txt.Take(4).Concat(nums.Take(2));
               // var reshuf = shuffle<char>(raw.ToList());
                //var cpt = String.Join("", reshuf.ToArray());

                //var password = Membership.GeneratePassword(passwordValidator.RequiredLength, 0);
                var txt = shuffle<char>(AsciiNumber());
               var  cpt = String.Join("", txt.Take(6).ToArray());
                var result = await UserManager.CreateAsync(user, cpt);

                if (result.Succeeded) {
                    succesUserCount++;
                    var chartuser = new ChartUser()
                    {
                        Id = Guid.Parse(user.Id),
                        Login = user.Email,
                        Password = cpt,
                        Expires = DateTime.ParseExact("2030-Dec-31", "yyyy-MMM-dd", null)
                    };
                    using (var context = new ApplicationDbContext())
                    {
                        context.ChartUsers.Add(chartuser);
                        await context.SaveChangesAsync();
                    }

                    await SendUserRegistrationEmailAsync(user.Email, cpt);
                }
                else {
                    errorEmails.Add(user.Email);
                }
            }

            var uploadUserResult = new UploadUserResult() {
                SuccessUserCount = succesUserCount,
                ErrorUserEmails = errorEmails

            };

            this.TempData.AddUploadUserResult(uploadUserResult);
            return RedirectToAction("UploadUserResult");
        }

        //
        // GET: /Account/UploadUsers
        [Authorize(Roles = "Admin")]
        public ActionResult UploadUsers() {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult UploadSpecial()
        {
            return View();
        }


        public ActionResult UploadUserResult() {
            var result = this.TempData.GetUploadUserResult();
            return View(result);
        }

        //
        [SessionExpireFilter]
        public async Task<ActionResult> GetUsers() {
            using (var db = new ApplicationDbContext()) 
            {
                var vm = new SearchUserResult();
                
                List<UserViewModel> list = new List<UserViewModel>();
                vm.Result = list;
                var schemes = await db.Schemes.ToArrayAsync();
                vm.Schemes = schemes.Select(s => new SelectListItem()
                {
                    Text = string.Format("({0}) {1}", s.Name, s.Description),
                    Value = s.Id.ToString()
                }).ToArray();
                return View(vm);
            }
        }        

        private IEnumerable<ApplicationUser> GetUsers(Stream stream) {
            var workbook = new XSSFWorkbook(stream);
            var sheet = workbook.GetSheetAt(0);
            
            var iterator = sheet.GetRowEnumerator();
            var count = 1;
            var errorMessage = String.Empty;
            while (iterator.MoveNext()) {
                if (count > 1) {
                    var row = (XSSFRow)iterator.Current;
                    var name = row.GetCell(0);
                    var handPhone = row.GetCell(1);
                    var email = row.GetCell(2);
                    var user = new ApplicationUser();
                    string nameval = "";
                    if (name != null) {
                        nameval = name.StringCellValue;
                        if (string.IsNullOrWhiteSpace(nameval)) {
                            errorMessage = "Error at Row : " + count;
                        }
                    }
                    string phoneval = "";
                    if (handPhone != null) {
                      
                        try
                        {
                            phoneval = handPhone.NumericCellValue.ToString();
                        }
                        catch (Exception)
                        {
                            phoneval = handPhone.StringCellValue;
                        }
                        
                        if (string.IsNullOrWhiteSpace(phoneval)) {
                            errorMessage = "Error at Row : " + count;
                        }
                    }
                    string emailval ="";
                    if (email != null) {
                        emailval = email.StringCellValue;
                        if (string.IsNullOrWhiteSpace(emailval)) {
                            errorMessage = "Error at Row : " + count;
                        }
                    }     
                    
                    user.Name = nameval;
                       user.PhoneNumber =phoneval;
                        user.PhoneNumberConfirmed = true;
                        user.UserName = emailval;
                        user.Email =emailval;
                        user.EmailConfirmed = true;
                        user.CreatedDate = DateTime.Now;
                        user.ModifiedDate = DateTime.Now;
                        //user.Expires = DateTime.ParseExact("2030-Dec-31", "yyyy-MMM-dd", null);
                    yield return user;
                }
                count++;
            }

        }

        private List<char> SmallAscii()
        {
            var az = Enumerable.Range('a', 'z' - 'a' + 1).
                      Select(c => (char)c);

            return az.ToList();

        }

        private List<char> NumOToT()
        {
            var ot = Enumerable.Range(0, 10).Select(i => Convert.ToChar(i.ToString()));
            return ot.ToList();

        }

        private static List<char> AsciiNumber()
        {
            var az = Enumerable.Range('A', 'Z' - 'A' + 1).
                      Select(c => (char)c);

            var ot = Enumerable.Range(0, 10).Select(i => Convert.ToChar(i.ToString()));

            return az.Concat(ot).ToList();

        }


        public static void GenerateCaptcha(out string cpt, out byte[] bytes)
        {
            Bitmap Bmp = new Bitmap(100, 20);
            Graphics gx = Graphics.FromImage(Bmp);

            var txt = shuffle<char>(AsciiNumber());
            cpt = String.Join("", txt.Take(6).ToArray());

            gx.Clear(Color.Orange);

            gx.DrawString(cpt, new Font("Arial", 11),
                            new SolidBrush(Color.Blue), new PointF(1, 2));
            var cvtr = new ImageConverter();
            bytes = cvtr.ConvertTo(Bmp, typeof(byte[])) as byte[];
        }


        [AllowAnonymous]
        public ActionResult GetCaptcha()
        {
            string cpt;
            byte[] bytes;
            GenerateCaptcha(out cpt, out bytes);

            //var mem = new System.IO.FileStream(@"d:\zarni\ctcha.jpeg", FileMode.Create);
            //Bmp.Save(@"d:\zarni\ctcha4.png", System.Drawing.Imaging.ImageFormat.Png);
            //var stream = new MemoryStream();
            //Bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            Session["Captcha"] = cpt;
            return File(bytes, "image/png");
            //Response.BinaryWrite(bytes);

            // return View();
        }
       

        #region Helpers

        private static List<T> shuffle<T>(List<T> lsin)
        {
            Random r = new Random();
            int n = lsin.Count;
            List<T> lsOut = new List<T>();
            lsOut.AddRange(lsin);
            while (n > 1)
            {
                var k = r.Next(n);
                n--;
                T temp = lsOut[n];
                lsOut[n] = lsOut[k];
                lsOut[k] = temp;
            }
            return lsOut;
        }

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }


        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}
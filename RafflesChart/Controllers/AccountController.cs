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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetUsers(SearchUser suser)
        {
            using (var db = new ApplicationDbContext())
            {
                var usr = suser.Name?? "";
                var email = suser.Email ?? "";
                var ph = suser.PhoneNumber ?? "";
                var users = await db.Users.Where(x=> x.Name.StartsWith(usr) && 
                                        x.Email.StartsWith(email) && 
                                        x.PhoneNumber.StartsWith(ph)
                            ).ToArrayAsync();

                var schemes = await db.Schemes.ToListAsync();
                
                List<UserViewModel> vm = new List<UserViewModel>();
                var roles = await db.Roles.ToListAsync();
                foreach (var ur in users)
                {
                    var item = new UserViewModel();
                    item.CiAdd = ur.CiAdd;
                    item.CustomIndicators = ur.CustomIndicators;
                    item.Email = ur.Email;
                    item.Expires = ur.Expires;
                    item.PhoneNumber = ur.PhoneNumber;
                    item.Live = ur.Live;
                    item.Name = ur.Name;
                    item.PatternAdd = ur.PatternAdd;
                    item.PhoneNumber = ur.PhoneNumber;
                    item.Scanner = ur.Scanner;
                    item.ScannerAdd = ur.ScannerAdd;
                    item.Scheme = "na";
                    item.SignalAdd = ur.SignalAdd;
                    item.TrendAdd = ur.TrendAdd;
                    var sql = (from r in roles
                              from r2 in ur.Roles
                              where r.Id == r2.RoleId
                              select r.Name).ToList();
                    var rr = string.Join(",", sql);
                    item.Role = rr ?? "User";
                    if (ur.SchemeId.HasValue)
                    {
                        item.Scheme = schemes.FirstOrDefault(x => x.Id == ur.SchemeId).Name;
                    }
                    vm.Add(item);
                }
                return View(vm);
            }
        }

        [Authorize (Roles="Admin")]
        public ActionResult Edit(string email)
        {
            var user = UserManager.FindByEmail(email);
            return View(user);
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Details(string email)
        {
            var user = await UserManager.FindByEmailAsync(email);
            return View(user);
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteConfirm(string email)
        {
            var user = await UserManager.FindByEmailAsync(email);
            return View(user);
        }


        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(string email)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var us = await db.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            db.Users.Remove(us);
            int result = await db.SaveChangesAsync();
            if (result > 0)
            { }
            return RedirectToAction("GetUsers");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(ApplicationUser user)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var us = await db.Users.Where(u => u.Email == user.Email).FirstOrDefaultAsync();
            us.Name = user.Name;
            us.Scanner = user.Scanner;
            us.CustomIndicators = user.CustomIndicators;
            us.Live = user.Live;
            us.CiAdd = user.CiAdd;

            us.ScannerAdd = user.ScannerAdd;
            us.SignalAdd = user.SignalAdd;
            us.TrendAdd = user.TrendAdd;
            us.PatternAdd = user.PatternAdd;
            us.Expires = user.Expires;
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
                    return RedirectToLocal(returnUrl);
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
                string cpt = Session["Captcha"] as string;

                if (!model.Captcha.Equals(cpt))
                {
                    ModelState.AddModelError("", "Wrong Captcha!");
                    return View(model);
                } 
                var user = new ApplicationUser() { 
                    Name = model.Name,
                    PhoneNumber = model.PhoneNumber,
                    PhoneNumberConfirmed = true,
                    UserName = model.Email,
                    Email = model.Email 
                };

                var passwordValidator = (PasswordValidator)UserManager.PasswordValidator;
                var password = Membership.GeneratePassword(passwordValidator.RequiredLength, 1);
                IdentityResult result = await UserManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await SignInAsync(user, isPersistent: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");                    
                    await SendUserRegistrationEmailAsync(model.Email, password);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
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
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
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
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        //
        // POST: /Account/UploadUsers
        [HttpPost]
        public async Task<ActionResult> UploadUsers(ActivateFunctionViewModel vm) {
            var users = GetUsers(vm.Users.InputStream).ToArray();
            var errorEmails = new List<string>();
            var succesUserCount = 0;
            var passwordValidator = (PasswordValidator)UserManager.PasswordValidator;

            

            foreach (var user in users) {
                var txt = shuffle<char>(SmallAscii());
                var nums = shuffle<char>(NumOToT());
                var raw = txt.Take(4).Concat(nums.Take(2));
                var reshuf = shuffle<char>(raw.ToList());
                var cpt = String.Join("", reshuf.ToArray());

                var password = Membership.GeneratePassword(passwordValidator.RequiredLength, 0);
                var result = await UserManager.CreateAsync(user, password);

                if (result.Succeeded) {
                    succesUserCount++;
                    await SendUserRegistrationEmailAsync(user.Email, password);
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

        public ActionResult UploadUserResult() {
            var result = this.TempData.GetUploadUserResult();
            return View(result);
        }

        //
        // GET: /Account/GetUsers
        public async Task<ActionResult> GetUsers() {
            using (var db = new ApplicationDbContext()) {
                List<UserViewModel> vm = new List<UserViewModel>();
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

        private List<char> AsciiNumber()
        {
            var az = Enumerable.Range('A', 'Z' - 'A' + 1).
                      Select(c => (char)c);

            var ot = Enumerable.Range(0, 10).Select(i => Convert.ToChar(i.ToString()));

            return az.Concat(ot).ToList();

        }

        [AllowAnonymous]
        public ActionResult GetGuestCaptcha()
        {
            string cpt;
            byte[] bytes;
            GenerateCaptcha(out cpt, out bytes);
          
            Session["GuestCaptcha"] = cpt;
            return File(bytes, "image/png");
         
        }

        private void GenerateCaptcha(out string cpt, out byte[] bytes)
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
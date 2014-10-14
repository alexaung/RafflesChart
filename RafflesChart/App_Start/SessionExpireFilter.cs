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
namespace RafflesChart.App_Start
{
    public class SessionExpireFilterAttribute : ActionFilterAttribute
    {
       
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;

            if (ctx.Request.Url.AbsolutePath.Equals("/"))
            {
                return;
            }
           if( ctx.Request.Url.AbsolutePath.Contains("/Login")){
               return;
                
            }
            var active = filterContext.HttpContext.Session["activeuser"];
            if (active == null)
            {
                filterContext.HttpContext.GetOwinContext().Authentication.SignOut();
                filterContext.HttpContext.Response.Redirect("/Account/LogOff");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
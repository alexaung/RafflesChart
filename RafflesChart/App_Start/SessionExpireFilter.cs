﻿using System;
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
using System.Web.Routing;
namespace RafflesChart.App_Start
{
    public class SessionExpireFilterAttribute : ActionFilterAttribute
    {
        private void SaveHit(string url)
        {
            using (var context = new ApplicationDbContext())
            {
                var hit = new Hit();
                hit.Url = url;
                hit.CreatedDate = DateTime.Now;

                context.Hits.Add(hit);
                context.SaveChanges();
            }
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            Task.Run(() => SaveHit(ctx.Request.Url.AbsolutePath));
            if (ctx.Request.Url.AbsolutePath.Equals("/"))
            {
                return;
            }
            if (ctx.Request.Url.AbsolutePath.Contains("/Home"))
            {
                return;
            }
           if( ctx.Request.Url.AbsolutePath.Contains("/Login")){
               return;                
            }
           if (ctx.Request.Url.AbsolutePath.Contains("Captcha"))
           {
               return;
           }
           if (ctx.Request.Url.AbsolutePath.Contains("Forgot"))
           {
               return;
           }
           if (ctx.Request.Url.AbsolutePath.Contains("Reset"))
           {
               return;
           }
           if (ctx.Request.Url.AbsolutePath.Contains("/LogOff"))
           {
               return;
           }
           if (ctx.Request.Url.AbsolutePath.Contains("/Events"))
           {
               return;
           }
           if (ctx.Request.Url.AbsolutePath.Contains("EventRegisterMessage"))
           {
               return;
           }
           if (ctx.Request.Url.AbsolutePath.Contains("Register"))
           {
               return;
           }
            var active = filterContext.HttpContext.Session["activeuser"];
            if (active == null)
            {
                //filterContext.HttpContext.GetOwinContext().Authentication.SignOut();
           //     filterContext.Result = new RedirectToRouteResult(
           // new RouteValueDictionary {{ "Controller", "Account" },
           //                           { "Action", "LogOff" } });
                //filterContext.HttpContext.Response.Redirect("/Home");
               
                //return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
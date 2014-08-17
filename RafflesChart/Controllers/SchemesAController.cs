using Newtonsoft.Json;
using RafflesChart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RafflesChart.Controllers
{
    [Authorize(Roles="Admin")]
    public class SchemesAController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //
        // GET: /SchemsA/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Activate()
        {
            return View();
        }

        public ActionResult ActivateFunction()
        {
            return View();
        }

        private char DELIM_SEMICOLON = ';';
        private string DELIM_COMMA = ",";

        public ActionResult FunctionActivate(string fn, string val, string userupload)
        {
            var users = from u in userupload.Split(Environment.NewLine.ToArray())
                        select u;

            var mkt = new string[] { };
            if (!string.IsNullOrEmpty(val))
            {
                mkt = (from u in users
                       from m in val.Split(DELIM_SEMICOLON)
                       select u + DELIM_COMMA + m).ToArray();
            }
            var sbresult = new List<string>();
            GetActivateResult(fn, sbresult, mkt);

            var obj = new { retValue = sbresult };
            return Content(JsonConvert.SerializeObject(obj));
        }

        public ActionResult SchemeActivate(int schemeId, string userupload)
        {
            var sbresult = new List<string>();
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var users = from u in userupload.Split(Environment.NewLine.ToArray())
                                select u;
                    var scheme = db.Schemes.FirstOrDefault(r => r.Id == schemeId);

                    Console.WriteLine("scheme to activate :" + scheme.Name);
                    Console.WriteLine("scheme markets: " + scheme.Markets);


                    var mkt = new string[] { };
                    if (!string.IsNullOrEmpty(scheme.Markets))
                    {
                        mkt = (from u in users
                               from m in scheme.Markets.Split(DELIM_SEMICOLON)
                               select u + DELIM_COMMA + m).ToArray();
                    }
                    var idc = new string[] { };
                    if (!string.IsNullOrEmpty(scheme.Indicators))
                    {
                        idc = (from u in users
                               from i in scheme.Indicators.Split(DELIM_SEMICOLON)
                               select u + DELIM_COMMA + i).ToArray();
                    }

                    var pscan = new string[] { };
                    if (!string.IsNullOrEmpty(scheme.PatternScanners))
                    {
                        pscan = (from u in users
                                 from p in scheme.PatternScanners.Split(DELIM_SEMICOLON)
                                 select u + DELIM_COMMA + p).ToArray();
                    }
                    var scan = new string[] { };
                    if (!string.IsNullOrEmpty(scheme.Scanners))
                    {
                        scan = (from u in users
                                from s in scheme.Scanners.Split(DELIM_SEMICOLON)
                                select u + DELIM_COMMA + s).ToArray();
                    }
                    var bbt = new string[] { };
                    if (!string.IsNullOrEmpty(scheme.BullBearTests))
                    {
                        bbt = (from u in users
                               select u + DELIM_COMMA + scheme.BullBearTests).ToArray();
                    }

                    var bt = new string[] { };
                    if (!string.IsNullOrEmpty(scheme.BackTests))
                    {
                        bt = (from u in users
                              select u + DELIM_COMMA + scheme.BackTests).ToArray();
                    }


                    GetActivateResult("Users Markets", sbresult, mkt);
                    GetActivateResult("Users Indicators", sbresult, idc);
                    GetActivateResult("Users Bull Bear Test", sbresult, bbt);
                    GetActivateResult("Users Back Test", sbresult, bt);
                    GetActivateResult("Users Pattern Scanners", sbresult, pscan);
                    GetActivateResult("Users Scanners", sbresult, scan);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error activating scheme: " + ex.ToString());
            }
            Console.WriteLine("Return result:" + sbresult.Count());

            var sb = sbresult.Select((ss, idx) => (idx + 1).ToString("000") + ss);

            var obj = new { retValue = sb };
            return Content(JsonConvert.SerializeObject(obj));
        }

        private static void GetActivateResult(string title, List<string> sbresult, IEnumerable<string> userdata)
        {
            sbresult.Add(title);
            sbresult.Add(string.Join("", Enumerable.Repeat("*", 18).ToArray()));

            if (userdata != null && userdata.Count() > 0)
            {
                sbresult.AddRange(userdata.ToArray());
            }
            sbresult.Add(string.Join("", Enumerable.Repeat(" ", 35).ToArray()));
        }


	}
}
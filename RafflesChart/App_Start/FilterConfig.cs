using RafflesChart.App_Start;
using System.Web;
using System.Web.Mvc;

namespace RafflesChart {
    public class FilterConfig {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new SessionExpireFilterAttribute());
        }
    }
}

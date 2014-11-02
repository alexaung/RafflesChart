using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RafflesChart.Models
{
    public class SearchUserResult
    {
        public List<Models.UserViewModel> Result { get; set; }

        public System.Web.Mvc.SelectListItem[] Schemes { get; set; }
    }
}

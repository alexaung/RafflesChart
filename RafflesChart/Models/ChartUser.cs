using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RafflesChart.Models
{
    public class ChartUserViewModel
    {
        public ChartUser ChartUserModel { get; set; }
        public ApplicationUser ApplicationUserModel { get; set; }

        public SelectListItem[] Schemes { get; set; }

        public string SelectedScheme {get; set;}
    }

    [System.ComponentModel.DataAnnotations.Schema.Table("User")]
    public class ChartUser
    {
         public Guid Id { get; set; }

         public string Login { get; set; }

         public string Password { get; set; }

        public bool Scanner { get; set; }

        public bool CustomIndicators { get; set; }

        public bool Live { get; set; }

        public bool CI_Add { get; set; }

        public bool Scanner_Add { get; set; }

        public bool Signal_Add { get; set; }

        public bool Trend_Add { get; set; }

        public bool Pattern_Add { get; set; }

        public DateTime Expires { get; set; }
    }
}
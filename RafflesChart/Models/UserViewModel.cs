using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RafflesChart.Models
{
    public class UserViewModel:ApplicationUser
    {
        public string Scheme { get; set; }
    }
}
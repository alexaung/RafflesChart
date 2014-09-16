using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RafflesChart.Models
{
    public class SearchUser
    {
        public string Name{ get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Captcha { get; set; }

        public int EventId { get; set; }
    }
}
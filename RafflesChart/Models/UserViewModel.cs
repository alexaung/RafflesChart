using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RafflesChart.Models
{
    public class UserViewModel
    {
        public string Scheme { get; set; }

        public string Name { get; set; }

        public bool Scanner { get; set; }

        public bool CustomIndicators { get; set; }

        public bool Live { get; set; }

        public bool CiAdd { get; set; }

        public bool ScannerAdd { get; set; }

        public bool SignalAdd { get; set; }

        public bool TrendAdd { get; set; }

        public bool PatternAdd { get; set; }

        public double Expires { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }

        public double? ModifiedDate { get; set; }

        public bool Picked { get; set; }

        public DateTime ExpiresDate { get; set; }
    }
}
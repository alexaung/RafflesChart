using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RafflesChart.Models {
    public class Scheme {
        public int Id { get; set; }

        public String Description { get; set; }

        public String Name { get; set; }

        public ICollection<UserMarket> UserMarkets { get; set; }

        public ICollection<UserIndicator> UserIndicators { get; set; }

        public ICollection<UserBullBearTest> UserBullBearTests { get; set; }

        public ICollection<UserBackTest> UserBackTests { get; set; }

        public ICollection<PatternScanner> PatternScanners { get; set; }

        public ICollection<Scanner> Scanners { get; set; }

        public bool Scanner { get; set; }

        public bool CustomIndicators { get; set; }

        public bool Live { get; set; }

        public bool CiAdd { get; set; }

        public bool ScannerAdd { get; set; }

        public bool SignalAdd { get; set; }

        public bool TrendAdd { get; set; }

        public bool PatternAdd { get; set; }

        public bool Expires { get; set; }


    }
}
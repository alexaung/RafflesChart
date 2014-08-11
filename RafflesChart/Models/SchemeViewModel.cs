using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RafflesChart.Models {
    public class SchemeViewModel {
        public int Id { get; set; }

        public String Description { get; set; }

        public String Name { get; set; }        

        public bool Scanner { get; set; }

        [DisplayName("Custom Indicators")]
        public bool CustomIndicators { get; set; }

        public bool Live { get; set; }

        public bool CiAdd { get; set; }

        public bool ScannerAdd { get; set; }

        public bool SignalAdd { get; set; }

        public bool TrendAdd { get; set; }

        public bool PatternAdd { get; set; }

        public bool Expires { get; set; }

        [DisplayName("Markets")]
        public ICollection<int> UserMarketIds { get; set; }

        [DisplayName("Markets")]
        public ICollection<SelectListItem> AllUserMarkets { get; set; }

        [DisplayName("Indicators")]
        public ICollection<int> UserIndicatorIds { get; set; }

        [DisplayName("Indicators")]
        public ICollection<SelectListItem> AllUserIndicators { get; set; }


        [DisplayName("Bull Bear Tests")]
        public ICollection<int> UserBullBearTestIds { get; set; }

        [DisplayName("Bull Bear Tests")]
        public ICollection<SelectListItem> AllUserBullBearTests { get; set; }

        [DisplayName("Back Tests")]
        public ICollection<int> UserBackTestIds { get; set; }

        [DisplayName("Back Tests")]
        public ICollection<SelectListItem> AllUserBackTests { get; set; }


        [DisplayName("Pattern Scanners")]
        public ICollection<int> PatternScannerIds { get; set; }

        [DisplayName("Pattern Scanners")]
        public ICollection<SelectListItem> AllPatternScanners { get; set; }

        [DisplayName("Scanners")]
        public ICollection<int> ScannerIds { get; set; }

        [DisplayName("Scanners")]
        public ICollection<SelectListItem> AllScanners { get; set; }

    }
}
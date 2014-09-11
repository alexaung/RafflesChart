using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RafflesChart.Models
{
    public class SchemeUserViewModel
    {
        public IEnumerable<string> UserMarkets;
        public IEnumerable<string> UserIndicators;
        public IEnumerable<string> UserBackTests;
        public IEnumerable<string> UserBullBearTests;
        public IEnumerable<string> UserPatternScanners;
        public IEnumerable<string> UserScanners;
    }
}